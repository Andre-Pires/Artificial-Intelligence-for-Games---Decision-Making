using Assets.Scripts.IAJ.Unity.Pathfinding;
using Assets.Scripts.IAJ.Unity.Utils;
using System.Collections.Generic;

namespace Assets.Scripts.IAJ.Unity.TacticalAnalysis.DataStructures
{
    public class LocationPriorityHeap : IOpenLocationRecord
    {
        private PriorityHeap<LocationRecord> OpenHeap { get; set; }

        public LocationPriorityHeap()
        {
            this.OpenHeap = new PriorityHeap<LocationRecord>();
        }

        public void Initialize()
        {
            this.OpenHeap.Clear();
        }

        public void Replace(LocationRecord nodeToBeReplaced, LocationRecord nodeToReplace)
        {
            this.OpenHeap.Remove(nodeToBeReplaced);
            this.OpenHeap.Enqueue(nodeToReplace);
        }

        public LocationRecord GetBestAndRemove()
        {
            return this.OpenHeap.Dequeue();
        }

        public LocationRecord PeekBest()
        {
            return this.OpenHeap.Peek();
        }

        public void AddToOpen(LocationRecord nodeRecord)
        {
            this.OpenHeap.Enqueue(nodeRecord);
        }

        public void RemoveFromOpen(LocationRecord nodeRecord)
        {
            this.OpenHeap.Remove(nodeRecord);
        }

        public LocationRecord SearchInOpen(LocationRecord nodeRecord)
        {
            return this.OpenHeap.SearchForEqual(nodeRecord);
        }

        public ICollection<LocationRecord> All()
        {
            return this.OpenHeap;
        }

        public int CountOpen()
        {
            return this.OpenHeap.Count;
        }
    }
}