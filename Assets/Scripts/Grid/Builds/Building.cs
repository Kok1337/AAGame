using GridSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Transform)), RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(FadeObject))]
public abstract class Building : MonoBehaviour
{
    protected Vector2Int _gridPosition; 

    [Header("Area")]

    [SerializeField]
    [Min(0)]
    protected int _width;
    [SerializeField]
    [Min(0)]
    protected int _height;

    private PathfindingGrid _grid;

    public PathfindingGrid Grid
    {
        set
        {
            _grid = value;
        }
    }

    public bool[,] WalkableArea
    {
        get
        {
            return _walkableArea;
        }
    }

    public Vector2Int Position
    {
        get
        {
            return _gridPosition;
        }

        private set
        {
            if (_gridPosition != value)
            {
                _grid.TrigerBuildingChanged(_gridPosition, _walkableArea, value, _walkableArea);
                _gridPosition = value;
            }
        }
    }

    private bool[,] _walkableArea; 

    private void Start()
    {
        // Transform transform = GetComponent<Transform>();

        _gridPosition = GetGridPosition();

        GenerateArea();
    }

    private Vector2Int GetGridPosition()
    {
        return new Vector2Int(Mathf.FloorToInt(transform.localPosition.x), Mathf.FloorToInt(transform.localPosition.y));
    }

    private void Update()
    {
        if (transform.hasChanged)
        {
            Position = GetGridPosition();
        }
    }

    private void GenerateArea()
    {
        _walkableArea = new bool[_width, _height];
        FillWalkableArea(false);
    }

    private void FillWalkableArea(bool defaultValue)
    {
        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                _walkableArea[i, j] = defaultValue;
            }
        }
        SetupWalkableArea(_width, _height, _walkableArea);
    }

    public void LeftTurn()
    {
        int temp = _width;
        _width = _height;
        _height = temp;
        bool[,] nArea = new bool[_width, _height];

        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                nArea[i, j] = _walkableArea[_height - j - 1, i];
            }
        }

        _grid.TrigerBuildingChanged(_gridPosition, _walkableArea, _gridPosition, nArea);

        _walkableArea = nArea;
    }

    public void RightTurn()
    {
        int temp = _width;
        _width = _height;
        _height = temp;
        bool[,] nArea = new bool[_width, _height];

        for (int i = 0; i < _width; i++)
        {
            for (int j = 0; j < _height; j++)
            {
                nArea[i, j] = _walkableArea[j, _width - i - 1];
            }
        }

        _grid.TrigerBuildingChanged(_gridPosition, _walkableArea, _gridPosition, nArea);

        _walkableArea = nArea;
    }

    protected abstract void SetupWalkableArea(int width, int height, bool[,] walkableArea);
}
