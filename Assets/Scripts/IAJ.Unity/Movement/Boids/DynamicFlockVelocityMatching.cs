using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using Assets.Scripts.IAJ.Unity.Utils;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.Boids
{
    public class DynamicFlockVelocityMatching : DynamicVelocityMatch
    {
        public override string Name
        {
            get { return "Cohesion"; }
        }
        public float FlockRadius { get; set; }
        public Flock Flock { get; protected set; }

        public DynamicFlockVelocityMatching(Flock flock)
        {
            this.Flock = flock;
            this.FlockRadius = 1.0f;
            this.MaxAcceleration = 20.0f;
            this.MovingTarget = new KinematicData();
        }
        public override MovementOutput GetMovement()
        {
            Vector3 direction;
            Vector3 averageVelocity = new Vector3();
            uint closeBoids = 0;
            foreach (var boid in this.Flock.FlockMembers)
            {
                if (boid.KinematicData != this.Character)
                {
                    direction = boid.KinematicData.position - this.Character.position;
                    if (direction.sqrMagnitude < this.FlockRadius * this.FlockRadius)
                    {
                        var angle = MathHelper.ConvertVectorToOrientation(direction);
                        var angleDifference = MathHelper.SmallestDifferenceBetweenTwoAngles(angle,this.Character.orientation);
                        if (angleDifference <= MathConstants.MATH_PI_2)
                        {
                            averageVelocity += boid.KinematicData.velocity;
                            closeBoids++;
                        }
                    }
                }
            }

            if (closeBoids != 0)
            {
                this.MovingTarget.velocity = averageVelocity/closeBoids;

                return base.GetMovement();
            }
            else return new MovementOutput();
        }
    }
}
