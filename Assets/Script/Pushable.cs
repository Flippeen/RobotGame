using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;

[RequireComponent(typeof(GridObject))]
public class Pushable : MoveObjectController
{
    public async Task<bool> UseInteractable(Vector3Int objUsing)
    {
        StopAllCoroutines();
        Vector3Int pushDir = GetGridPosition - GridManager.Instance.GetGridObject(objUsing).GetGridPosition;
        if (GridManager.Instance.MoveObject(GetGridPosition, GetGridPosition + pushDir))
        {
            await MoveAnimation(GetGridPosition + pushDir, 5);
            return true;
        }
        else
            return false;
    }

    //Animation -------------------------------------
    async Task MoveAnimation(Vector3Int targetPos, float speed)
    {
        Move(targetPos);
        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.fixedDeltaTime * GameManager.Instance.GetAnimationSpeed);
            await Task.Yield();
        }
        transform.position = targetPos;
        if (GridManager.Instance.GridPosIsNull(GetGridPosition + Vector3Int.down))
            await FallAnimation(GetGridPosition);
    }
    async Task FallAnimation(Vector3Int currentPos)
    {
        Vector3Int nextPos = currentPos + Vector3Int.down;
        if (!GridManager.Instance.MoveObject(GetGridPosition, nextPos))
            return;

        Move(nextPos);
        while (Vector3.Distance(transform.position, nextPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, nextPos, Time.fixedDeltaTime * GameManager.Instance.GetAnimationSpeed);
            await Task.Yield();
        }
        transform.position = nextPos;
        print(GetGridPosition);
        if (GridManager.Instance.GridPosIsNull(GetGridPosition + Vector3Int.down))
            await FallAnimation(GetGridPosition);
    }
}
