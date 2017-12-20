using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.TacticalAnalysis
{
    public interface IInfluenceFunction
    {
        float DetermineInfluence(IInfluenceUnit unit, Vector3 location);
    }
}
