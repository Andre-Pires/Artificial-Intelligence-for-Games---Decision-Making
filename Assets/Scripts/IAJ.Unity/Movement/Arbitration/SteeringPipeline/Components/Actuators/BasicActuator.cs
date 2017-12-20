using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using Assets.Scripts.IAJ.Unity.Pathfinding.Path;

namespace Assets.Scripts.IAJ.Unity.Movement.Arbitration.SteeringPipeline.Components.Actuators 
{
    public class BasicActuator : ActuatorComponent
    {
        public BasicActuator(SteeringPipeline pipeline) : base(pipeline)
        {
        }
        public DynamicSeek Seek { get; set; }

        public override Path GetPath(SteeringGoal goal)
        {
            return new LineSegmentPath(this.Pipeline.Character.position, goal.Position);
        }

        public override Path GetPath(GlobalPath path)
        {
            return path.LocalPaths[0];
        }

        public override MovementOutput GetSteering(Path path)
        {
            var target = path.GetPosition(1.0f);
            
            this.Seek.Character = this.Pipeline.Character;
            this.Seek.Target = new KinematicData(new StaticData(target));
            this.Seek.MaxAcceleration = this.Pipeline.MaxAcceleration;

            return this.Seek.GetMovement();
        }
    }
}
