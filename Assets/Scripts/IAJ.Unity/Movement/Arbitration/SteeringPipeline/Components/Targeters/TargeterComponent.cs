namespace Assets.Scripts.IAJ.Unity.Movement.Arbitration.SteeringPipeline.Components.Targeters
{
    //A targeter tells the system where the character should be going
    public abstract class TargeterComponent : SteeringPipelineComponent
    {
        public TargeterComponent(SteeringPipeline pipeline) : base(pipeline)
        {
        }
        public abstract SteeringGoal GetGoal();
    }
}
