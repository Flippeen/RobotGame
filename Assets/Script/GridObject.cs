using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridObject : MonoBehaviour
{
    public CatalogueEnums objectID;
    public Vector3Int GetGridPosition { get { return gridPosition; } }
    [Header("Grid Object")]
    [SerializeField] Vector3Int gridPosition;
    protected bool canMove;
    void Start()
    {
        SetPosition(new Vector3Int((int)Mathf.Round(transform.position.x), (int)Mathf.Round(transform.position.y), (int)Mathf.Round(transform.position.z)));
    }

    public void SetPosition(Vector3Int positionToSet)
    {
        gridPosition = positionToSet;
        transform.position = gridPosition;

        GridManager.Instance.SetStartPosition(this);
    }

    public void Move(Vector3Int newPos)
    {
        gridPosition = newPos;
    }
}
