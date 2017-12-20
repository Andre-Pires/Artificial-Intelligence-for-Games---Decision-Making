using System.Collections.Generic;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures
{
    public class RightPriorityList : IOpenSet, IComparer<NodeRecord>
    {
        private List<NodeRecord> Open { get; set; }

        public RightPriorityList()
        {
            this.Open = new List<NodeRecord>();    
        }
        public void Initialize()
        {
            this.Open.Clear();
        }

        public void Replace(NodeRecord nodeToBeReplaced, NodeRecord nodeToReplace)
        {
            this.Open.Remove(nodeToBeReplaced);
            this.AddToOpen(nodeToReplace);
        }

        public NodeRecord GetBestAndRemove()
        {
            var index = this.Open.Count - 1;
            var best = this.Open[index];
            this.Open.RemoveAt(index);
            return best;
        }

        public NodeRecord PeekBest()
        {
            return this.Open[this.Open.Count-1];
        }

        public void AddToOpen(NodeRecord nodeRecord)
        {
            int index = this.Open.BinarySearch(nodeRecord,this);
            if (index < 0)
            {
                this.Open.Insert(~index, nodeRecord);
            }
        }

        public void RemoveFromOpen(NodeRecord nodeRecord)
        {
            this.Open.Remove(nodeRecord);
        }

        public NodeRecord SearchInOpen(NodeRecord nodeRecord)
        {
            var count = this.Open.Count;
            for (int i = 0; i < count; i++)
            {
                if (this.Open[i].Equals(nodeRecord))
                {

                    return this.Open[i];
                }
            }
            return null;
        }

        public ICollection<NodeRecord> All()
        {
            return this.Open;
        }

        public int CountOpen()
        {
            return this.Open.Count;
        }

        public int Compare(NodeRecord x, NodeRecord y)
        {
            return y.CompareTo(x);
        }
    }
}
