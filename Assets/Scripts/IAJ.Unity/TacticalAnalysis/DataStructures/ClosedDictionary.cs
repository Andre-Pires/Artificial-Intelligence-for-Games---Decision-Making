using RAIN.Navigation.Graph;
using System.Collections.Generic;

namespace Assets.Scripts.IAJ.Unity.TacticalAnalysis.DataStructures
{
    public class ClosedLocationRecordDictionary : IClosedLocationRecord
    {
        private Dictionary<NavigationGraphNode, LocationRecord> Closed { get; set; }

        public ClosedLocationRecordDictionary()
        {
            this.Closed = new Dictionary<NavigationGraphNode, LocationRecord>();
        }

        public void Initialize()
        {
            this.Closed.Clear();
        }

        public void AddToClosed(LocationRecord nodeRecord)
        {
            this.Closed.Add(nodeRecord.Location, nodeRecord);
        }

        public void RemoveFromClosed(LocationRecord nodeRecord)
        {
            this.Closed.Remove(nodeRecord.Location);
        }

        public LocationRecord SearchInClosed(LocationRecord nodeRecord)
        {
            LocationRecord record;
            if (Closed.TryGetValue(nodeRecord.Location, out record))
            {
                return record;
            }
            return null;
        }

        public ICollection<LocationRecord> All()
        {
            return this.Closed.Values;
        }
    }
}