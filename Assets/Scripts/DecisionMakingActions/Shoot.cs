using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using System;
using RAIN.Navigation.Graph;
using UnityEngine;

namespace Assets.Scripts.DecisionMakingActions
{
    public class Shoot : WalkToTargetAndExecuteAction
    {
        public Shoot(AutonomousCharacter character, GameObject target, NavigationGraphNode targetNode, int resourceIndex) : base("Shoot", character, target, targetNode, resourceIndex)
        {
        }

        public override float GetGoalChange(Goal goal)
        {
            if (goal.Name == AutonomousCharacter.EAT_GOAL)
            {
                return -2.0f;
            }

            return 0.0f;
        }

        public override bool CanExecute()
        {
            if (!base.CanExecute()) return false;
            return this.Character.GameManager.characterData.Arrows > 0;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            if (!base.CanExecute(worldModel)) return false;
            int arrows = (int)(worldModel.GetProperty(Properties.ARROWS_INDEX));

            return arrows > 0;
        }

        public override void Execute()
        {
            base.Execute();
            this.Character.GameManager.Shoot(this.Target);
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            float eatValue = worldModel.GetGoalValue(AutonomousCharacter.EAT_GOAL_INDEX);
            worldModel.SetGoalValue(AutonomousCharacter.EAT_GOAL_INDEX, eatValue - 2.0f);

            float hunger = (float)worldModel.GetProperty(Properties.HUNGER_INDEX);
            worldModel.SetProperty(Properties.HUNGER_INDEX, hunger - 2.0f);

            int arrows = (int)worldModel.GetProperty(Properties.ARROWS_INDEX);
            worldModel.SetProperty(Properties.ARROWS_INDEX, arrows - 1);

            //disables the target object so that it can't be reused again
            worldModel.SetResourceStatus(this.ResourceIndex, false);
        }
    }
}