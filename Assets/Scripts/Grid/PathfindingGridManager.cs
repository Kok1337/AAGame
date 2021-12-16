using GridSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingGridManager : MonoBehaviour
{
    [Header("Grid params")]

    [SerializeField]
    [Min(1)]
    private int _wight = 100;

    [SerializeField]
    [Min(1)]
    private int _height = 100;

    [SerializeField]
    private float _cellSize = 1f;

    [Header("Debug params")]

    [SerializeField]
    private Sprite _debugSprite;

    [SerializeField]
    private bool _showGridDebug = true;

    private Camera _mainCamera;
    private Player _player;
    private PathfindingGrid _pathfindingGrid;

    private Building _flyingBuilding;

    private void Start()
    {
        _mainCamera = Camera.main;
        _player = GameObject.FindWithTag("Player").GetComponent<Player>();
        _pathfindingGrid = new PathfindingGrid(_wight, _height, _cellSize, transform, _debugSprite, (CustomGrid<PathNode> grid, int x, int y) => new PathNode(grid, x, y));
    }

    // Update is called once per frame
    private void Update()
    {
        if (_flyingBuilding != null)
        {
            UpdateFlyingBuilding();
        }
        else
        {
            ControllPlayer();
        }
    }

    private void ControllPlayer()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (_player != null)
            {
                _player.FollowPath(_pathfindingGrid.FindPath(_player.transform.position, GetWorldMousePosition()));
            }         
        }
    }

    private void UpdateFlyingBuilding()
    {
        // Получаем координаты курсора в мире
        Vector3 cursorWorldPosition = GetWorldMousePosition();         
        // Получаем координаты точки в мире
        Vector3 position = _pathfindingGrid.ConvertWorldPositionToGrid(cursorWorldPosition - _flyingBuilding.GetShiftForCursor());

        _flyingBuilding.transform.position = position;
        //_flyingBuilding.transform.position = _pathfindingGrid.ConvertWorldPositionToGrid(cursorWorldPosition);

        bool canBuildBuilding = _pathfindingGrid.IsCanBuild(_flyingBuilding);
        _flyingBuilding.SetCanBuild(canBuildBuilding);

        if (Input.GetMouseButtonDown(0) && canBuildBuilding)
        {
            BuildBuilding();
        }
        if (Input.GetMouseButtonDown(1))
        {
            CancelBuild();
        }  
    }

    private void CancelBuild()
    {
        Destroy(_flyingBuilding.gameObject);
        _flyingBuilding = null;
        ShowGridDebug = false;
    }

    private void BuildBuilding()
    {
        _pathfindingGrid.AddBuilding(_flyingBuilding);
        _flyingBuilding = null;
        ShowGridDebug = false;
    }

    private void OnDrawGizmosSelected()
    {
        Vector3 size = new Vector3(_wight, _height, -0.1f) * _cellSize;
        Vector3 center = transform.position + (size / 2);
        Gizmos.color = new Color(.5f, .5f, .5f, .5f);
        Gizmos.DrawCube(center, size);
    }

    private Vector3 GetWorldMousePosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = -_mainCamera.transform.position.z;
        return Camera.main.ScreenToWorldPoint(mousePosition);
    }

    public void StartPlacingBulding(Building buildingPrefab)
    {
        if (_flyingBuilding != null)
        {
            Destroy(_flyingBuilding.gameObject);
        }
        _flyingBuilding = Instantiate(buildingPrefab);
        ShowGridDebug = true;
    }

    public bool ShowGridDebug
    {
        get
        {
            return _showGridDebug;
        }

        set
        {
            _showGridDebug = value;
            _pathfindingGrid.ShowDebug = value;
        }
    }
}
