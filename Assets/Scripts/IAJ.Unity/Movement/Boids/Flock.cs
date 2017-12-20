using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.IAJ.Unity.Movement.Arbitration;
using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.Boids
{
    public class Flock
    {
        public List<DynamicCharacter> FlockMembers { get; protected set; }
        public float MaximumSpeed { get; set; }
        public float MaximumAcceleration { get; set; }
        public float Drag { get; set; }
        public float FlockRadius { get; set; }
        public float SeparationFactor { get; set; }
        public GameObject[] Obstacles { get; set; }
        public KinematicData Target { get; set; }
            
        public Flock()
        {
            this.FlockMembers = new List<DynamicCharacter>();
            this.MaximumSpeed = 20.0f;
            this.MaximumAcceleration = 40.0f;
            this.Drag = 0.9f;
            this.FlockRadius = 50.0f;
            this.SeparationFactor = 40.0f;
        }

        public void AddFlockMember(GameObject gameObject)
        {
            var member = new DynamicCharacter(gameObject)
            {
                Drag = this.Drag,
                MaxSpeed = this.MaximumSpeed
            };

            member.KinematicData.velocity = new Vector3(Random.Range(0, this.MaximumSpeed), 0,Random.Range(0, this.MaximumSpeed));
            

            member.Movement = this.GenerateBlendingMovementFor(member);
            this.FlockMembers.Add(member);
        }

        private BlendedMovement GenerateBlendingMovementFor(DynamicCharacter character)
        {
            var blending = new BlendedMovement
            {
                MaxAcceleration = this.MaximumAcceleration,
                Character = character.KinematicData
            };

            var cohesion = new DynamicCohesion(this)
            {
                Character = character.KinematicData,
                MaxAcceleration = this.MaximumAcceleration,
                MaxSpeed = this.MaximumSpeed,
                FlockRadius = this.FlockRadius,
                MovementDebugColor = Color.yellow
            };

            var separation = new DynamicSeparation(this)
            {
                Character = character.KinematicData,
                MaxAcceleration = this.MaximumAcceleration,
                FlockRadius = this.FlockRadius,
                SeparationFactor = this.SeparationFactor,
                MovementDebugColor = Color.red
            };

            var velocityMatch = new DynamicFlockVelocityMatching(this)
            {
                Character = character.KinematicData,
                MaxAcceleration = this.MaximumAcceleration,
                FlockRadius = this.FlockRadius,
                MovementDebugColor = Color.green
            };

            var collisionDetection = new PriorityMovement
            {
                Character = character.KinematicData,
                MovementDebugColor = Color.magenta
            };
            foreach (var obstacle in this.Obstacles)
            {
                var avoidMovement = new DynamicAvoidObstacle(obstacle)
                {
	                MaxAcceleration = this.MaximumAcceleration,
	                AvoidMargin = 4.0f,
	                MaxLookAhead = 10.0f,
	            };
                
                collisionDetection.Movements.Add(avoidMovement);
            }

            var flockSeek = new DynamicFlockTarget(this)
            {
                Character = character.KinematicData,
                MaxAcceleration = this.MaximumAcceleration,
                MovementDebugColor = Color.cyan
            };

            blending.Movements.Add(new MovementWithWeight(cohesion));
            blending.Movements.Add(new MovementWithWeight(separation,2.0f));
            blending.Movements.Add(new MovementWithWeight(velocityMatch));
            blending.Movements.Add(new MovementWithWeight(collisionDetection,1000.0f));
            blending.Movements.Add(new MovementWithWeight(flockSeek));

            return blending;
        }

        public void Update()
        {
            foreach (var member in this.FlockMembers)
            {
                member.Update();
            }

            if (this.Target != null)
            {
                var direction = this.Target.position - this.FlockMembers.FirstOrDefault().KinematicData.position;
                if (direction.sqrMagnitude < 16)
                {
                    this.Target = null;
                }
            }

        }
    }
}
