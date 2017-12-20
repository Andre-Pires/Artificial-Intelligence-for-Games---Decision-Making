using RAIN.Navigation.Graph;
using System;

namespace Assets.Scripts.IAJ.Unity.TacticalAnalysis.DataStructures
{
    public class LocationRecord : IComparable<LocationRecord>
    {
        public NavigationGraphNode Location { get; set; }
        public LocationRecord Parent { get; set; }
        public IInfluenceUnit StrongestInfluenceUnit { get; set; }
        public float Influence { get; set; }

        public int CompareTo(LocationRecord other)
        {
            return other.Influence.CompareTo(this.Influence);
        }

        //two node records are equal if they refer to the same node
        public override bool Equals(object obj)
        {
            var target = obj as LocationRecord;
            if (target != null) return this.Location == target.Location;
            else return false;
        }

        public override int GetHashCode()
        {
            return this.Location.GetHashCode();
        }
    }
}