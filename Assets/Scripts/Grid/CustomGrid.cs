using System;
using UnityEngine;

namespace GridSystem
{
    public class CustomGrid<TGridObject>
    {
        private int _width, _height;
        private float _cellSize;
        private Transform _transform;
        private TGridObject[,] _gridArray;

        public event ObjectChangedHandler Notify;

        public delegate void ObjectChangedHandler(int x, int y, TGridObject gridObject);

        public CustomGrid(int wight, int hight, float cellSize, Transform transform, Func<CustomGrid<TGridObject>, int, int, TGridObject> createGridObject)
        {
            _width = wight;
            _height = hight;
            _cellSize = cellSize;
            _transform = transform;

            GenerateGrid(createGridObject);
        }

        private void GenerateGrid(Func<CustomGrid<TGridObject>, int, int, TGridObject> createGridObject)
        {
            _gridArray = new TGridObject[_width, _height];

            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    _gridArray[x, y] = createGridObject(this, x, y);
                }
            }

            bool showDebug = true;
            if (showDebug)
            {
                TextMesh[,] _textMeshArray = new TextMesh[_width, _height];
                for (int x = 0; x < _width; x++)
                {
                    for (int y = 0; y < _height; y++)
                    {
                        _textMeshArray[x, y] = CreateTextMesh(GetWorldPosition(x, y) + new Vector3(_cellSize, _cellSize) * .5f, _gridArray[x, y].ToString(), Color.black);
                        Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x, y + 1), Color.white, 100f);
                        Debug.DrawLine(GetWorldPosition(x, y), GetWorldPosition(x + 1, y), Color.white, 100f);
                    }
                }
                Debug.DrawLine(GetWorldPosition(0, _height), GetWorldPosition(_width, _height), Color.white, 100f);
                Debug.DrawLine(GetWorldPosition(_width, 0), GetWorldPosition(_width, _height), Color.white, 100f);

                Notify += (int x, int y, TGridObject gridObject) =>
                {
                    _textMeshArray[x, y].text = gridObject?.ToString();
                };
            }
        }

        private TextMesh CreateTextMesh(Vector3 position, string text, Color color)
        {
            GameObject gameObject = new GameObject("World_text", typeof(TextMesh));
            gameObject.transform.localPosition = position;
            gameObject.transform.SetParent(_transform, false);
            float scale = 0.2f * _cellSize;
            gameObject.transform.localScale = new Vector3(scale, scale);
            TextMesh textMesh = gameObject.GetComponent<TextMesh>();
            textMesh.fontSize = 20;
            textMesh.anchor = TextAnchor.MiddleCenter;
            textMesh.transform.position = position;
            textMesh.text = text;
            textMesh.color = color;
            return textMesh;
        }

        private Vector3 GetWorldPosition(int x, int y)
        {
            return _transform.position + (new Vector3(x, y) * _cellSize);
        }

        private Vector2Int GetXY(Vector3 worldPosition)
        {
            return new Vector2Int(Mathf.FloorToInt(worldPosition.x - _transform.position.x / _cellSize), Mathf.FloorToInt(worldPosition.y - _transform.position.y / _cellSize));
        }

        public void SetValue(int x, int y, TGridObject value)
        {
            if (x >= 0 && y >= 0 && x < _width && y < _height)
            {
                _gridArray[x, y] = value;
                Notify?.Invoke(x, y, value);
            }
        }

        public void TrigerGridObjectChanged(int x, int y)
        {
            if (x >= 0 && y >= 0 && x < _width && y < _height)
            {
                Notify?.Invoke(x, y, _gridArray[x, y]);
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