using System;
using System.Collections.Generic;
using UnityEngine;

namespace GridSystem
{
    public class CustomGrid<TGridObject>
    {
        protected int _width, _height;
        protected float _cellSize;
        protected Transform _transform;
        protected TGridObject[,] _gridArray;

        public event ObjectChangedHandler NotifyObjectChanged;
        public delegate void ObjectChangedHandler(int x, int y, TGridObject gridObject);

        public bool showDebug = true;

        public CustomGrid(int wight, int hight, float cellSize, Transform transform, Func<CustomGrid<TGridObject>, int, int, TGridObject> createGridObject)
        {
            _width = wight;
            _height = hight;
            _cellSize = cellSize;
            _transform = transform;

            GenerateGrid(createGridObject);
        }

        protected virtual void GenerateGrid(Func<CustomGrid<TGridObject>, int, int, TGridObject> createGridObject)
        {
            _gridArray = new TGridObject[_width, _height];

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    _gridArray[x, y] = createGridObject(this, x, y);
                }
            }
        }

        public List<TGridObject> GetNeighbors(Vector2Int position)
        {
            List<TGridObject> neighbors = new List<TGridObject>();

            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                        continue;

                    int checkX = position.x + x;
                    int checkY = position.y + y;

                    if (checkX >= 0 && checkX < _width && checkY >= 0 && checkY < _height)
                    {
                        neighbors.Add(_gridArray[checkX, checkY]);
                    }
                }
            }

            return neighbors;
        }

        public int MaxSize
        {
            get => Width * Hight;
        }

        protected Vector3 GetWorldPosition(int x, int y)
        { 
            return _transform.position + (new Vector3(x, y) * _cellSize);
        }

        public Vector3 GetNodeCenterPosition(int x, int y)
        {
            return GetWorldPosition(x, y) + (new Vector3(.5f, .5f) * _cellSize);
        }

        protected Vector2Int GetXY(Vector3 worldPosition)
        {
            return new Vector2Int(Mathf.FloorToInt((worldPosition.x - _transform.position.x) / _cellSize), Mathf.FloorToInt((worldPosition.y - _transform.position.y) / _cellSize));
        }

        public void SetValue(int x, int y, TGridObject value)
        {
            if (x >= 0 && y >= 0 && x < _width && y < _height)
            {
                _gridArray[x, y] = value;
                NotifyObjectChanged?.Invoke(x, y, value);
            }
        }

        public void TrigerGridObjectChanged(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < _width && y < _height)
            {
                NotifyObjectChanged?.Invoke(x, y, _gridArray[x, y]);
            }
        }

        public void SetValue(Vector3 worldPosition, TGridObject value)
        {
            Vector2Int gridPosition = GetXY(worldPosition);
            SetValue(gridPosition.x, gridPosition.y, value);
        }

        public TGridObject GetValue(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < _width && y < _height)
            {
                return _gridArray[x, y];
            }
            return default; 
        }

        public TGridObject GetValue(Vector3 worldPosition)
        {
            Vector2Int gridPosition = GetXY(worldPosition);
            return GetValue(gridPosition.x, gridPosition.y);
        }

        public int Width
        {
            get
            {
                return _width;
            }
        }

        public int Hight
        {
            get
            {
                return _height;
            }
        }

        public float CellSize
        {
            get
            {
                return _cellSize;
            }
        }
    }
}