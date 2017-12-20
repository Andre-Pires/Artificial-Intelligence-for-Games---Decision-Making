using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicAvoidCharacter : DynamicMovement
    {
        public override string Name
        {
            get { return "Avoid Character"; }
        }

        public override KinematicData Target { get; set; }

        public float MaxTimeLookAhead { get; set; }

        public float AvoidMargin { get; set; }


        public DynamicAvoidCharacter(KinematicData target)
        {
            this.Target = target;
            this.MaxTimeLookAhead = 1.0f;
            this.AvoidMargin = 1.0f;
            
        }

        public override MovementOutput GetMovement()
        {
            var output = new MovementOutput();

            var deltaPos = this.Target.position - this.Character.position;
            var distance = deltaPos.magnitude;

            var deltaVel = this.Target.velocity - this.Character.velocity;
            var deltaSpeed = deltaVel.magnitude;

            if (!(deltaSpeed > 0)) return output;

            var timeToClosest = - Vector3.Dot(deltaPos,deltaVel)/(deltaSpeed*deltaSpeed);

            if (timeToClosest > this.MaxTimeLookAhead) return output;

            var minSeparation = distance - deltaSpeed*timeToClosest;

            if(minSeparation > 2*this.AvoidMargin) return output;

            if (minSeparation <= 0 || distance < 2*this.AvoidMargin)
            {
                output.linear = this.Character.position - this.Target.position;
            }
            else
            {
                output.linear = (deltaPos + deltaVel*timeToClosest)*-1;
            }

            output.linear.Normalize();
            output.linear *= this.MaxAcceleration;

            return output;
        }
    }
}
