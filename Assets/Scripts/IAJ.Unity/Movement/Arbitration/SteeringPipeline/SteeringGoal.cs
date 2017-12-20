using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.Arbitration.SteeringPipeline
{
    public class SteeringGoal
    {
        public bool PositionSet { get; private set; }

        private Vector3 position;

        public Vector3 Position
        {
            get { return this.position; }
            set
            {
                this.position = value;
                this.PositionSet = true;
            }
        }

        private float orientation;
        public bool OrientationSet { get; private set; }
        public float Orientation
        {
            get { return this.orientation; }
            set
            {
                this.orientation = value;
                this.OrientationSet = true;
            }
        }

        private Vector3 velocity;
        public bool VelocitySet { get; private set; }
        public Vector3 Velocity
        {
            get { return this.velocity; }
            set
            {
                this.velocity = value;
                this.VelocitySet = true;
            }
        }

        private float rotation;
        public bool RotationSet { get; private set; }
        public float Rotation
        {
            get { return this.rotation; }
            set
            {
                this.rotation = value;
                this.RotationSet = true;
            }
        }

        public void Clear()
        {
            this.PositionSet = false;
            this.OrientationSet = false;
            this.VelocitySet = false;
            this.RotationSet = false;
        }

        public void UpdateGoal(SteeringGoal goal)
        {
            if (this.CanMergeGoals(goal))
            {
                if (goal.PositionSet)
                {
                    this.Position = goal.Position;
                }
                if (goal.OrientationSet)
                {
                    this.Orientation = goal.Orientation;
                }
                if (goal.VelocitySet)
                {
                    this.Velocity = goal.Velocity;
                }
                if (goal.RotationSet)
                {
                    this.Rotation = goal.Rotation;
                }
            }
        }

        public bool CanMergeGoals(SteeringGoal goal)
        {
            return !(this.PositionSet && goal.PositionSet ||
                     this.OrientationSet && goal.OrientationSet ||
                     this.VelocitySet && goal.VelocitySet ||
                     this.RotationSet && goal.RotationSet);
        }

        public KinematicData ToKinematicData()
        {
            return new KinematicData(this.Position,this.Velocity,this.Orientation,this.Rotation);
        }
    }
}
