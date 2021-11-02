using GridSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAndGridTest : MonoBehaviour
{
    public Camera Camera;

    private PathfindingGrid _pathfindingGrid;

    private Player _player;

    public Sprite sprite;

    void Start()
    {
        _pathfindingGrid = new PathfindingGrid(100, 100, 1f, transform, sprite, (CustomGrid<PathNode> grid, int x, int y) => new PathNode(grid, x, y));

        _player = GameObject.FindWithTag("Player").GetComponent<Player>();
    }

    void Update()
    {
        if (Camera != null)
        {         
            if (Input.GetMouseButtonDown(0))
            {
                if (_player != null)
                {
                    _player.FollowPath(_pathfindingGrid.FindPath(_player.transform.position, GetWorldMousePosition()));
                }
            }

            if (Input.GetMouseButtonDown(1))
            {
                PathNode pathNode = _pathfindingGrid.GetValue(GetWorldMousePosition());
                pathNode?.InvestIsWalkable();
            }
        }
    }

    private Vector3 GetWorldMousePosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -Camera.transform.position.z;
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }
}
