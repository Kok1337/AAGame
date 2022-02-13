using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMouseController : MonoBehaviour
{
    public Camera Camera;

    public float DefaultZPozition = -10f;
    public float ZoomInCoef = 2.0f;
    public float ZoomOutCoef = 2.0f;
    public float ZoomChangeAmount = 50;

    private float _maxZoomIn;
    private float _maxZoomOut;


    [SerializeField]
    public float ZoomSmootch = 0.025f;
    private float _currZPosition;

    void Start()
    {
        _maxZoomIn = DefaultZPozition / ZoomInCoef;
        _maxZoomOut = DefaultZPozition * ZoomOutCoef;

        Camera = Camera ?? GetComponent<Camera>();
        if (Camera == null)
        {
            Debug.LogError("Камера не задана");
        }
        else
        {
            CameraSetup();
        }
    }

    private void CameraSetup()
    {
        Camera.orthographic = false;
        SetCameraZPosition(DefaultZPozition);
        _currZPosition = DefaultZPozition;
    }

    private void SetCameraZPosition(float z)
    {
        Camera.transform.position = new Vector3(Camera.transform.position.x, Camera.transform.position.y, Mathf.MoveTowards(Camera.transform.position.z, z, ZoomSmootch));
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera != null)
        {
            if (Input.mouseScrollDelta.y > 0)
            {
                if (_currZPosition < _maxZoomIn)
                {
                    _currZPosition += ZoomChangeAmount * Time.deltaTime;
                    if (_currZPosition > _maxZoomIn)
                    {
                        _currZPosition = _maxZoomIn;
                    }
                }
            }

            if (Input.mouseScrollDelta.y < 0)
            {
                if (_currZPosition > _maxZoomOut)
                {
                    _currZPosition -= ZoomChangeAmount * Time.deltaTime;
                    if (_currZPosition < _maxZoomOut)
                    {
                        _currZPosition = _maxZoomOut;
                    }
                    
                }
            }

            if (_currZPosition != Camera.transform.position.z)
            {
                SetCameraZPosition(_currZPosition);
            }
        }
    }
}