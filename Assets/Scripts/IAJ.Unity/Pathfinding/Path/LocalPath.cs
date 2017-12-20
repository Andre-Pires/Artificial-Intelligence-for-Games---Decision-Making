using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.Path
{
    public abstract class LocalPath : Path
    {
        protected Vector3 StartPosition { get; set; }
        protected Vector3 EndPosition { get; set; }
    }
}
