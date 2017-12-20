using Assets.Scripts.IAJ.Unity.Utils;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicAvoidObstacle : DynamicSeek
    {
        public override string Name
        {
            get { return "Avoid Obstacle"; }
        }

        private GameObject obstacle;

        public GameObject Obstacle
        {
            get { return this.obstacle; }
            set
            {
                this.obstacle = value;
                this.ObstacleCollider = value.GetComponent<Collider>();
            }
        }

        private Collider ObstacleCollider { get; set; }
        public float MaxLookAhead { get; set; }

        public float AvoidMargin { get; set; }

        public float FanAngle { get; set; }

        public DynamicAvoidObstacle(GameObject obstacle)
        {
            this.Obstacle = obstacle;
            this.Target = new KinematicData();
            this.MaxLookAhead = 5.0f;
            this.AvoidMargin = 1.0f;
            this.FanAngle = MathConstants.MATH_PI_4;
        }

        public override MovementOutput GetMovement()
        {
            RaycastHit hit;
            bool collision = false;
            //small whiskers, 30% of the central ray size
            var color0 = Color.black;
            var color1 = Color.black;
            var color2 = Color.black;

            var whisker1 = MathHelper.Rotate2D(this.Character.velocity, this.FanAngle).normalized;
            var whisker2 = MathHelper.Rotate2D(this.Character.velocity, -this.FanAngle).normalized;
            var normalizedVelocity = this.Character.velocity;
            normalizedVelocity.Normalize();

            if (this.ObstacleCollider.Raycast(new Ray(this.Character.position, normalizedVelocity), out hit, this.MaxLookAhead))
            {
                this.Target.position = hit.point + hit.normal*this.AvoidMargin;
                color0 = Color.red;
                collision = true;
            }
            else if (this.ObstacleCollider.Raycast(new Ray(this.Character.position, whisker1), out hit, this.MaxLookAhead*0.3f))
            {
                this.Target.position = hit.point + hit.normal*this.AvoidMargin;
                color1 = Color.red;
                collision = true;
            }
            else if (this.ObstacleCollider.Raycast(new Ray(this.Character.position, whisker2), out hit, this.MaxLookAhead * 0.3f))
            {
                this.Target.position = hit.point + hit.normal * this.AvoidMargin;
                color2 = Color.red;
                collision = true;
            }

            Debug.DrawRay(this.Character.position, this.Character.velocity.normalized*this.MaxLookAhead, color0);
            Debug.DrawRay(this.Character.position, whisker1*this.MaxLookAhead*0.3f, color1);
            Debug.DrawRay(this.Character.position, whisker2*this.MaxLookAhead*0.3f, color2);

            if (collision)
            {
                return base.GetMovement();
            }

            return new MovementOutput();
        }
    }
}
