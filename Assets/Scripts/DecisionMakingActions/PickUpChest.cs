using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using System;
using RAIN.Navigation.Graph;
using UnityEngine;

namespace Assets.Scripts.DecisionMakingActions
{
    public class PickUpChest : WalkToTargetAndExecuteAction
    {
        public PickUpChest(AutonomousCharacter character, GameObject target, NavigationGraphNode targetNode, int resourceIndex) : base("PickUpChest", character, target, targetNode, resourceIndex)
        {
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);

            if (goal.Name == AutonomousCharacter.GET_RICH_GOAL)
                return -1.0f;

            return 0;
        }

        public override bool CanExecute()
        {
            if (!base.CanExecute()) return false;
            return this.Character.GameManager.characterData.Energy > 4.0f
                && this.Character.GameManager.characterData.Hunger < 6.0f;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            if (!base.CanExecute(worldModel)) return false;
            var hunger = (float)worldModel.GetProperty(Properties.HUNGER_INDEX);
            var energy = (float)worldModel.GetProperty(Properties.ENERGY_INDEX);

            return hunger < 6.0f && energy > 4.0f;
        }

        public override void Execute()
        {
            base.Execute();
            this.Character.GameManager.PickUpChest(this.Target);
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            var richValue = worldModel.GetGoalValue(AutonomousCharacter.GET_RICH_GOAL_INDEX);
            worldModel.SetGoalValue(AutonomousCharacter.GET_RICH_GOAL_INDEX, richValue - 1.0f);

            var money = (int)worldModel.GetProperty(Properties.MONEY_INDEX);
            worldModel.SetProperty(Properties.MONEY_INDEX, money + 1);

            //disables the target object so that it can't be reused again
            worldModel.SetResourceStatus(this.ResourceIndex, false);
        }
    }
}