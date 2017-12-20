using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicArrive : DynamicSeek
    {
        public float MaxSpeed { get; set; }

        public float TimeToTarget { get; set; }

        public float TargetRadius { get; set; }

        public float SlowRadius { get; set; }

        public override string Name
        {
            get { return "Arrive"; }
        }

        public DynamicArrive()
        {
            this.TimeToTarget = 1;
            this.TargetRadius = 1;
            this.SlowRadius = 10;
        }
        
        
        public override MovementOutput GetMovement()
        {
            float targetSpeed;
            MovementOutput output = new MovementOutput();

            var direction = this.Target.position - this.Character.position;
            var distance = direction.magnitude;

            if (distance < this.TargetRadius)
            {
                output.linear = Vector3.zero;
                return output;
            }

            if (distance > this.SlowRadius)
            {
                //maximum speed
                targetSpeed = this.MaxSpeed;
            }
            else
            {
                targetSpeed = this.MaxSpeed*distance/this.SlowRadius;
            }

            direction.Normalize();
            direction *= targetSpeed;

            output.linear = direction - this.Character.velocity;
            output.linear /= this.TimeToTarget;
            

            // If that is too fast, then clip the acceleration
            if (output.linear.sqrMagnitude > this.MaxAcceleration*this.MaxAcceleration)
            {
                output.linear.Normalize();
                output.linear *= this.MaxAcceleration;
            }

            return output;
        }
    }
}
