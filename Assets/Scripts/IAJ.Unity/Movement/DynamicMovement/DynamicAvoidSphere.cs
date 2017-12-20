using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicAvoidSphere : DynamicSeek
    {
        public override string Name
        {
            get { return "Avoid Sphere"; }
        }

        public GameObject Obstacle { get; set; }
        public float MaxLookAhead { get; set; }

        public float AvoidMargin { get; set; }

        public DynamicAvoidSphere(GameObject obstacle, KinematicData target)
        {
            this.Obstacle = obstacle;
            this.Target = target;
        }

        public override MovementOutput GetMovement()
        {
            if (this.Character.velocity.sqrMagnitude > 0)
            {
                var normalizedVelocity = this.Character.velocity.normalized;
                var selfToObstacle = this.Obstacle.gameObject.transform.position - this.Character.position;

                var dotProduct = Vector3.Dot(selfToObstacle,normalizedVelocity);
                var distanceSquared = selfToObstacle.sqrMagnitude - dotProduct*dotProduct;

                float radius = this.Obstacle.gameObject.transform.localScale.x/2 + this.AvoidMargin;

                if (distanceSquared < radius*radius)
                {
                    if (dotProduct > 0 && dotProduct < MaxLookAhead)
                    {
                        var closestPoint = this.Character.position + normalizedVelocity*dotProduct;

                        this.Target.position = this.Obstacle.gameObject.transform.position +
                                                         (closestPoint - this.Obstacle.gameObject.transform.position).normalized*radius;

                        return base.GetMovement();
                    }
                }
            }

            return new MovementOutput();
        }
    }
}
