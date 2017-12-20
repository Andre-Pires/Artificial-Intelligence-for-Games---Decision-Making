using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using Assets.Scripts.IAJ.Unity.Pathfinding.Path;

namespace Assets.Scripts.IAJ.Unity.Movement.Arbitration.SteeringPipeline.Components.Actuators
{
    public class FollowPathActuator : ActuatorComponent
    {
        private DynamicFollowPath FollowPathMovement { get; set; }
        private Path PreviousPath { get; set; }

        public FollowPathActuator(SteeringPipeline pipeline) : base(pipeline)
        {
            this.FollowPathMovement = new DynamicFollowPath(this.Pipeline.Character,null)
            {
	                MaxAcceleration = 40.0f,
	                MaxSpeed = 40.0f
	        };
        }

        public override Path GetPath(SteeringGoal goal)
        {
            return new LineSegmentPath(this.Pipeline.Character.position,goal.Position);
        }

        public override Path GetPath(GlobalPath path)
        {
            return path;
        }

        public override MovementOutput GetSteering(Path path)
        {
            if (this.PreviousPath == null || this.PreviousPath != path)
            {
                this.FollowPathMovement.Path = path;
                this.FollowPathMovement.CurrentParam = 0;
                this.PreviousPath = path;
            }
            
            return this.FollowPathMovement.GetMovement();
        }
    }
}
