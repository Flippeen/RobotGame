using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class MovingArrows : MonoBehaviour
{
    [SerializeField] GameObject arrow;
    List<GameObject> spawnedArrows;
    LevelCreationController lvlCre;
    GameObject newArrow;
    private void Awake()
    {
        lvlCre = FindObjectOfType<LevelCreationController>();
        newArrow = Instantiate(arrow, Vector3.zero, Quaternion.identity);
        newArrow.GetComponent<Canvas>().worldCamera = Camera.main;
    }
    bool tester = false;
    private void Update()
    {
        if (lvlCre.objectToMove == null)
            return;

        Vector3 originPos = lvlCre.objectToMove.position;
        newArrow.transform.position = originPos + Vector3.up;
        newArrow.transform.LookAt(Camera.main.transform);
    }
    //private void OnDrawGizmos()
    //{
    //    if (lvlCre.objectToMove == null)
    //        return;

    //    Vector3 originPos = lvlCre.objectToMove.position;
    //    float sphereSize = 0.5f;

    //    Gizmos.color = Color.red;
    //    Gizmos.DrawSphere(originPos + Vector3.up, sphereSize);
    //    Gizmos.DrawSphere(originPos + Vector3.down, sphereSize);
    //    Gizmos.DrawSphere(originPos + Vector3.right, sphereSize);
    //    Gizmos.DrawSphere(originPos + Vector3.left, sphereSize);
    //    Gizmos.DrawSphere(originPos + Vector3.forward, sphereSize);
    //    Gizmos.DrawSphere(originPos + Vector3.back, sphereSize);
    //}
}
