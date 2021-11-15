using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLookAt : MonoBehaviour
{
    Vector3 centerPoint;
    void Start()
    {
        centerPoint = (GridManager.Instance.GetGridSize() - Vector3.one) / 2;
    }

    void LateUpdate()
    {
        transform.rotation = Quaternion.LookRotation(centerPoint - transform.position);
    }
}
