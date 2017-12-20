using Assets.Scripts.IAJ.Unity.Pathfinding.Path;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicFollowPath : DynamicArrive
    {
        public Path Path { get; set; }
        public float PathOffset { get; set; }

        public float CurrentParam { get; set; }

        private DynamicSeek Seek { get; set; }

        private MovementOutput EmptyMovementOutput { get; set; }


        public DynamicFollowPath(KinematicData character, Path path) 
        {

            //arrive properties
            this.SlowRadius = 5.0f;
            this.TimeToTarget = 0.5f;
            this.TargetRadius = 1.0f;

            this.MaxAcceleration = 40.0f;

            this.Target = new KinematicData();
            this.Character = character;
            this.Path = path;
            this.CurrentParam = 0.0f;
            this.PathOffset = 0.5f;
            this.EmptyMovementOutput = new MovementOutput();
        }

        public override MovementOutput GetMovement()
        {
            if (this.Path == null)
            {
                return this.EmptyMovementOutput;
            }

            this.CurrentParam = this.Path.GetParam(this.Character.position, this.CurrentParam);

            if (this.Path.PathEnd(this.CurrentParam))
            {
                return base.GetMovement();
            }

            var targetParam = this.CurrentParam + PathOffset;

            Target.position = this.Path.GetPosition(targetParam);

            return base.GetMovement();
        }
    }
}
