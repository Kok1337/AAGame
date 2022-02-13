using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GridSystem
{
    public class PathfindingGrid : CustomGrid<PathNode>
    {
        private const string DebugLayerName = "Debug Layer";
        private const string BuildingLayerName = "Building Layer";

        // PathFinding
        protected const int DiagonalWeight = 14;
        protected const int DirectWeight = 10;
        private List<PathNode> _path;
        private bool _showDebug = false;

        // Debug
        private GameObject _debugGameObjectsLayer;
        private GameObject[,] _debugArray;
        private Sprite _sprite;

        // private 
        private float _alpha = 0.3f;
        private Color _pathColor = Color.blue;
        private Color _pathlessColor = Color.red;
        private Color _buildingColor = Color.green;
        private Color _walkableColor = Color.white;

        // Buildings
        private GameObject _buildingsGameObjectsLayer;
        public event BuildingChangedHandler NotifyBuildingChanged;
        public delegate void BuildingChangedHandler(Vector2Int oldPosition, bool[,] oldArea, Vector2Int newPosition, bool[,] newArea);

        private Vector2Int _tempBuildingPosition;
        private bool[,] _tempBuildingArea;

        public PathfindingGrid(int wight, int hight, float cellSize, Transform transform, Sprite sprite, Func<CustomGrid<PathNode>, int, int, PathNode> createGridObject) : base(wight, hight, cellSize, transform, createGridObject)
        {
            _sprite = sprite;

            _pathColor.a = _alpha;
            _pathlessColor.a = _alpha;
            _buildingColor.a = _alpha;
            _walkableColor.a = 0.2f;

            _path = new List<PathNode>();   

            GenereDebugInfo();
            GenereBuildingsInfo();
        }

        #region Debug

        private void GenereDebugInfo()
        {
            _debugGameObjectsLayer = new GameObject(DebugLayerName);
            _debugGameObjectsLayer.transform.parent = _transform;
            _debugGameObjectsLayer.transform.localPosition = Vector3.zero;

            _debugArray = new GameObject[_width, _height];
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    _debugArray[x, y] = GenarateDebugObject(_gridArray[x, y]);
                }
            }

            NotifyObjectChanged += (int x, int y, PathNode pathNode) =>
            {
                Color nodeColor = pathNode.IsWalkable ? _walkableColor : _pathlessColor;
                ColorizeNode(x, y, nodeColor);
            };

            SetDebugLayerActive();
        }

        private GameObject GenarateDebugObject(PathNode pathNode)
        {
            GameObject debugGameObject = new GameObject(pathNode.ToString());
            SpriteRenderer spriteRenderer = debugGameObject.AddComponent<SpriteRenderer>();    
            spriteRenderer.sprite = _sprite;
            spriteRenderer.sortingLayerName = "Debug";

            spriteRenderer.color = _walkableColor;     
            debugGameObject.transform.parent = _debugGameObjectsLayer.transform;
            debugGameObject.transform.localScale *= CellSize;

            Vector3 offset = new Vector3(_sprite.pivot.x / _sprite.pixelsPerUnit, _sprite.pivot.y / _sprite.pixelsPerUnit);
            debugGameObject.transform.position = GetWorldPosition(pathNode.GridPosition.x, pathNode.GridPosition.y) + offset * CellSize;
            return debugGameObject;
        }

        private void ColorizeNode(int x, int y, Color color)
        {
            _debugArray[x, y].GetComponent<SpriteRenderer>().color = color;
        }

        private void ClearPath()
        {
            foreach (PathNode node in _path)
            {
                node.Clear();
                Color color = node.IsWalkable ? _walkableColor : _pathlessColor;
                ColorizeNode(node.X, node.Y, color);
            }
        }

        private void ColorizePath()
        {
            foreach (PathNode node in _path)
            {
                ColorizeNode(node.X, node.Y, _pathColor);
            }
        }

        public bool ShowDebug
        {
            get
            {
                return _showDebug;
            }
 
            set
            {
                if (_showDebug != value)
                {
                    _showDebug = value;
                    SetDebugLayerActive();
                }              
            }
        }

        private void SetDebugLayerActive()
        {
            _debugGameObjectsLayer.SetActive(_showDebug);
        }

        #endregion

        #region FindPath

        public List<PathNode> FindPath(Vector3 startWorldPosition, Vector3 endWorldPosition)
        {
            Vector2Int startPos = GetXY(startWorldPosition);
            Vector2Int endPos = GetXY(endWorldPosition);
            return FindPath(startPos.x, startPos.y, endPos.x, endPos.y);
        }

        public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
        {
            ClearPath();

            PathNode startNode = GetValue(startX, startY);
            PathNode endNode = GetValue(endX, endY);

            if (startNode != null && endNode != null)
            {
                Heap<PathNode> openSet = new Heap<PathNode>(MaxSize);
                HashSet<PathNode> closedSet = new HashSet<PathNode>();
                openSet.Add(startNode);

                while (!openSet.IsEmpty)
                {
                    PathNode currentNode = openSet.RemoveFirst();
                    closedSet.Add(currentNode);

                    // Если мы нашли нужную Node
                    if (currentNode == endNode)
                    {
                        break;
                    }

                    foreach (PathNode neighbour in GetNeighbors(currentNode.GridPosition))
                    {
                        if (!neighbour.IsWalkable || closedSet.Contains(neighbour))
                        {
                            continue;
                        }

                        int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.Penalty;
                        if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                        {
                            neighbour.gCost = newMovementCostToNeighbour;
                            neighbour.hCost = GetDistance(neighbour, endNode);
                            neighbour.comeFromNode = currentNode;

                            if (!openSet.Contains(neighbour))
                            {
                                openSet.Add(neighbour);
                            }
                            else
                            {
                                openSet.UpdateItem(neighbour);
                            }
                        }
                    }
                }
            }           

            RetracePath(startNode, endNode);
            return _path;
        }

        private void RetracePath(PathNode startNode, PathNode endNode)
        {
            List<PathNode> path = new List<PathNode>();

            if (startNode != null && endNode != null && endNode.comeFromNode != null)
            {
                PathNode currentNode = endNode;
                while (currentNode != startNode)
                {
                    path.Add(currentNode);
                    currentNode = currentNode.comeFromNode;
                }
                path.Add(startNode);
                path.Reverse();
            }

            _path = path;
            ColorizePath();
        }

        protected int GetDistance(PathNode nodeA, PathNode nodeB)
        {
            int dstX = Mathf.Abs(nodeA.GridPosition.x - nodeB.GridPosition.x);
            int dstY = Mathf.Abs(nodeA.GridPosition.y - nodeB.GridPosition.y);
            int minDst = Mathf.Min(dstX, dstY);
            int maxDst = Mathf.Max(dstX, dstY);
            return (minDst * DiagonalWeight) + ((maxDst - minDst) * DirectWeight);      
        }

        #endregion

        #region Buildings

        private void GenereBuildingsInfo()
        {
            Transform buildingLayerTransform = _transform.Find(BuildingLayerName);
            if (buildingLayerTransform != null)
            {
                _buildingsGameObjectsLayer = buildingLayerTransform.gameObject;
            }
            else
            {
                _buildingsGameObjectsLayer = new GameObject(BuildingLayerName);
                _buildingsGameObjectsLayer.transform.parent = _transform;
            }
            _buildingsGameObjectsLayer.transform.localPosition = Vector3.zero;

            // Смотрим все вложенные объекты
            foreach (Transform child in _buildingsGameObjectsLayer.transform)
            {
                Building building = child.GetComponent<Building>();
                if (building != null)
                {
                    building.Grid = this;
                    FillBuildingArea(building.Position, building.WalkableArea);
                }
            }     


            NotifyBuildingChanged += (Vector2Int oldPosition, bool[,] oldArea, Vector2Int newPosition, bool[,] newArea) =>
            {
                ClearBuildingArea(oldPosition, oldArea);
                FillBuildingArea(newPosition, newArea);   
            };
        }

        public void TrigerBuildingChanged(Vector2Int oldPosition, bool[,] oldArea, Vector2Int newPosition, bool[,] newArea)
        {
            NotifyBuildingChanged?.Invoke(oldPosition, oldArea, newPosition, newArea);
        }

        public void AddBuilding(Building building)
        {
            building.transform.SetParent(_buildingsGameObjectsLayer.transform);
            building.Grid = this;
            FillBuildingArea(building.Position, building.WalkableArea);
        }

        public void FillBuildingArea(Vector2Int position, bool[,] area)
        {
            for (int dx = 0; dx < area.GetLength(0); dx++)
            {
                for (int dy = 0; dy < area.GetLength(1); dy++)
                {
                    int x = position.x + dx;
                    int y = position.y + dy;

                    PathNode node = GetValue(x, y);

                    if (node != null)
                    {
                        node.IsWalkable = area[dx, dy];
                    }
                }
            }
        }

        public void ClearBuildingArea(Vector2Int position, bool[,] area)
        {
            for (int dx = 0; dx < area.GetLength(0); dx++)
            {
                for (int dy = 0; dy < area.GetLength(1); dy++)
                {
                    int x = position.x + dx;
                    int y = position.y + dy;

                    PathNode node = GetValue(x, y);

                    if (node != null)
                    {
                        node.IsWalkable = true;
                    }
                }
            }
        }

        public bool IsCanBuild(Building building)
        {
            ClearTempBuildingArea();

            _tempBuildingPosition = GetXY(building.transform.position);
            _tempBuildingArea = building.WalkableArea;

            bool canBuild = CheckBuild();

            FillTempBuildingArea(canBuild);
                    
            return canBuild;
        }

        private bool CheckBuild()
        {
            if (_tempBuildingPosition == null || _tempBuildingArea == null)
            {
                return false;
            }
            for (int dx = 0; dx < _tempBuildingArea.GetLength(0); dx++)
            {
                for (int dy = 0; dy < _tempBuildingArea.GetLength(1); dy++)
                {
                    int x = _tempBuildingPosition.x + dx;
                    int y = _tempBuildingPosition.y + dy;
                    PathNode node = GetValue(x, y);
                    if (node == null)
                    {
                        return false;
                    }
                    else if (node.IsWalkable == _tempBuildingArea[dx, dy])
                    {
                        return false;
                    }
                }
            }
            return true;
        }
     
        private void FillTempBuildingArea(bool canBuild)
        {
            if (_tempBuildingPosition != null && _tempBuildingArea != null)
            {
                Color buildColor = canBuild ? _buildingColor : _pathlessColor;
                for (int dx = 0; dx < _tempBuildingArea.GetLength(0); dx++)
                {
                    for (int dy = 0; dy < _tempBuildingArea.GetLength(1); dy++)
                    {
                        int x = _tempBuildingPosition.x + dx;
                        int y = _tempBuildingPosition.y + dy;

                        PathNode node = GetValue(x, y);

                        if (node != null && node.IsWalkable != _tempBuildingArea[dx, dy])
                        {
                            ColorizeNode(x, y, buildColor);
                        }
                    }
                }
            }            
        }

        private void ClearTempBuildingArea()
        {
            if (_tempBuildingPosition != null && _tempBuildingArea != null)
            {
                for (int dx = 0; dx < _tempBuildingArea.GetLength(0); dx++)
                {
                    for (int dy = 0; dy < _tempBuildingArea.GetLength(1); dy++)
                    {
                        int x = _tempBuildingPosition.x + dx;
                        int y = _tempBuildingPosition.y + dy;

                        PathNode node = GetValue(x, y);

                        if (node != null && node.IsWalkable != _tempBuildingArea[dx, dy])
                        {
                            ColorizeNode(x, y, _walkableColor);
                        }
                    }
                }
            }
        }

        #endregion
    }
}