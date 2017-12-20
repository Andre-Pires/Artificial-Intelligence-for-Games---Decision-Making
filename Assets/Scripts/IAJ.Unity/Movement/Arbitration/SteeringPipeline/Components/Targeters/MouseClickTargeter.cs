using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.Arbitration.SteeringPipeline.Components.Targeters
{
    public class MouseClickTargeter : TargeterComponent
    {
        protected Camera Camera { get; set; }
        protected SteeringGoal Goal { get; set; }

        public MouseClickTargeter(SteeringPipeline pipeline, Camera camera) : base(pipeline)
        {
            this.Camera = camera;
            this.Goal = new SteeringGoal();
        }

        public override SteeringGoal GetGoal()
        {
            Vector3 position;

            if (Input.GetMouseButtonDown(0))
            {
                //if there is a valid position
                if (this.MouseClickPosition(out position))
                {
                    this.Goal.Position = position;
                }
            }

            return this.Goal;
        }

        private bool MouseClickPosition(out Vector3 position)
        {
            RaycastHit hit;

            var ray = this.Camera.ScreenPointToRay(Input.mousePosition);
            //test intersection with objects in the scene
            if (Physics.Raycast(ray, out hit))
            {
                //if there is a collision, we will get the collision point
                position = hit.point;
                return true;
            }

            position = Vector3.zero;
            //if not the point is not valid
            return false;
        }
    }
}
