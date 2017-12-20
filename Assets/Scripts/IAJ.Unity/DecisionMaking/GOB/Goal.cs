namespace Assets.Scripts.IAJ.Unity.DecisionMaking.GOB
{
    public class Goal
    {
        public string Name { get; private set; }
        public float InsistenceValue { get; set; }
        public float ChangeRate { get; set; }
        public float Weight { get; private set; }

        public Goal(string name, float weight)
        {
            this.Name = name;
            this.Weight = weight;
        }

        public override bool Equals(object obj)
        {
            var goal = obj as Goal;
            if (goal == null) return false;
            else return this.Name.Equals(goal.Name);
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }

        public float GetDiscontentment(float goalValue)
        {
            return this.Weight*goalValue*goalValue;
        }
    }
}
