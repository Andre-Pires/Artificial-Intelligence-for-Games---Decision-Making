using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.IAJ.Unity.TacticalAnalysis.DataStructures
{
    //very simple (and unefficient) implementation of the open sets
    public class SimpleUnorderedList : IOpenLocationRecord
    {
        private List<LocationRecord> NodeRecords { get; set; }

        public SimpleUnorderedList()
        {
            this.NodeRecords = new List<LocationRecord>();
        }

        public void Initialize()
        {
            this.NodeRecords.Clear(); 
        }

        public int CountOpen()
        {
            return this.NodeRecords.Count;
        }

        public void AddToOpen(LocationRecord nodeRecord)
        {
            this.NodeRecords.Add(nodeRecord);
        }

        public void RemoveFromOpen(LocationRecord nodeRecord)
        {
            this.NodeRecords.Remove(nodeRecord);
        }

        public LocationRecord SearchInOpen(LocationRecord nodeRecord)
        {
            //here I cannot use the == comparer because the nodeRecord will likely be a different computational object
            //and therefore pointer comparison will not work, we need to use Equals
            //LINQ with a lambda expression
            return this.NodeRecords.FirstOrDefault(n => n.Equals(nodeRecord));
        }

        public ICollection<LocationRecord> All()
        {
            return this.NodeRecords;
        }

        public void Replace(LocationRecord nodeToBeReplaced, LocationRecord nodeToReplace)
        {
            //since the list is not ordered we do not need to remove the node and add the new one, just copy the different values
            //remember that if NodeRecord is a struct, for this to work we need to receive a reference
            nodeToBeReplaced.Parent = nodeToReplace.Parent;
            nodeToBeReplaced.Influence = nodeToReplace.Influence;
            nodeToBeReplaced.StrongestInfluenceUnit = nodeToReplace.StrongestInfluenceUnit;
        }

        public LocationRecord GetBestAndRemove()
        {
            var best = this.PeekBest();
            this.NodeRecords.Remove(best);
            return best;
        }

        public LocationRecord PeekBest()
        {
            //welcome to LINQ guys, for those of you that remember LISP from the AI course, the LINQ Aggregate method is the same as lisp's Reduce method
            //so here I'm just using a lambda that compares the first element with the second and returns the lowest
            //by applying this to the whole list, I'm returning the node with the lowest F value.
            return this.NodeRecords.Aggregate((nodeRecord1, nodeRecord2) => nodeRecord1.Influence > nodeRecord2.Influence ? nodeRecord1 : nodeRecord2);
        }
    }
}
