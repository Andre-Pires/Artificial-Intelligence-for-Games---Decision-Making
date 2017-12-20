using Assets.Scripts.IAJ.Unity.TacticalAnalysis;
using RAIN.Navigation.Graph;

namespace Assets.Scripts.GameManager
{
    public class Resource : IInfluenceUnit
    {
        public NavigationGraphNode Location { get; private set; }
        public float DirectInfluence { get; private set; }

        public Resource(NavigationGraphNode location)
        {
            this.DirectInfluence = 5.0f;
            this.Location = location;
        }
    }
}
