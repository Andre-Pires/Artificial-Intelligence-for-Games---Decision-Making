using Assets.Scripts.IAJ.Unity.Utils;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicWander2 : DynamicSeek
    {
        public DynamicWander2(float wanderOffSet, float wanderRadius, float turnAngle, float maxAcceleration)
        {
            this.Target = new KinematicData();
            this.WanderOffset = wanderOffSet;
            this.WanderRadius = wanderRadius;
            this.TurnAngle = turnAngle;
            this.MaxAcceleration = maxAcceleration;
            this.WanderOrientation = 0;
        }
        public override string Name
        {
            get { return "Wander"; }
        }
        public float TurnAngle { get; private set; }

        public float WanderOffset { get; private set; }
        public float WanderRadius { get; private set; }

        protected float WanderOrientation { get; set; }

        public override MovementOutput GetMovement()
        {
            this.WanderOrientation += RandomHelper.RandomBinomial(this.TurnAngle);

            var targetOrientation = this.Character.orientation + this.WanderOrientation;
            var targetOrientationVector = MathHelper.ConvertOrientationToVector(targetOrientation);

            this.Target.position = this.Character.position +
                                             this.WanderOffset*Character.GetOrientationAsVector() +
                                             this.WanderRadius*targetOrientationVector;

            return base.GetMovement();
        }
    }
}
