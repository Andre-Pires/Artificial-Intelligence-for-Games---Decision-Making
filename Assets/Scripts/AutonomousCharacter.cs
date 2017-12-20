using Assets.Scripts.DecisionMakingActions;
using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using Assets.Scripts.IAJ.Unity.Movement.Arbitration.SteeringPipeline;
using Assets.Scripts.IAJ.Unity.Movement.Arbitration.SteeringPipeline.Components.Actuators;
using Assets.Scripts.IAJ.Unity.Movement.Arbitration.SteeringPipeline.Components.Decomposers;
using Assets.Scripts.IAJ.Unity.Movement.Arbitration.SteeringPipeline.Components.Targeters;
using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using Assets.Scripts.IAJ.Unity.Pathfinding;
using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures;
using Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics;
using Assets.Scripts.IAJ.Unity.TacticalAnalysis;
using Assets.Scripts.IAJ.Unity.TacticalAnalysis.DataStructures;
using RAIN.Navigation;
using RAIN.Navigation.Graph;
using RAIN.Navigation.NavMesh;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions.Comparers;
using UnityEngine.UI;
using Action = Assets.Scripts.IAJ.Unity.DecisionMaking.GOB.Action;

namespace Assets.Scripts
{
    public class AutonomousCharacter : MonoBehaviour
    {
        //constants
        public const string SURVIVE_GOAL = "Survive";

        public const int SURVIVE_GOAL_INDEX = 0;
        public const string EAT_GOAL = "Eat";
        public const int EAT_GOAL_INDEX = 1;
        public const string REST_GOAL = "Rest";
        public const int REST_GOAL_INDEX = 2;
        public const string GET_RICH_GOAL = "GetRich";
        public const int GET_RICH_GOAL_INDEX = 3;
        public const string CONQUER_GOAL = "Conquer";
        public const int CONQUER_GOAL_INDEX = 4;

        public const float DECISION_MAKING_INTERVAL = 1.5f;

        //public fields to be set in Unity Editor
        public GameManager.GameManager GameManager;

        public TextMesh ActionText;
        public Text SurviveGoalText;
        public Text EatGoalText;
        public Text RestGoalText;
        public Text GetRichGoalText;
        public Text ConquerGoalText;
        public Text TotalProcessingTimeText;
        public Text BestDiscontentmentText;
        public Text ProcessedActionsText;
        public Text BestActionText;

        public Goal RestGoal { get; private set; }
        public Goal SurviveGoal { get; private set; }
        public Goal GetRichGoal { get; private set; }
        public Goal EatGoal { get; private set; }
        public Goal ConquerGoal { get; private set; }
        public List<Goal> Goals { get; set; }
        public List<Action> Actions { get; set; }
        public Action CurrentAction { get; private set; }
        public DynamicCharacter Character { get; private set; }
        public FixedTargeter Targeter { get; set; }
        public DepthLimitedGOAPDecisionMaking GOAPDecisionMaking { get; set; }
        public InfluenceMap RedInfluenceMap { get; set; }
        public InfluenceMap GreenInfluenceMap { get; set; }
        public InfluenceMap ResourceInfluenceMap { get; set; }

        public List<IInfluenceUnit> RedFlags { get; set; }
        public List<IInfluenceUnit> GreenFlags { get; set; }
        public Dictionary<NavigationGraphNode, IInfluenceUnit> ActiveResources { get; set; }
        public Dictionary<LocationRecord, float> CombinedInfluence { get; set; }
        public Vector3 BestFlagPosition { get; set; }
        public float BestCombinedInfluence { get; set; }
        public NavMeshPathGraph NavMesh { get; set; }

        //private fields for internal use only

        private AStarPathfinding aStarPathFinding;
        private PathFindingDecomposer decomposer;

        private bool draw;
        private uint influenceMapDebugMode;

        private float nextUpdateTime = 0.0f;
        private float previousGold = 0.0f;

        public void Start()
        {
            this.draw = true;
            this.influenceMapDebugMode = 0;

            this.NavMesh = NavigationManager.Instance.NavMeshGraphs[0];
            this.Character = new DynamicCharacter(this.gameObject);

            //initialization of the movement algorithms
            this.aStarPathFinding = new NodeArrayAStarPathFinding(this.NavMesh, new EuclideanDistanceHeuristic(), this);
            this.aStarPathFinding.NodesPerSearch = 500;

            var steeringPipeline = new SteeringPipeline
            {
                MaxAcceleration = 40.0f,
                MaxConstraintSteps = 2,
                Character = this.Character.KinematicData,
            };

            this.decomposer = new PathFindingDecomposer(steeringPipeline, this.aStarPathFinding);
            this.Targeter = new FixedTargeter(steeringPipeline);
            steeringPipeline.Targeters.Add(this.Targeter);
            steeringPipeline.Decomposers.Add(this.decomposer);
            steeringPipeline.Actuator = new FollowPathActuator(steeringPipeline);

            this.Character.Movement = steeringPipeline;

            //initialization of the Influence Maps
            this.RedInfluenceMap = new InfluenceMap(this.NavMesh, new LocationPriorityHeap(), new ClosedLocationRecordDictionary(), new LinearInfluenceFunction(), 0.1f);
            this.GreenInfluenceMap = new InfluenceMap(this.NavMesh, new LocationPriorityHeap(), new ClosedLocationRecordDictionary(), new LinearInfluenceFunction(), 0.1f);
            this.ResourceInfluenceMap = new InfluenceMap(this.NavMesh, new LocationPriorityHeap(), new ClosedLocationRecordDictionary(), new LinearInfluenceFunction(), 0.1f);
            this.CombinedInfluence = new Dictionary<LocationRecord, float>();

            //initialization of the GOB decision making
            //let's start by creating 5 main goals
            //the eat goal is the only goal that increases at a fixed rate per second, it increases at a rate of 0.1 per second
            this.SurviveGoal = new Goal(SURVIVE_GOAL, 1.5f);
            this.EatGoal = new Goal(EAT_GOAL, 1.5f)
            {
                ChangeRate = 0.1f
            };
            this.GetRichGoal = new Goal(GET_RICH_GOAL, 1.0f)
            {
                InsistenceValue = 5.0f,
                ChangeRate = 0.1f
            };
            this.RestGoal = new Goal(REST_GOAL, 2.0f);
            this.ConquerGoal = new Goal(CONQUER_GOAL, 1.0f)
            {
                InsistenceValue = 5.0f
            };

            this.Goals = new List<Goal>();
            this.Goals.Add(this.SurviveGoal);
            this.Goals.Add(this.EatGoal);
            this.Goals.Add(this.GetRichGoal);
            this.Goals.Add(this.RestGoal);
            this.Goals.Add(this.ConquerGoal);

            //initialize the available actions

            var restAction = new Rest(this);
            this.Actions = new List<Action>();
            this.Actions.Add(restAction);
            this.Actions.Add(new PlaceFlag(this));

            this.ActiveResources = new Dictionary<NavigationGraphNode, IInfluenceUnit>();
            var resourceIndex = 0;

            foreach (var chest in GameObject.FindGameObjectsWithTag("Chest"))
            {
                var resourceNode = this.NavMesh.QuantizeToNode(chest.transform.position, 1.0f);
                this.Actions.Add(new PickUpChest(this, chest, resourceNode, resourceIndex));
                this.AddResource(new Resource(resourceNode));
                resourceIndex++;
            }

            foreach (var tree in GameObject.FindGameObjectsWithTag("Tree"))
            {
                var resourceNode = this.NavMesh.QuantizeToNode(tree.transform.position, 1.0f);
                this.Actions.Add(new GetArrows(this, tree, resourceNode, resourceIndex));
                this.AddResource(new Resource(resourceNode));
                resourceIndex++;
            }

            foreach (var bed in GameObject.FindGameObjectsWithTag("Bed"))
            {
                var resourceNode = this.NavMesh.QuantizeToNode(bed.transform.position, 1.0f);
                this.Actions.Add(new Sleep(this, bed, resourceNode, resourceIndex));
                this.AddResource(new Resource(resourceNode));
                resourceIndex++;
            }

            foreach (var boar in GameObject.FindGameObjectsWithTag("Boar"))
            {
                var resourceNode = this.NavMesh.QuantizeToNode(boar.transform.position, 1.0f);
                this.AddResource(new Resource(resourceNode));
                this.Actions.Add(new MeleeAttack(this, boar, resourceNode, resourceIndex));
                this.Actions.Add(new Shoot(this, boar, resourceNode, resourceIndex));
                resourceIndex++;
            }

            this.ResourceInfluenceMap.Initialize(this.ActiveResources.Values.ToList());

            //flags used for the influence map
            this.RedFlags = new List<IInfluenceUnit>();
            foreach (var redFlag in GameObject.FindGameObjectsWithTag("RedFlag"))
            {
                this.RedFlags.Add(new Flag(this.NavMesh.QuantizeToNode(redFlag.transform.position, 1.0f), FlagColor.Red));
            }

            this.GreenFlags = new List<IInfluenceUnit>();
            foreach (var greenFlag in GameObject.FindGameObjectsWithTag("GreenFlag"))
            {
                this.GreenFlags.Add(new Flag(this.NavMesh.QuantizeToNode(greenFlag.transform.position, 1.0f), FlagColor.Green));
            }

            this.RedInfluenceMap.Initialize(this.RedFlags);
            this.GreenInfluenceMap.Initialize(this.GreenFlags);

            var worldModel = new CurrentStateWorldModel(this.GameManager, this.Actions, this.Goals, resourceIndex);

            this.GOAPDecisionMaking = new DepthLimitedGOAPDecisionMaking(worldModel, this.Actions, this.Goals);
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                this.ChangeDebugInfluenceMap();
            }

            if (Time.time > this.nextUpdateTime)
            {
                this.nextUpdateTime = Time.time + DECISION_MAKING_INTERVAL;

                //first step, perceptions
                //update the agent's goals based on the state of the world
                this.SurviveGoal.InsistenceValue = 10 - this.GameManager.characterData.HP;
                this.EatGoal.InsistenceValue = this.GameManager.characterData.Hunger;
                this.RestGoal.InsistenceValue = 10.0f - this.GameManager.characterData.Energy;

                //the get rich goal is managed by DecisionMaking process because it does not correspond to a physiological need handled in the world
                this.GetRichGoal.InsistenceValue += 0.1f; //decay
                if (this.GetRichGoal.InsistenceValue > 10)
                {
                    this.GetRichGoal.InsistenceValue = 10.0f;
                }
                this.ConquerGoal.InsistenceValue += 0.1f;
                if (this.ConquerGoal.InsistenceValue > 10)
                {
                    this.ConquerGoal.InsistenceValue = 10;
                }

                if (this.GameManager.characterData.Money > this.previousGold)
                {
                    this.GetRichGoal.InsistenceValue -= this.GameManager.characterData.Money - this.previousGold;
                    this.previousGold = this.GameManager.characterData.Money;
                }

                this.SurviveGoalText.text = "Survive: " + this.SurviveGoal.InsistenceValue;
                this.EatGoalText.text = "Eat: " + this.EatGoal.InsistenceValue;
                this.RestGoalText.text = "Rest: " + this.RestGoal.InsistenceValue;
                this.GetRichGoalText.text = "GetRich: " + this.GetRichGoal.InsistenceValue;
                this.ConquerGoalText.text = "Conquer: " + this.ConquerGoal.InsistenceValue;

                //initialize GOAP Decision Making Proccess
                this.GOAPDecisionMaking.InitializeDecisionMakingProcess();
            }

            //process the influence maps
            if (this.RedInfluenceMap.InProgress)
            {
                if (this.RedInfluenceMap.MapFloodDijkstra())
                {
                    this.CalculateCombinedInfluence();
                }
            }
            if (this.GreenInfluenceMap.InProgress)
            {
                if (this.GreenInfluenceMap.MapFloodDijkstra())
                {
                    this.CalculateCombinedInfluence();
                }
            }
            if (this.ResourceInfluenceMap.InProgress)
            {
                if (this.ResourceInfluenceMap.MapFloodDijkstra())
                {
                    this.CalculateCombinedInfluence();
                }
            }

            if (this.GOAPDecisionMaking.InProgress)
            {
                //choose an action using the GOB Decision Making process
                var action = this.GOAPDecisionMaking.ChooseAction();
                if (action != null)
                {
                    action.Execute();
                    this.CurrentAction = action;
                    this.ActionText.text = this.CurrentAction.Name;
                }
            }

            this.TotalProcessingTimeText.text = "Processing Time: " + this.GOAPDecisionMaking.TotalProcessingTime;
            this.BestDiscontentmentText.text = "Best Discontentment: " + this.GOAPDecisionMaking.BestDiscontentmentValue;
            this.ProcessedActionsText.text = "Action comb. processed: " + this.GOAPDecisionMaking.TotalActionCombinationsProcessed;

            if (this.GOAPDecisionMaking.BestActionSequence != null && this.GOAPDecisionMaking.BestActionSequence[0] != null)
            {
                var actionText = "";
                foreach (var action in this.GOAPDecisionMaking.BestActionSequence)
                {
                    actionText += "\n" + action.Name;
                }
                this.BestActionText.text = "Best Action Sequence: " + actionText;
            }
            else
            {
                this.BestActionText.text = "Best Action Sequence:\nNone";
            }

            this.Character.Update();
        }

        private void AddResource(Resource resource)
        {
            if (!this.ActiveResources.ContainsKey(resource.Location))
            {
                this.ActiveResources.Add(resource.Location, resource);
            }
        }

        private void CalculateCombinedInfluence()
        {
            //this can only be done when all influence maps are calculated
            if (this.RedInfluenceMap.InProgress || this.GreenInfluenceMap.InProgress || this.ResourceInfluenceMap.InProgress)
            {
                return;
            }

            this.CombinedInfluence.Clear();
            this.BestCombinedInfluence = 0;
            this.BestFlagPosition = Vector3.zero;

            List<LocationRecord> resourceRecords = new List<LocationRecord>(ResourceInfluenceMap.Closed.All());

            if (resourceRecords.Count == 0)
                return;

            for (int i = 0; i < resourceRecords.Count; i++)
            {
                LocationRecord redRecord = RedInfluenceMap.Closed.SearchInClosed(resourceRecords[i]);
                float redInfluence = 0.0f;

                if (redRecord != null)
                {
                    redInfluence = redRecord.Influence;
                }
                LocationRecord greenRecord = GreenInfluenceMap.Closed.SearchInClosed(resourceRecords[i]);
                float greenInfluence = 0.0f;
                if (greenRecord != null)
                {
                    greenInfluence = greenRecord.Influence;
                }

                float redFlagDistance = float.MaxValue;
                foreach (IInfluenceUnit flag in RedFlags)
                {
                    float distance = (resourceRecords[i].Location.LocalPosition - flag.Location.LocalPosition).magnitude;
                    if (distance < redFlagDistance)
                    {
                        redFlagDistance = distance;
                    }
                }

                //NOTE: the value of the literal changes how far from the flag another flag is placed
                redFlagDistance = 70 - redFlagDistance;
                if (redFlagDistance < 0)
                    redFlagDistance = 0;
                redFlagDistance = -redFlagDistance;

                float combinedInfluence = 0;

                combinedInfluence += resourceRecords[i].Influence * 6.0f;
                combinedInfluence += (redInfluence - greenInfluence) * 2.0f;
                combinedInfluence += redFlagDistance;

                LocationRecord record = new LocationRecord()
                {
                    Location = resourceRecords[i].Location,
                    Influence = combinedInfluence
                };

                CombinedInfluence.Add(record, record.Influence);

                if (combinedInfluence > BestCombinedInfluence)
                {
                    this.BestCombinedInfluence = combinedInfluence;
                    this.BestFlagPosition = record.Location.LocalPosition;
                }
            }

            if (BestCombinedInfluence == 0)
            {
                this.BestFlagPosition = Vector3.zero;
            }
        }

        public void UpdateRedFlags(ICollection<GameObject> redFlags)
        {
            //flags used for the influence map
            this.RedFlags = new List<IInfluenceUnit>();
            foreach (var redFlag in redFlags)
            {
                this.RedFlags.Add(new Flag(this.NavMesh.QuantizeToNode(redFlag.transform.position, 1.0f), FlagColor.Red));
            }

            this.RedInfluenceMap.Initialize(this.RedFlags);
            this.BestCombinedInfluence = 0;
        }

        public void UpdateResources(ICollection<GameObject> resources)
        {
            this.ActiveResources.Clear();
            foreach (var resource in resources)
            {
                this.AddResource(new Resource(this.NavMesh.QuantizeToNode(resource.transform.position, 1.0f)));
            }

            this.ResourceInfluenceMap.Initialize(this.ActiveResources.Values.ToList());
            this.BestCombinedInfluence = 0;
        }

        private void ChangeDebugInfluenceMap()
        {
            if (this.influenceMapDebugMode == 3)
            {
                this.influenceMapDebugMode = 0;
            }
            else
            {
                this.influenceMapDebugMode++;
            }
        }

        public void OnDrawGizmos()
        {
            if (this.draw)
            {
                var size = new Vector3(2, 1, 2);

                if (this.influenceMapDebugMode == 0)
                {
                    var red = Color.red;
                    Gizmos.color = red;
                    foreach (var locationRecord in this.RedInfluenceMap.Closed.All())
                    {
                        red.a = (1 / 5.0f) * locationRecord.Influence;
                        Gizmos.color = red;
                        Gizmos.DrawCube(locationRecord.Location.LocalPosition, size);
                    }
                }
                else if (this.influenceMapDebugMode == 1)
                {
                    var green = Color.green;
                    Gizmos.color = green;
                    foreach (var locationRecord in this.GreenInfluenceMap.Closed.All())
                    {
                        green.a = 1 / 5.0f * locationRecord.Influence;
                        Gizmos.color = green;
                        Gizmos.DrawCube(locationRecord.Location.LocalPosition, size);
                    }
                }
                else if (this.influenceMapDebugMode == 2)
                {
                    var yellow = Color.yellow;
                    Gizmos.color = yellow;
                    foreach (var locationRecord in this.ResourceInfluenceMap.Closed.All())
                    {
                        yellow.a = 1 / 2.5f * locationRecord.Influence;
                        Gizmos.color = yellow;
                        Gizmos.DrawCube(locationRecord.Location.LocalPosition, size);
                    }
                }
                else if (this.influenceMapDebugMode == 3)
                {
                    var blue = Color.blue;
                    Gizmos.color = blue;
                    foreach (var locationRecord in this.CombinedInfluence)
                    {
                        blue.a = 1 / 5.0f * locationRecord.Value;
                        Gizmos.color = blue;
                        Gizmos.DrawCube(locationRecord.Key.Location.LocalPosition, size);
                    }
                }

                //draw the current Solution Path if any (for debug purposes)
                if (this.decomposer.UnsmoothedPath != null)
                {
                    var previousPosition = this.Character.KinematicData.position;
                    foreach (var pathPosition in this.decomposer.UnsmoothedPath.PathPositions)
                    {
                        Debug.DrawLine(previousPosition, pathPosition, Color.red);
                        previousPosition = pathPosition;
                    }

                    previousPosition = this.Character.KinematicData.position;
                    foreach (var pathPosition in this.decomposer.CurrentPath.PathPositions)
                    {
                        Debug.DrawLine(previousPosition, pathPosition, Color.green);
                        previousPosition = pathPosition;
                    }
                }

                //draw the nodes in Open and Closed Sets
                //if (this.aStarPathFinding != null)
                //{
                //    Gizmos.color = Color.cyan;

                //    if (this.aStarPathFinding.Open != null)
                //    {
                //        foreach (var nodeRecord in this.aStarPathFinding.Open.All())
                //        {
                //            Gizmos.DrawSphere(nodeRecord.node.LocalPosition, 1.0f);
                //        }
                //    }

                //    Gizmos.color = Color.blue;

                //    if (this.aStarPathFinding.Closed != null)
                //    {
                //        foreach (var nodeRecord in this.aStarPathFinding.Closed.All())
                //        {
                //            Gizmos.DrawSphere(nodeRecord.node.LocalPosition, 1.0f);
                //        }
                //    }
                //}
            }
        }
    }
}