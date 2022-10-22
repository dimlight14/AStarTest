using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

namespace PathFinding.Grid
{
    public class GridHolder : IGridHolder, IGridDataProvider, INativeGridDataProvider
    {
        private GridNode[] _gridNodes;
        private NativeArray<GridNode> _nativeGridNodes;

        public IReadOnlyCollection<GridNode> GridNodes => _gridNodes;
        public NativeArray<GridNode> NativeGridNodes => _nativeGridNodes;

        public void GenerateGridBase(Vector2Int dimensions)
        {
            var size = dimensions.x * dimensions.y;
            _gridNodes = new GridNode[size];
            _nativeGridNodes = new NativeArray<GridNode>(size, Allocator.Persistent);

            for (var i = 0; i < dimensions.x; i++)
            {
                for (var j = 0; j < dimensions.y; j++)
                {
                    var newNode = new GridNode()
                    {
                        X = i,
                        Y = j,
                        IsWalkable = true,
                        Index = PathFindingUtility.GetIndex(i, j, dimensions.x)
                    };
                    _gridNodes[j * dimensions.y + i] = newNode;
                    _nativeGridNodes[j * dimensions.y + i] = newNode;
                }
            }
        }

        public bool IsNodeWalkable(int index)
        {
            return _gridNodes[index].IsWalkable;
        }

        public void SetNodeWalkable(int index, bool value)
        {
            _gridNodes[index].SetWalkable(value);
            var node = _nativeGridNodes[index];
            node.SetWalkable(value);
            _nativeGridNodes[index] = node;
        }

        public void Dispose()
        {
            _nativeGridNodes.Dispose();
        }
    }
}