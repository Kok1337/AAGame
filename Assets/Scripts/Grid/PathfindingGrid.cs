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

        protected const int DiagonalWeight = 14;
        protected const int DirectWeight = 10;

        private GameObject _debugGameObjectsLayer;
        private GameObject[,] _debugArray;
        private Sprite _sprite;

        private List<PathNode> _path;

        public PathfindingGrid(int wight, int hight, float cellSize, Transform transform, Sprite sprite, Func<CustomGrid<PathNode>, int, int, PathNode> createGridObject) : base(wight, hight, cellSize, transform, createGridObject)
        {
            _sprite = sprite;
            _path = new List<PathNode>();

            _debugGameObjectsLayer = new GameObject(DebugLayerName);
            _debugGameObjectsLayer.transform.parent = _transform;

            GenereDebugInfo();
        }

        #region Debug
        private void GenereDebugInfo()
        {
            _debugArray = new GameObject[_width, _height];
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    _debugArray[x, y] = GenarateDebugObject(_gridArray[x, y]);
                }
            }

            Notify += (int x, int y, PathNode pathNode) =>
            {
                Color nodeColor = pathNode.IsWalkable ? Color.white : Color.blue;
                ColorizeNode(x, y, nodeColor);
            };
        }

        private GameObject GenarateDebugObject(PathNode pathNode)
        {
            GameObject debugGameObject = new GameObject(pathNode.ToString());
            SpriteRenderer spriteRenderer = debugGameObject.AddComponent<SpriteRenderer>();    
            spriteRenderer.sprite = _sprite;
            spriteRenderer.color = Color.white;
            debugGameObject.transform.parent = _debugGameObjectsLayer.transform;

            Vector3 offset = new Vector3(_sprite.pivot.x / _sprite.pixelsPerUnit, _sprite.pivot.y / _sprite.pixelsPerUnit);
            debugGameObject.transform.position = GetWorldPosition(pathNode.GridPosition.x, pathNode.GridPosition.y) + offset * CellSize;
            return debugGameObject;
        }

        private void ColorizeNode(int x, int y, Color color)
        {
            _debugArray[x, y].GetComponent<SpriteRenderer>().color = color;
        }

        /*
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
        */

        private void ClearPath()
        {
            foreach (PathNode node in _path)
            {
                node.Clear();
                Color color = node.IsWalkable ? Color.white : Color.blue;
                ColorizeNode(node.X, node.Y, color);
            }
        }

        private void ColorizePath()
        {
            foreach (PathNode node in _path)
            {
                ColorizeNode(node.X, node.Y, Color.red);
            }
        }
        #endregion

        public List<PathNode> FindPath(Vector3 startWorldPosition, Vector3 endWorldPosition)
        {
            Vector2Int startPos = GetXY(startWorldPosition);
            Vector2Int endPos = GetXY(endWorldPosition);
            return FindPath(startPos.x, startPos.y, endPos.x, endPos.y);
        }

        /*
        private void ClearAllNodes()
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    _gridArray[x, y].Clear();
                }
            }
        }
        */

        public List<PathNode> FindPath(int startX, int startY, int endX, int endY)
        {
            ClearPath();
            // ClearAllNodes();

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

                        int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour);
                        if (newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour))
                        {
                            neighbour.gCost = newMovementCostToNeighbour;
                            neighbour.hCost = GetDistance(neighbour, endNode);
                            neighbour.comeFromNode = currentNode;

                            if (!openSet.Contains(neighbour))
                            {
                                openSet.Add(neighbour);
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
    }
}

