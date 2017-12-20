using Assets.Scripts.IAJ.Unity.Pathfinding.Path;

namespace Assets.Scripts.IAJ.Unity.Movement.Arbitration.SteeringPipeline.Components.Actuators
{
    //An actuator turns a goal into a Path: taking the character's capabilities into account.
    public abstract class ActuatorComponent : SteeringPipelineComponent
    {
        protected ActuatorComponent(SteeringPipeline pipeline) : base(pipeline)
        {
        }
        public abstract Path GetPath(SteeringGoal goal);

        public abstract Path GetPath(GlobalPath path);

        public abstract MovementOutput GetSteering(Path path);

    }
}
