using System.Collections.Generic;
using RAIN.Navigation.Graph;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures
{
    public class ClosedDictionary : IClosedSet
    {
        private Dictionary<NavigationGraphNode,NodeRecord> Closed { get; set; }

        public ClosedDictionary()
        {
            this.Closed = new Dictionary<NavigationGraphNode, NodeRecord>();
        }
        public void Initialize()
        {
            this.Closed.Clear();
        }

        public void AddToClosed(NodeRecord nodeRecord)
        {
            this.Closed.Add(nodeRecord.node,nodeRecord);
        }

        public void RemoveFromClosed(NodeRecord nodeRecord)
        {
            this.Closed.Remove(nodeRecord.node);
        }

        public NodeRecord SearchInClosed(NodeRecord nodeRecord)
        {
            if (this.Closed.ContainsKey(nodeRecord.node))
            {
                return this.Closed[nodeRecord.node];
            }
            else return null;
        }

        public ICollection<NodeRecord> All()
        {
            return this.Closed.Values;
        }
    }
}
