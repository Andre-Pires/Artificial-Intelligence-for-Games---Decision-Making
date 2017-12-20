using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using RAIN.Navigation.Graph;
using UnityEngine;

namespace Assets.Scripts.DecisionMakingActions
{
    public class Sleep : WalkToTargetAndExecuteAction
    {
        public Sleep(AutonomousCharacter character, GameObject target, NavigationGraphNode targetNode, int resourceIndex) : base("Sleep", character, target, targetNode, resourceIndex)
        {
        }

        public override float GetGoalChange(Goal goal)
        {
            float distance = (this.Target.transform.position - Character.transform.position).magnitude / 20.0f;

            var change = base.GetGoalChange(goal);
            if (goal.Name == AutonomousCharacter.REST_GOAL) change -= 1.0f;

            return change;
        }

        public override bool CanExecute()
        {
            if (!base.CanExecute()) return false;
            return this.Character.GameManager.characterData.Energy < 7.0f;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            if (!base.CanExecute(worldModel)) return false;

            var energy = (float)worldModel.GetProperty(Properties.ENERGY_INDEX);
            return energy < 7.0f;
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            var restValue = worldModel.GetGoalValue(AutonomousCharacter.REST_GOAL_INDEX);
            worldModel.SetGoalValue(AutonomousCharacter.REST_GOAL_INDEX, restValue - 1.0f);

            var energy = (float)worldModel.GetProperty(Properties.ENERGY_INDEX);
            worldModel.SetProperty(Properties.ENERGY_INDEX, energy + 1.0f);
        }

        public override void Execute()
        {
            base.Execute();
            this.Character.Targeter.Target.Position = this.Target.transform.position;
            this.Character.GameManager.Sleep(this.Target);
        }
    }
}