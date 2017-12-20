using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using RAIN.Navigation.Graph;
using UnityEngine;

namespace Assets.Scripts.DecisionMakingActions
{
    public class MeleeAttack : WalkToTargetAndExecuteAction
    {
        public MeleeAttack(AutonomousCharacter character, GameObject target, NavigationGraphNode targetNode, int resourceIndex) : base("MeleeAttack", character, target, targetNode, resourceIndex)
        {
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);

            if (goal.Name == AutonomousCharacter.EAT_GOAL)
            {
                change -= 2.0f;
            }
            else if (goal.Name == AutonomousCharacter.REST_GOAL)
            {
                change += 0.5f;
            }
            else if (goal.Name == AutonomousCharacter.SURVIVE_GOAL)
            {
                change += 2.0f;
            }

            return change;
        }

        public override bool CanExecute()
        {
            if (!base.CanExecute()) return false;
            return this.Character.GameManager.characterData.HP > 2;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            if (!base.CanExecute(worldModel)) return false;
            var hp = (int)worldModel.GetProperty(Properties.HP_INDEX);
            return hp > 2;
        }

        public override void Execute()
        {
            base.Execute();
            this.Character.GameManager.MeleeAttack(this.Target);
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            var eatValue = worldModel.GetGoalValue(AutonomousCharacter.EAT_GOAL_INDEX);
            worldModel.SetGoalValue(AutonomousCharacter.EAT_GOAL_INDEX, eatValue - 2.0f);
            var restValue = worldModel.GetGoalValue(AutonomousCharacter.REST_GOAL_INDEX);
            worldModel.SetGoalValue(AutonomousCharacter.REST_GOAL_INDEX, restValue + 0.5f);
            var surviveValue = worldModel.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL_INDEX);
            worldModel.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL_INDEX, surviveValue + 2.0f);

            var hp = (int)worldModel.GetProperty(Properties.HP_INDEX);
            worldModel.SetProperty(Properties.HP_INDEX, hp - 2);
            var energy = (float)worldModel.GetProperty(Properties.ENERGY_INDEX);
            worldModel.SetProperty(Properties.ENERGY_INDEX, energy - 0.5f);
            var hunger = (float)worldModel.GetProperty(Properties.HUNGER_INDEX);
            worldModel.SetProperty(Properties.HUNGER_INDEX, hunger - 2.0f);

            //disables the target object so that it can't be reused again
            worldModel.SetResourceStatus(this.ResourceIndex, false);
        }
    }
}