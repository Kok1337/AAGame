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
    }

    private void SetCameraZPosition(float z)
    {
        Camera.transform.position = new Vector3(Camera.transform.position.x, Camera.transform.position.y, z);
    }

    // Update is called once per frame
    void Update()
    {
        if (Camera != null)
        {
            if (Input.mouseScrollDelta.y > 0)
            {
                float zPosition = Camera.transform.position.z;
                if (zPosition < _maxZoomIn)
                {
                    zPosition += ZoomChangeAmount * Time.deltaTime;
                    if (zPosition > _maxZoomIn)
                    {
                        zPosition = _maxZoomIn;
                    }
                    SetCameraZPosition(zPosition);
                }
            }

            if (Input.mouseScrollDelta.y < 0)
            {
                float zPosition = Camera.transform.position.z;
                if (zPosition > _maxZoomOut)
                {
                    zPosition -= ZoomChangeAmount * Time.deltaTime;
                    if (zPosition < _maxZoomOut)
                    {
                        zPosition = _maxZoomOut;
                    }
                    SetCameraZPosition(zPosition);
                }
            }
        }
    }
}
