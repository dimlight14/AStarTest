namespace PathFinding.Grid
{
    public struct GridNode
    {
        public int X;
        public int Y;
        public bool IsWalkable;
        public int Index;

        public void SetWalkable(bool value)
        {
            IsWalkable = value;
        }
    }
}