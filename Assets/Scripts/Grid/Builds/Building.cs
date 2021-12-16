using GridSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Transform)), RequireComponent(typeof(Rigidbody2D)), RequireComponent(typeof(FadeObject)), RequireComponent(typeof(SpriteRenderer))]
public abstract class Building : MonoBehaviour
{
    protected Vector2Int _gridPosition; 

    [Header("Area")]

    [SerializeField]
    [Min(0)]
    protected int _width = 0;
    [SerializeField]
    [Min(0)]
    protected int _height = 0;

    private PathfindingGrid _grid;
    private SpriteRenderer _spriteRenderer;

    private Color _defoultColor = new Color(1, 1, 1, 1);
    private Color _canBuildColor = new Color(0, 1, 0, 0.5f);
    private Color _cantBuildColor = new Color(1, 0, 0, 0.5f);

    public PathfindingGrid Grid
    {
        set
        {
            _grid = value;
            Colorize(_defoultColor);
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
        _gridPosition = GetGridPosition();
        GenerateArea();
    }

    private void Colorize(Color color)
    {
        _spriteRenderer.color = color;
    }

    private Vector2Int GetGridPosition()
    {
        return new Vector2Int(Mathf.FloorToInt(transform.localPosition.x), Mathf.FloorToInt(transform.localPosition.y));
    }

    private void Update()
    {      
        if (transform.hasChanged && _grid != null)
        {
            Position = GetGridPosition();
        }
    }

    private void OnDisable()
    {
        if (_grid != null)
        {
            _grid.TrigerBuildingChanged(_gridPosition, _walkableArea, _gridPosition, new bool[0, 0]);
        }
    }

    private void OnEnable()
    {
        if (_spriteRenderer == null)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        
        if (_grid != null)
        {
            _grid.TrigerBuildingChanged(_gridPosition, new bool[0, 0], _gridPosition, _walkableArea);
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

    public Vector3 GetShiftForCursor()
    {
        float halfWidth = _spriteRenderer.bounds.size.x / 2;
        float halfHeight = _spriteRenderer.bounds.size.y / 2;
        return new Vector3(halfWidth, halfHeight);
    }

    public void SetCanBuild(bool canBuild)
    {
        Colorize(canBuild ? _canBuildColor : _cantBuildColor);
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
