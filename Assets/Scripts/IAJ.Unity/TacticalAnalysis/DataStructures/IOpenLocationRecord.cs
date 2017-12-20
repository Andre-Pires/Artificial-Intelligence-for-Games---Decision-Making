using System.Collections.Generic;

namespace Assets.Scripts.IAJ.Unity.TacticalAnalysis.DataStructures
{
    public interface IOpenLocationRecord
    {
        void Initialize();
        void Replace(LocationRecord nodeToBeReplaced, LocationRecord nodeToReplace);
        LocationRecord GetBestAndRemove();
        LocationRecord PeekBest();
        void AddToOpen(LocationRecord nodeRecord);
        void RemoveFromOpen(LocationRecord nodeRecord);
        //should return null if the node is not found
        LocationRecord SearchInOpen(LocationRecord nodeRecord);
        ICollection<LocationRecord> All();
        int CountOpen();
    }
}
