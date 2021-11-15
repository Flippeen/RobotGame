using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour
{
    public Vector3Int GetGridPosition { get { return gridPosition; } }
    [Header("Grid Object")]
    [SerializeField] Vector3Int gridPosition;
    protected bool canMove;
    void Start()
    {
        gridPosition = new Vector3Int((int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y), (int)Mathf.Round(transform.position.z));
        transform.position = gridPosition;

        GridManager.Instance.SetStartPosition(this);
    }

    public void Move(Vector3Int newPos)
    {
        gridPosition = newPos;
    }
}
