using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(GridObject))]
public class Pushable : MonoBehaviour, IInteractable
{
    GridObject gridObj;
    void Awake()
    {
        gridObj = GetComponent<GridObject>();
    }
    public void UseInteractable(PlayerMovement player)
    {
        print("Going: " + transform.name);
        Vector3Int pushDir = gridObj.GetGridPosition - player.GetGridPosition;
        MoveCrate(pushDir);
    }

    void MoveCrate(Vector3Int direction)
    {
        if(GridManager.Instance.MoveObject(gridObj.GetGridPosition, gridObj.GetGridPosition + direction))
        {
            //PlayerMovement playerAbove = GridManager.Instance.GetGridObjectOnPosition(GetGridPosition + Vector3Int.up)?.GetComponent<PlayerMovement>();
            //if (playerAbove != null)
            //    playerAbove.FloorRemoved();

            StartCoroutine(MoveAnimation(gridObj.GetGridPosition + direction, 5));
        }
    }

    //Animation -------------------------------------
    IEnumerator MoveAnimation(Vector3Int targetPos, float speed)
    {
        gridObj.Move(targetPos);
        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * speed);
            //transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.fixedDeltaTime * speed);
            yield return new WaitForSeconds(0.01f);
        }
        transform.position = targetPos;
        if (GridManager.Instance.GridPosIsNull(gridObj.GetGridPosition + Vector3Int.down))
            StartCoroutine(FallAnimation(gridObj.GetGridPosition));
    }
    IEnumerator FallAnimation(Vector3Int currentPos)
    {
        Vector3Int targetPos = new Vector3Int(-1, -1, -1);
        while (targetPos.x < 0 && targetPos.y < 0 && targetPos.z < 0)
        {
            Vector3Int newPos = currentPos + Vector3Int.down;
            if (!GridManager.Instance.GridPosIsNull(newPos))
                targetPos = newPos + Vector3Int.up;
            else
                currentPos = newPos;
        }

        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * 5);
            GridManager.Instance.MoveObject(gridObj.GetGridPosition, Vector3Int.RoundToInt(transform.position));
            yield return new WaitForSeconds(0.01f);
        }
        gridObj.Move(targetPos);
        transform.position = gridObj.GetGridPosition;
        yield return new WaitForSeconds(0.1f);
    }
}
