using Assets.Scripts.IAJ.Unity.TacticalAnalysis;
using RAIN.Navigation.Graph;

namespace Assets.Scripts.GameManager
{
    public enum FlagColor
    {
        Green,
        Red
    };

    public class Flag : IInfluenceUnit
    {
        public FlagColor FlagColor { get; private set; }

        public NavigationGraphNode Location { get; private set; }
        public float DirectInfluence { get; private set; }

        public Flag(NavigationGraphNode location, FlagColor color)
        {
            this.DirectInfluence = 10.0f;
            this.Location = location;
            this.FlagColor = color;
        }
    }
}
