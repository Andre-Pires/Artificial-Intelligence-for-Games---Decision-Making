using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using RAIN.Navigation.Graph;
using System;
using UnityEngine;

namespace Assets.Scripts.DecisionMakingActions
{
    public class GetArrows : WalkToTargetAndExecuteAction
    {
        public GetArrows(AutonomousCharacter character, GameObject target, NavigationGraphNode targetNode, int resourceIndex) : base("GetArrows", character, target, targetNode, resourceIndex)
        {
        }

        public override bool CanExecute()
        {
            if (!base.CanExecute()) return false;

            return this.Character.GameManager.characterData.Arrows < 10;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            if (!base.CanExecute(worldModel)) return false;
            var arrows = (int)(worldModel.GetProperty(Properties.ARROWS_INDEX));
            return arrows < 10;
        }

        public override void Execute()
        {
            base.Execute();
            this.Character.GameManager.GetArrows(this.Target);
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            float surviveValue = worldModel.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL_INDEX);
            worldModel.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL_INDEX, surviveValue - 1.0f);

            worldModel.SetProperty(Properties.ARROWS_INDEX, 10);
        }
    }
}