using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures;
using Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics;
using Assets.Scripts.IAJ.Unity.Pathfinding.Path;
using Assets.Scripts.IAJ.Unity.TacticalAnalysis.DataStructures;
using RAIN.Navigation.Graph;
using RAIN.Navigation.NavMesh;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding
{
    public class NodeArrayAStarPathFinding : AStarPathfinding
    {
        protected NodeRecordArray NodeRecordArray { get; set; }
        protected AutonomousCharacter AutonomousCharacter;
        protected const float AvoidanceMargin = 400;

        public NodeArrayAStarPathFinding(NavMeshPathGraph graph, IHeuristic heuristic, AutonomousCharacter autonomousCharacter) : base(graph, null, null, heuristic)
        {
            //do not change this
            var nodes = this.GetNodesHack(graph);
            this.NodeRecordArray = new NodeRecordArray(nodes);
            this.Open = this.NodeRecordArray;
            this.Closed = this.NodeRecordArray;
            this.AutonomousCharacter = autonomousCharacter;
        }

        protected void ProcessChildNode(NodeRecord bestNode, NavigationGraphEdge connectionEdge)
        {
            float f;
            float g;
            float h;

            var childNode = connectionEdge.ToNode;
            var fromNode = connectionEdge.FromNode;

            var childNodeRecord = this.NodeRecordArray.GetNodeRecord(childNode);

            if (childNodeRecord == null)
            {
                //this piece of code is used just because of the special start nodes and goal nodes added to the RAIN Navigation graph when a new search is performed.
                //Since these special goals were not in the original navigation graph, they will not be stored in the NodeRecordArray and we will have to add them
                //to a special structure
                //it's ok if you don't understand this, this is a hack and not part of the NodeArrayA* algorithm
                childNodeRecord = new NodeRecord
                {
                    node = childNode,
                    parent = bestNode,
                    status = NodeStatus.Unvisited
                };
                this.NodeRecordArray.AddSpecialCaseNode(childNodeRecord);
            }

            // implement the rest of your code here

            if (childNodeRecord.status == NodeStatus.Closed) return;

            g = bestNode.gValue + connectionEdge.Cost;
            h = this.Heuristic.H(childNode, this.GoalNode);

            LocationRecord dummyRecord = new LocationRecord()
            {
                Location = childNode
            };

            float redInfluence = 0f, greenInfluence = 0f;

            // gets the influences for the child node
            LocationRecord redRecord = AutonomousCharacter.RedInfluenceMap.Closed.SearchInClosed(dummyRecord);
            if (redRecord != null)
            {
                redInfluence = redRecord.Influence;
            }

            LocationRecord greenRecord = AutonomousCharacter.GreenInfluenceMap.Closed.SearchInClosed(dummyRecord);
            if (greenRecord != null)
            {
                greenInfluence = greenRecord.Influence;
            }

            dummyRecord = new LocationRecord()
            {
                Location = fromNode
            };

            // gets the influences for the origin node
            redRecord = AutonomousCharacter.RedInfluenceMap.Closed.SearchInClosed(dummyRecord);
            if (redRecord != null)
            {
                redInfluence += redRecord.Influence;
            }

            greenRecord = AutonomousCharacter.GreenInfluenceMap.Closed.SearchInClosed(dummyRecord);
            if (greenRecord != null)
            {
                greenInfluence += greenRecord.Influence;
            }

            // calculates the average value of the path
            greenInfluence /= 2;
            redInfluence /= 2;

            // add the impact of the map's influence
            h += (greenInfluence - redInfluence) * AvoidanceMargin;

            f = F(g, h);

            if (childNodeRecord.status == NodeStatus.Open)
            {
                if (f <= childNodeRecord.fValue)
                {
                    childNodeRecord.gValue = g;
                    childNodeRecord.hValue = h;
                    childNodeRecord.fValue = f;
                    childNodeRecord.parent = bestNode;
                    this.NodeRecordArray.Replace(childNodeRecord, childNodeRecord);
                }
            }
            else
            {
                childNodeRecord.gValue = g;
                childNodeRecord.hValue = h;
                childNodeRecord.fValue = f;
                childNodeRecord.status = NodeStatus.Open;
                childNodeRecord.parent = bestNode;
                this.NodeRecordArray.AddToOpen(childNodeRecord);
            }
        }

        public override bool Search(out GlobalPath solution, bool returnPartialSolution = false)
        {
            var startTime = Time.realtimeSinceStartup;
            var processedNodes = 0;
            int count;

            while (processedNodes < this.NodesPerSearch)
            {
                count = this.Open.CountOpen();
                if (count == 0)
                {
                    solution = null;
                    this.InProgress = false;
                    this.CleanUp();
                    this.TotalProcessingTime += Time.realtimeSinceStartup - startTime;
                    return true;
                }

                if (count > this.MaxOpenNodes)
                {
                    this.MaxOpenNodes = count;
                }

                var bestNode = this.NodeRecordArray.GetBestAndRemove();

                //goal node found, return the shortest Path
                if (bestNode.node == this.GoalNode)
                {
                    solution = this.CalculateSolution(bestNode, false);
                    this.InProgress = false;
                    this.CleanUp();
                    this.TotalProcessingTime += Time.realtimeSinceStartup - startTime;
                    return true;
                }

                this.NodeRecordArray.AddToClosed(bestNode);

                processedNodes++;
                this.TotalProcessedNodes++;

                //put your code here

                //or if you would like, you can change just these lines of code this in the original A* Pathfinding Class,
                //create a ProcessChildNode method in the base class with the code from the previous A* algorithm.
                //if you do this, then you don't need to implement this search method method. Don't forget to override the ProcessChildMethod if you do this
                var outConnections = bestNode.node.OutEdgeCount;
                for (int i = 0; i < outConnections; i++)
                {
                    this.ProcessChildNode(bestNode, bestNode.node.EdgeOut(i));
                }
            }

            this.TotalProcessingTime += Time.realtimeSinceStartup - startTime;

            //this is very unlikely but it might happen that we process all nodes alowed in this cycle but there are no more nodes to process
            if (this.Open.CountOpen() == 0)
            {
                solution = null;
                this.InProgress = false;
                this.CleanUp();
                return true;
            }

            //if the caller wants create a partial Path to reach the current best node so far
            if (returnPartialSolution)
            {
                var bestNodeSoFar = this.Open.PeekBest();
                solution = this.CalculateSolution(bestNodeSoFar, true);
            }
            else
            {
                solution = null;
            }
            return false;
        }

        private List<NavigationGraphNode> GetNodesHack(NavMeshPathGraph graph)
        {
            //this hack is needed because in order to implement NodeArrayA* you need to have full acess to all the nodes in the navigation graph in the beginning of the search
            //unfortunately in RAINNavigationGraph class the field which contains the full List of Nodes is private
            //I cannot change the field to public, however there is a trick in C#. If you know the name of the field, you can access it using reflection (even if it is private)
            //using reflection is not very efficient, but it is ok because this is only called once in the creation of the class
            //by the way, NavMeshPathGraph is a derived class from RAINNavigationGraph class and the _pathNodes field is defined in the base class,
            //that's why we're using the type of the base class in the reflection call
            return (List<NavigationGraphNode>)Utils.Reflection.GetInstanceField(typeof(RAINNavigationGraph), graph, "_pathNodes");
        }
    }
}