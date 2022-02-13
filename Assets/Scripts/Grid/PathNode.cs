using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GridSystem
{
    public class PathNode : IHeapItem<PathNode>
    {
        private CustomGrid<PathNode> _grid;
        private Vector2Int _gridPosition;
        private bool _isWalkable;  

        // Пройденный вес 
        public int gCost;
        // Предполагаемый остальной вес 
        public int hCost;
        // Штрафной вес
        private byte _penalty;

        public PathNode comeFromNode;

        private int _heapIndex;

        public PathNode(CustomGrid<PathNode> grid, int x, int y, byte penalty = 0)
        {
            _grid = grid;
            _gridPosition = new Vector2Int(x, y);
            IsWalkable = true;
            _penalty = penalty;
        }

        public override string ToString()
        {
            return _gridPosition.x + " " + _gridPosition.y;
        }

        public Vector3 GetNodeCenterPosition()
        {
            return _grid.GetNodeCenterPosition(X, Y);
        }

        public Vector2Int GridPosition
        {
            get
            {
                return _gridPosition;
            }
        }

        public byte Penalty
        {
            get => _penalty;
            set => _penalty = value;
        }

        public void InvestIsWalkable()
        {
            IsWalkable = !IsWalkable;
        }

        public void Clear()
        {
            gCost = 0;
            hCost = 0;
            comeFromNode = null;
        }

        public int X
        {
            get => _gridPosition.x;
        }

        public int Y
        {
            get => _gridPosition.y;
        }

        public bool IsWalkable
        {
            get => _isWalkable;

            set
            {
				_isWalkable = value;
                _grid.TrigerGridObjectChanged(X, Y);
            }
            
        }

        public int fCost
        {
            get
            {
                return gCost + hCost;
            }
        }

        public int CompareTo(PathNode other)
        {
            int compare = fCost.CompareTo(other.fCost);
            if (compare == 0)
            {
                compare = hCost.CompareTo(other.hCost);
            }
            return -compare;
        }

        public int HeapIndex
        {
            get
            {
                return _heapIndex;
            }
            set
            {
                _heapIndex = value;
            }
        }
    }
}