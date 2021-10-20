using GridSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAndGridTest : MonoBehaviour
{
    public Camera Camera;

    private CustomGrid<bool> _grid;

    void Start()
    {
        _grid = new CustomGrid<bool>(10, 10, 1f, transform, (CustomGrid<bool> g, int x, int y) => false);
        // _grid = new CustomGrid<int>(10, 10, 1f, transform, (CustomGrid<int> g, int x, int y) => 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                _grid.SetValue(GetWorldMousePosition(), !_grid.GetValue(GetWorldMousePosition()));
                // Debug.Log(_grid.GetValue(GetWorldMousePosition()));
            }

            if (Input.GetMouseButtonDown(1))
            {
                // _grid.SetValue(GetWorldMousePosition(), 50);
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
