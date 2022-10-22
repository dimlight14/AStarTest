using System;

namespace PathFinding.Grid
{
    public struct PathFindingNode : IComparable<PathFindingNode>
    {
        public int X;
        public int Y;
        public bool Walkable;

        public int GCost;
        public int HCost;
        private int _fCost;


        public int ParentIndex;

        //Index of the node in the grid
        public int Index;

        //Index of the node in the heap (open list).
        //I use these indexes to cross-reference two collections since they are working on the copies of the same data.
        public int HeapIndex;

        public void ResetFCost()
        {
            _fCost = GCost + HCost;
        }

        public int CompareTo(PathFindingNode nodeToCompare)
        {
            var compare = _fCost.CompareTo(nodeToCompare._fCost);

            return -compare;
        }
    }
}