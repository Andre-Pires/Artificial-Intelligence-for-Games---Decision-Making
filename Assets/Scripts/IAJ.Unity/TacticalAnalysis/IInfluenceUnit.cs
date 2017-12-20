using RAIN.Navigation.Graph;

namespace Assets.Scripts.IAJ.Unity.TacticalAnalysis
{
    public interface IInfluenceUnit
    {
        NavigationGraphNode Location { get; }
        float DirectInfluence { get; }
    }
}
