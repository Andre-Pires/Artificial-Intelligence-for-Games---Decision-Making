
using Assets.Scripts.IAJ.Unity.Pathfinding;
using Assets.Scripts.IAJ.Unity.Pathfinding.Path;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.Arbitration.SteeringPipeline.Components.Decomposers
{
    public class PathFindingDecomposer : DecomposerComponent
    {
        private AStarPathfinding PathfindingAlgorithm { get; set; }

        public GlobalPath CurrentPath { get; private set; }

        public GlobalPath UnsmoothedPath { get; private set; }

        private Vector3 CurrentGoalPosition { get; set; }

        public PathFindingDecomposer(SteeringPipeline pipeline, AStarPathfinding pathfindingAlgorithm) : base(pipeline)
        {
            this.PathfindingAlgorithm = pathfindingAlgorithm;
        }

        public override GlobalPath DecomposeGoalIntoPath(SteeringGoal goal)
        {
            GlobalPath path;

            if (goal.PositionSet && !this.CurrentGoalPosition.Equals(goal.Position))
            {
                this.CurrentGoalPosition = goal.Position;
                this.PathfindingAlgorithm.InitializePathfindingSearch(this.Pipeline.Character.position,goal.Position);
            }

            if (this.PathfindingAlgorithm.InProgress)
            {
                if (this.PathfindingAlgorithm.Search(out path, true))
                {
                    this.UnsmoothedPath = path;
                    this.CurrentPath = StringPullingPathSmoothing.SmoothPath(this.Pipeline.Character, path);
                    this.CurrentPath.CalculateLocalPathsFromPathPositions(this.Pipeline.Character.position);
                }
                else
                {
                    this.UnsmoothedPath = path;
                    this.CurrentPath = path;
                }
            }

            return this.CurrentPath;
        }

        public override SteeringGoal DecomposeGoal(SteeringGoal goal)
        {
            var path = this.DecomposeGoalIntoPath(goal);
            return new SteeringGoal
            {
                Position = path.PathPositions[0]
            };
        }
    }
}
