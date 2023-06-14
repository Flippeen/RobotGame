using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARCore;
using UnityEngine.XR;

public class ARControlScript : MonoBehaviour
{
    ARSession arSession;
    [SerializeField] int yOffset;
    void Start()
    {
        arSession = GetComponent<ARSession>();
        transform.position = (GridManager.Instance.GetGridSize() - Vector3.one) / 2;
        transform.position -= Vector3.up * yOffset;
    }

    public void ResetSession()
    {
        arSession.Reset();
        transform.position = (GridManager.Instance.GetGridSize() - Vector3.one) / 2;
        transform.position -= Vector3.up * yOffset;
    }
}
