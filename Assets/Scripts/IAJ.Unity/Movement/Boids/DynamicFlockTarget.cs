using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;

namespace Assets.Scripts.IAJ.Unity.Movement.Boids
{
    public class DynamicFlockTarget : DynamicArrive
    {
        public override string Name
        {
            get { return "FlockSeek"; }
        }
        public float FlockRadius { get; set; }

        public Flock Flock { get; set; }

        public DynamicFlockTarget(Flock flock)
        {
            this.Flock = flock;
            this.FlockRadius = 1.0f;
            this.MaxAcceleration = 20.0f;
            this.MaxSpeed = 20.0f;
            this.Target = new KinematicData();
            this.TimeToTarget = 0.5f;
            this.SlowRadius = 10.0f;
            this.TargetRadius = 2.0f;
        }
        public override MovementOutput GetMovement()
        {
            if (this.Flock.Target != null)
            {
                this.Target = this.Flock.Target;
                return base.GetMovement();
            }
            else
            {
                return new MovementOutput();
            }
        }
    }
}
