using System;
using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using Assets.Scripts.IAJ.Unity.Utils;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.Boids
{
    public class DynamicCohesion : DynamicArrive
    {
        public override string Name
        {
            get { return "Cohesion"; }
        }
        public float FlockRadius { get; set; }

        public Flock Flock { get; set; }

        public DynamicCohesion(Flock flock)
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
            Vector3 massCenter = new Vector3();
            Vector3 direction;
            uint closeBoids = 0;
            foreach (var boid in this.Flock.FlockMembers)
            {
                if (boid.KinematicData != this.Character)
                {
                    direction = boid.KinematicData.position - this.Character.position;

                    if (direction.sqrMagnitude < this.FlockRadius * this.FlockRadius)
                    {
                        var angle = MathHelper.ConvertVectorToOrientation(direction);
                        if (Math.Abs((angle - this.Character.orientation) % MathConstants.MATH_PI) <= MathConstants.MATH_PI_2)
                        {
                            massCenter += boid.KinematicData.position;
                            closeBoids++;    
                        }
                    }
                }
            }

            if (closeBoids != 0)
            {
                massCenter /= closeBoids;
                this.Target.position = massCenter;
                return base.GetMovement();
            }

            return new MovementOutput();
        }
    }
}
