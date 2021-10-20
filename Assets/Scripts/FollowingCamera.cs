using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowingCamera : MonoBehaviour
{
    public Transform TargetTransform;
    public float Smooth;
    // Start is called before the first frame update
    void Start()
    {
        if (TargetTransform == null)
        {
            Debug.LogError("÷ель не задана");
        }
    }

    private void LateUpdate()
    {
        if (transform.position != TargetTransform?.position)
        {
            Vector3 targetPosition = new Vector3(TargetTransform.position.x, TargetTransform.position.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPosition, Smooth);
        }
    }
}
