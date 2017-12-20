using Assets.Scripts.GameManager;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.GOB
{
    public class WorldModel
    {
        public float[] GoalValuesArray { get; private set; }
        public Goal[] GoalsArray { get; private set; }

        public object[] PropertiesArray { get; private set; }

        public bool[] Resources { get; private set; }

        private List<Action> Actions { get; set; }
        protected IEnumerator<Action> ActionEnumerator { get; set; }

        public WorldModel(List<Action> actions, List<Goal> goals, int numResources)
        {
            this.GoalValuesArray = new float[goals.Count];
            this.GoalsArray = goals.ToArray();

            this.PropertiesArray = new object[6];

            this.Resources = Enumerable.Repeat(true, numResources).ToArray();

            this.Actions = actions;
            this.ActionEnumerator = actions.GetEnumerator();
        }

        public WorldModel(WorldModel parent)
        {
            this.GoalValuesArray = parent.GetAllGoalValues();
            this.GoalsArray = parent.GoalsArray;

            this.PropertiesArray = parent.GetAllProperties();

            this.Resources = (bool[])parent.Resources.Clone();

            this.Actions = parent.Actions;
            this.ActionEnumerator = this.Actions.GetEnumerator();
        }

        protected object[] GetAllProperties()
        {
            var numProperties = this.PropertiesArray.Length;
            var allPropertys = new object[numProperties];

            for (var i = 0; i < numProperties; i++)
                allPropertys[i] = GetProperty(i);

            return allPropertys;
        }

        public virtual object GetProperty(int propertyIndex)
        {
            return this.PropertiesArray[propertyIndex];
        }

        public virtual void SetProperty(int propertyIndex, object value)
        {
            this.PropertiesArray[propertyIndex] = value;
        }

        public virtual float[] GetAllGoalValues()
        {
            var numGoals = this.GoalValuesArray.Length;
            var allGoalValues = new float[numGoals];

            for (var i = 0; i < numGoals; i++)
                allGoalValues[i] = GetGoalValue(i);

            return allGoalValues;
        }

        public virtual float GetGoalValue(int goalIndex)
        {
            return this.GoalValuesArray[goalIndex];
        }

        public virtual void SetGoalValue(int goalIndex, float value)
        {
            var limitedValue = value;
            if (value > 10.0f)
            {
                limitedValue = 10.0f;
            }
            else if (value < 0.0f)
            {
                limitedValue = 0.0f;
            }

            this.GoalValuesArray[goalIndex] = limitedValue;
        }

        public virtual bool GetResourceStatus(int resourceIndex)
        {
            return this.Resources[resourceIndex];
        }

        public virtual void SetResourceStatus(int resourceIndex, bool status)
        {
            this.Resources[resourceIndex] = status;
        }

        public WorldModel GenerateChildWorldModel()
        {
            return new WorldModel(this);
        }

        public float CalculateDiscontentment(List<Goal> goals)
        {
            var discontentment = 0.0f;

            // Time: 1.20 ms
            var i = 0;
            foreach (var goalValue in this.GoalValuesArray)
            {
                var goal = this.GoalsArray[i++];
                discontentment += goal.GetDiscontentment(goalValue);
            }

            return discontentment;
        }

        public Action GetNextAction()
        {
            Action action = null;
            //returns the next action that can be executed or null if no more executable actions exist
            if (this.ActionEnumerator.MoveNext())
            {
                action = this.ActionEnumerator.Current;
            }

            while (action != null && !action.CanExecute(this))
            {
                if (this.ActionEnumerator.MoveNext())
                {
                    action = this.ActionEnumerator.Current;
                }
                else
                {
                    action = null;
                }
            }

            return action;
        }
    }
}