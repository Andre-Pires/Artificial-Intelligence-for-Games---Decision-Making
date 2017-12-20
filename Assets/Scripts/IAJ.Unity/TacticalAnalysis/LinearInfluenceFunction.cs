using System;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.TacticalAnalysis
{
    public class LinearInfluenceFunction : IInfluenceFunction
    {
        public float DetermineInfluence(IInfluenceUnit unit, Vector3 location)
        {
            return unit.DirectInfluence / (1 + (unit.Location.Position - location).magnitude);
        }
    }
}