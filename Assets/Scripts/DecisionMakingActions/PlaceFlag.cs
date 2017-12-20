using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using System;
using UnityEngine;
using Action = Assets.Scripts.IAJ.Unity.DecisionMaking.GOB.Action;

namespace Assets.Scripts.DecisionMakingActions
{
    public class PlaceFlag : Action
    {
        protected AutonomousCharacter Character { get; set; }

        public PlaceFlag(AutonomousCharacter character) : base("PlaceFlag")
        {
            this.Character = character;
        }

        public override float GetDuration()
        {
            return (this.Character.BestFlagPosition - this.Character.Character.KinematicData.position).magnitude / 20.0f;
        }

        public override float GetDuration(WorldModel worldModel)
        {
            var position = (Vector3)worldModel.GetProperty(Properties.POSITION_INDEX);
            return (this.Character.BestFlagPosition - position).magnitude / 20.0f;
        }

        public override float GetGoalChange(Goal goal)
        {
            var change = base.GetGoalChange(goal);
            if (goal.Name == AutonomousCharacter.REST_GOAL)
            {
                var distance = (this.Character.BestFlagPosition - this.Character.Character.KinematicData.position).magnitude;
                //+0.01 * distance because of the walk
                return change + distance * 0.01f;
            }
            else if (goal.Name == AutonomousCharacter.CONQUER_GOAL)
            {
                return change - 1.5f;
            }
            else return 0;
        }

        public override bool CanExecute()
        {
            if (!base.CanExecute()) return false;

            return this.Character.BestFlagPosition != Vector3.zero
                && this.Character.GameManager.characterData.Energy > 2.0f
                && this.Character.GameManager.characterData.Hunger < 8.0f;
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            if (!base.CanExecute(worldModel)) return false;
            var hunger = (float)worldModel.GetProperty(Properties.HUNGER_INDEX);
            var energy = (float)worldModel.GetProperty(Properties.ENERGY_INDEX);

            return this.Character.BestFlagPosition != Vector3.zero
                && energy > 2.0f
                && hunger < 8.0f;
        }

        public override void Execute()
        {
            base.Execute();
            Character.Targeter.Target.Position = Character.BestFlagPosition;
            this.Character.GameManager.PlaceFlag(Character.BestFlagPosition);
        }

        public override void ApplyActionEffects(WorldModel worldModel)
        {
            base.ApplyActionEffects(worldModel);

            float conquerValue = worldModel.GetGoalValue(AutonomousCharacter.CONQUER_GOAL_INDEX);
            worldModel.SetGoalValue(AutonomousCharacter.CONQUER_GOAL_INDEX, conquerValue - 1.5f);
        }
    }
}