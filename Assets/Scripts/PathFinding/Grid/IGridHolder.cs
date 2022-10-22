using System;
using System.Collections.Generic;
using UnityEngine;

namespace PathFinding.Grid
{
    public interface IGridHolder : IDisposable
    {
        IReadOnlyCollection<GridNode> GridNodes { get; }
        void GenerateGridBase(Vector2Int dimensions);
        bool IsNodeWalkable(int index);
        void SetNodeWalkable(int index, bool value);
    }
}