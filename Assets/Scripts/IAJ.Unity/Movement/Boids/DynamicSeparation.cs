using System;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.Boids
{
    public class DynamicSeparation : DynamicMovement.DynamicMovement
    {
        public override string Name
        {
            get { return "Separation"; }
        }
        public override KinematicData Target { get; set; }
        public float FlockRadius { get; set; }
        public float SeparationFactor { get; set; }
        public Flock Flock { get; private set; }

        public DynamicSeparation(Flock flock)
        {
            this.Flock = flock;
            this.SeparationFactor = 20.0f;
            this.FlockRadius = 1.0f;
            this.MaxAcceleration = 20.0f;
        }
        public override MovementOutput GetMovement()
        {
            MovementOutput output = new MovementOutput();
            Vector3 direction;
            float strength;
            foreach (var boid in this.Flock.FlockMembers)
            {
                if (boid.KinematicData != this.Character)
                {
                    direction = this.Character.position - boid.KinematicData.position;
                    var sqrDistance = direction.sqrMagnitude;
                    if (sqrDistance < this.FlockRadius*this.FlockRadius)
                    {
                        //var angle = MathHelper.ConvertVectorToOrientation(direction*-1);
                        //if (Math.Abs((angle - this.Character.orientation)%MathConstants.MATH_PI) <= MathConstants.MATH_PI_2)
                        //{
                            strength = Math.Min(this.SeparationFactor/sqrDistance, this.MaxAcceleration);
                            direction.Normalize();
                            output.linear += strength*direction;
                        //}
                    }
                }
            }

            if (output.linear.sqrMagnitude > this.MaxAcceleration*this.MaxAcceleration)
            {
                output.linear.Normalize();
                output.linear *= this.MaxAcceleration;
            }

            return output;
        }
    }
}
