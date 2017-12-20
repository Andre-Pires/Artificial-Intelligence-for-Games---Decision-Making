using System;
namespace Assets.Scripts.IAJ.Unity.Movement.Arbitration.SteeringPipeline.Components.Targeters
{
    public class FixedTargeter : TargeterComponent
    {
        public SteeringGoal Target { get; set; }

        public FixedTargeter(SteeringPipeline pipeline) : base(pipeline)
        {
            this.Target = new SteeringGoal();
        }

        public override SteeringGoal GetGoal()
        {
            return this.Target;
        }
    }
}
