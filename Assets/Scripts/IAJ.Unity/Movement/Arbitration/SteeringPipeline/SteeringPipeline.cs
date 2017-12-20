using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity.Movement.Arbitration.SteeringPipeline.Components.Actuators;
using Assets.Scripts.IAJ.Unity.Movement.Arbitration.SteeringPipeline.Components.Constraints;
using Assets.Scripts.IAJ.Unity.Movement.Arbitration.SteeringPipeline.Components.Decomposers;
using Assets.Scripts.IAJ.Unity.Movement.Arbitration.SteeringPipeline.Components.Targeters;
using Assets.Scripts.IAJ.Unity.Pathfinding.Path;

namespace Assets.Scripts.IAJ.Unity.Movement.Arbitration.SteeringPipeline
{
    public class SteeringPipeline : DynamicMovement.DynamicMovement
    {
        public ActuatorComponent Actuator { get; set; }
        public List<TargeterComponent> Targeters { get; protected set; }
        public List<DecomposerComponent> Decomposers { get; protected set; }
        public List<ConstraintComponent> Constraints { get; protected set; }

        public uint MaxConstraintSteps { get; set; }

        public DynamicMovement.DynamicMovement FallBackMovement { get; set; }

        public Path Path { get; protected set; }

        public SteeringGoal Goal { get; set; }

        public override KinematicData Target { get; set; }

        public override string Name
        {
            get { return "Pipeline"; }
        }

        public SteeringPipeline()
        {
            this.Targeters = new List<TargeterComponent>();
            this.Decomposers = new List<DecomposerComponent>();
            this.Constraints = new List<ConstraintComponent>();
            this.Goal = new SteeringGoal();
        }

        public override MovementOutput GetMovement()
        {

            var goal = this.Goal;
            SteeringGoal auxGoal;
            goal.Clear();

            foreach (var targeter in this.Targeters)
            {
                auxGoal = targeter.GetGoal();
                if (goal.CanMergeGoals(auxGoal))
                {
                    goal.UpdateGoal(auxGoal);
                }
            }

            //foreach (var decomposer in this.Decomposers)
            //{
            //    goal = decomposer.DecomposeGoal(goal);
            //}

            var path = this.Decomposers[0].DecomposeGoalIntoPath(goal);

            var constraintSteps = 0;
            float currentViolation, minViolation;
            ConstraintComponent violationConstraint;

            while (constraintSteps < this.MaxConstraintSteps)
            {
                this.Path = this.Actuator.GetPath(path);

                minViolation = float.PositiveInfinity;
                violationConstraint = null;

                foreach (var constraint in this.Constraints)
                {
                    currentViolation = constraint.WillViolate(this.Path, minViolation);
                    if (currentViolation > 0 && currentViolation < minViolation)
                    {
                        minViolation = currentViolation;
                        violationConstraint = constraint;
                    }
                }

                if (violationConstraint != null)
                {
                    goal = violationConstraint.Suggest(this.Path);
                    this.Path = this.Actuator.GetPath(goal);
                }
                else
                {
                     return this.Actuator.GetSteering(this.Path);
                }
                constraintSteps++;
            }

            if (this.FallBackMovement != null)
            {
                return this.FallBackMovement.GetMovement();
            }
            else return new MovementOutput();
        }
    }
}
