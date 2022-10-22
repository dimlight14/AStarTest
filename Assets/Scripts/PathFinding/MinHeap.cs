using System;
using PathFinding.Grid;

namespace PathFinding
{
    public class MinHeap
    {
        private PathFindingNode[] _items;
        private PathFindingNode[] _initialCollection;
        private int _currentItemCount;

        public int Count => _currentItemCount;

        public void Initialize(PathFindingNode[] initialCollection)
        {
            _initialCollection = initialCollection;
            _currentItemCount = 0;
            if (_items == null || _items.Length != initialCollection.Length)
            {
                _items = new PathFindingNode[_initialCollection.Length];
            }
            else
            {
                //TODO improve clearing?
                Array.Clear(_items, 0, _items.Length);
            }
        }

        public void Enqueue(PathFindingNode item)
        {
            _items[_currentItemCount] = item;
            UpdateHeapIndex(_currentItemCount);
            SortUp(_currentItemCount);
            _currentItemCount++;
        }

        public PathFindingNode Dequeue()
        {
            var firstItem = _items[0];
            
            _currentItemCount--;
            _items[0] = _items[_currentItemCount];
            _items[_currentItemCount].Index = 0;
            UpdateHeapIndex(0);
            
            SortDown(0);
            
            return firstItem;
        }

        public void UpdateItem(PathFindingNode node)
        {
            _items[node.HeapIndex] = node;
            SortUp(node.HeapIndex);
        }

        public bool Contains(int heapIndex, int initialCollectionIndex)
        {
            return _items[heapIndex].Index == initialCollectionIndex;
        }

        private void UpdateHeapIndex(int heapPosition)
        {
            _items[heapPosition].HeapIndex = heapPosition;
            _initialCollection[_items[heapPosition].Index].HeapIndex = heapPosition;
        }

        private void SortDown(int heapIndex)
        {
            var currentItem = _items[heapIndex];
            while (true)
            {
                var childIndexLeft = currentItem.HeapIndex * 2 + 1;
                var childIndexRight = currentItem.HeapIndex * 2 + 2;

                if (childIndexLeft < _currentItemCount)
                {
                    var swapIndex = childIndexLeft;

                    if (childIndexRight < _currentItemCount)
                    {
                        if (_items[childIndexLeft].CompareTo(_items[childIndexRight]) < 0)
                        {
                            swapIndex = childIndexRight;
                        }
                    }

                    if (currentItem.CompareTo(_items[swapIndex]) < 0)
                    {
                        Swap(currentItem.HeapIndex, swapIndex);
                        currentItem = _items[swapIndex];
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }
        }

        private void SortUp(int indexToSort)
        {
            var parentIndex = (indexToSort - 1) / 2;
            var currentItem = _items[indexToSort];

            while (true)
            {
                if (currentItem.CompareTo(_items[parentIndex]) > 0)
                {
                    Swap(currentItem.HeapIndex, parentIndex);
                }
                else
                {
                    break;
                }

                currentItem = _items[parentIndex];
                parentIndex = (currentItem.HeapIndex - 1) / 2;
            }
        }

        private void Swap(int indexA, int indexB)
        {
            var temp = _items[indexA];
            _items[indexA] = _items[indexB];
            UpdateHeapIndex(indexA);
            _items[indexB] = temp;
            UpdateHeapIndex(indexB);
        }
    }
}