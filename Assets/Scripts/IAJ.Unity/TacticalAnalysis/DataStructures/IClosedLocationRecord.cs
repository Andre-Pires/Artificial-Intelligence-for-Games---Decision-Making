using System.Collections.Generic;

namespace Assets.Scripts.IAJ.Unity.TacticalAnalysis.DataStructures
{
    public interface IClosedLocationRecord
    {
        void Initialize();
        void AddToClosed(LocationRecord nodeRecord);
        void RemoveFromClosed(LocationRecord nodeRecord);
        //should return null if the node is not found
        LocationRecord SearchInClosed(LocationRecord nodeRecord);
        ICollection<LocationRecord> All();
    }
}
