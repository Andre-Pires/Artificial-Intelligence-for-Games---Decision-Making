using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using Assets.Scripts.IAJ.Unity.TacticalAnalysis.DataStructures;
using RAIN.Navigation.Graph;
using UnityEditor;
using UnityEngine;
using Action = Assets.Scripts.IAJ.Unity.DecisionMaking.GOB.Action;

namespace Assets.Scripts.DecisionMakingActions
{
    public abstract class WalkToTargetAndExecuteAction : Action
    {
        protected AutonomousCharacter Character { get; set; }

        protected GameObject Target { get; set; }

        private LocationRecord DummyRecord { get; set; }

        protected int ResourceIndex { get; set; }

        protected WalkToTargetAndExecuteAction(string actionName, AutonomousCharacter character, GameObject target, NavigationGraphNode targetNode, int resourceIndex) : base(actionName + "(" + target.name + ")")
        {
            this.Character = character;
            this.Target = target;
            this.ResourceIndex = resourceIndex;

            this.DummyRecord = new LocationRecord()
            {
                Location = targetNode
            };
        }

        public override float GetDuration()
        {
            //assume a velocity of 20.0f/s to get to the target
            return (this.Target.transform.position - this.Character.Character.KinematicData.position).magnitude / 40.0f;
        }

        public override float GetDuration(WorldModel worldModel)
        {
            //assume a velocity of 20.0f/s to get to the target
            var position = (Vector3)worldModel.GetProperty(Properties.POSITION_INDEX);
            return (this.Target.transform.position - position).magnitude / 40.0f;
        }

        public override float GetGoalChange(Goal goal)
        {
            var distance = (this.Target.transform.position - this.Character.Character.KinematicData.position).magnitude;
            if (goal.Name == AutonomousCharacter.REST_GOAL)
            {
                //+0.01 * distance because of the walk
                return distance * 0.01f;
            }
            else if (goal.Name == AutonomousCharacter.EAT_GOAL)
            {
                return distance * 0.1f;
            }
            else return 0;
        }

        public override bool CanExecute()
        {
            if (this.Target == null) return false;
            return CheckLocationSecurity();
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            if (this.Target == null) return false;

            if (!CheckLocationSecurity()) return false;

            var targetEnabled = (bool)worldModel.GetResourceStatus(this.ResourceIndex);
            return targetEnabled;
        }

        private bool CheckLocationSecurity()
        {
            LocationRecord redRecord = Character.RedInfluenceMap.Closed.SearchInClosed(this.DummyRecord);
            LocationRecord greenRecord = Character.GreenInfluenceMap.Closed.SearchInClosed(this.DummyRecord);
            float redInfluence = 0, greenInfluence = 0;

            if (redRecord != null)
            {
                redInfluence = redRecord.Influence;
            }

            if (greenRecord != null)
            {
                greenInfluence = greenRecord.Influence;
            }

            if (redInfluence - greenInfluence <= 0.0f)
            {
                return false;
            }

            return true;
        }

        public override void Execute()
        {
            this.Character.Targeter.Target.Position = this.Target.transform.position;
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            var duration = this.GetDuration(worldModel);
            var energyChange = duration * 0.01f;
            var hungerChange = duration * 0.1f;

            var restValue = worldModel.GetGoalValue(AutonomousCharacter.REST_GOAL_INDEX);
            worldModel.SetGoalValue(AutonomousCharacter.REST_GOAL_INDEX, restValue + energyChange);

            var energy = (float)worldModel.GetProperty(Properties.ENERGY_INDEX);
            worldModel.SetProperty(Properties.ENERGY_INDEX, energy - energyChange);

            var eatGoalValue = worldModel.GetGoalValue(AutonomousCharacter.EAT_GOAL_INDEX);
            worldModel.SetGoalValue(AutonomousCharacter.EAT_GOAL_INDEX, eatGoalValue + hungerChange);

            var hunger = (float)worldModel.GetProperty(Properties.HUNGER_INDEX);
            worldModel.SetProperty(Properties.HUNGER_INDEX, hunger + hungerChange);

            worldModel.SetProperty(Properties.POSITION_INDEX, Target.transform.position);
        }
    }
}