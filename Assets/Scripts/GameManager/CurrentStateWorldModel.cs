using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.GameManager;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.GOB
{
    //class that represents a world model that corresponds to the current state of the world,
    //all required properties and goals are stored inside the game manager
    public class CurrentStateWorldModel : WorldModel
    {
        private GameManager.GameManager GameManager { get; set; }
        private List<Goal> Goals { get; set; } 
        public CurrentStateWorldModel(GameManager.GameManager gameManager, List<Action> actions, List<Goal> goals, int numResources) : base(actions, goals, numResources)
        {
            this.GameManager = gameManager;
            this.Goals = goals;
        }

        public void Initialize()
        {
            this.ActionEnumerator.Reset();
        }

        public override object GetProperty(int propertyIndex)
        {
            if (propertyIndex == Properties.ARROWS_INDEX) return this.GameManager.characterData.Arrows;

            if (propertyIndex == Properties.ENERGY_INDEX) return this.GameManager.characterData.Energy;

            if (propertyIndex == Properties.HP_INDEX) return this.GameManager.characterData.HP;

            if (propertyIndex == Properties.MONEY_INDEX) return this.GameManager.characterData.Money;

            if (propertyIndex == Properties.HUNGER_INDEX) return this.GameManager.characterData.Hunger;

            if (propertyIndex == Properties.POSITION_INDEX)
                return this.GameManager.characterData.CharacterGameObject.transform.position;

            return true;
        }

        public override float GetGoalValue(int goalIndex)
        {
            return this.Goals[goalIndex].InsistenceValue;
        }

        public override void SetGoalValue(int goalIndex, float goalValue)
        {
            //this method does nothing, because you should not directly set a goal value of the CurrentStateWorldModel
        }

        public override void SetProperty(int propertyIndex, object value)
        {
            //this method does nothing, because you should not directly set a property of the CurrentStateWorldModel
        }

        public override bool GetResourceStatus(int resourceIndex)
        {
            return true;
        }

        public override void SetResourceStatus(int resourceIndex, bool status)
        {
            //this method does nothing, because you should not directly set a property of the CurrentStateWorldModel
        }
    }
}
