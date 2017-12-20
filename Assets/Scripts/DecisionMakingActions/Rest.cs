using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using System;
using UnityEngine;
using Action = Assets.Scripts.IAJ.Unity.DecisionMaking.GOB.Action;

namespace Assets.Scripts.DecisionMakingActions
{
    public class Rest : Action
    {
        private AutonomousCharacter Character { get; set; }

        public Rest(AutonomousCharacter character) : base("Rest")
        {
            this.Character = character;
            this.Duration = 0.5f;
        }

        public override float GetGoalChange(Goal goal)
        {
            if (goal.Name == AutonomousCharacter.REST_GOAL) return -1.0f;

            return 0.0f;
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
            Character.Character.KinematicData.velocity *= 0.1f;
            this.Character.GameManager.Rest();
        }
    }
}