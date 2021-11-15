using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class OldPlayerMovement : GridObject
{
    [SerializeField] GameObject actionItem;
    [SerializeField] GameObject actionsCanvas;
    [SerializeField] TMPro.TMP_Text text;
    [SerializeField] int textOffset;
    Queue<ActionObject> eventQueue = new Queue<ActionObject>();
    double idCounter;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            actionsCanvas.SetActive(false);
            StartNextAction();
        }
        if (transform.position.y <= 0)
            Destroy(gameObject, 0.5f);
    }
    public void AddEventToQueue(UnityEvent ev)
    {
        print("Added event");
        idCounter++;
        GameObject newObj = Instantiate(actionItem);
        ActionObject newActionObj = newObj.GetComponent<ActionObject>();
        newActionObj.SetValues(ev, MethodNameToName(ev.GetPersistentMethodName(0)), idCounter);
        newObj.transform.SetParent(text.transform);
        newObj.gameObject.GetComponent<TMPro.TMP_Text>().text = newActionObj._actionName;
        eventQueue.Enqueue(newActionObj);
        List<ActionObject> actionList = new List<ActionObject>();
        actionList.AddRange(eventQueue.ToArray());
        newObj.transform.localPosition = new Vector3(0,actionList.IndexOf(newActionObj) * -textOffset, 0);
    }
    public void RemoveEventToQueue(double eventId)
    {
        Queue<ActionObject> newQueue = new Queue<ActionObject>();
        foreach (var actionObj in eventQueue)
        {
            if (actionObj._id != eventId)
            {
                newQueue.Enqueue(actionObj);
                List<ActionObject> actionList = new List<ActionObject>();
                actionList.AddRange(newQueue.ToArray());
                actionObj.transform.localPosition = new Vector3(0, actionList.IndexOf(actionObj) * -textOffset, 0);
            }
            else
            {
                Destroy(actionObj.gameObject);
            }
        }
        eventQueue.Clear();
        eventQueue = newQueue;
    }
    void StartNextAction()
    {
        if (eventQueue.Count <= 0 && GridManager.Instance.GridPosIsGoal(GetGridPosition + Vector3Int.down))
        {
            //GridManager.Instance.GetGridObjectOnPosition(GetGridPosition + Vector3Int.down).GetComponent<Goal>().OnGoal();
        }
        if (eventQueue.Count <= 0)
            return;

        eventQueue.Peek()._eventItem.Invoke();
        RemoveEventToQueue(eventQueue.Peek()._id);
    }
    public async void MovePlayer()
    {
        if (GridManager.Instance.MoveObject(GetGridPosition, GetGridPosition + Vector3Int.RoundToInt(transform.right)))
        {
            await MoveAnimation(GetGridPosition + Vector3Int.RoundToInt(transform.right), 1);
            if (GridManager.Instance.GridPosIsNull(GetGridPosition + Vector3Int.down))
                await FallAnimation(GetGridPosition);
        }
        await Task.Delay(10);
        StartNextAction();
    }
    public async void JumpPlayer()
    {
        if(GridManager.Instance.GridPosIsNull(GetGridPosition + Vector3Int.up))
        {
            await MoveAnimation(GetGridPosition + Vector3Int.up + Vector3Int.RoundToInt(transform.right), 1.5f); 
            if (GridManager.Instance.GridPosIsNull(GetGridPosition + Vector3Int.down))
            {
                while(GridManager.Instance.GridPosIsNull(GetGridPosition + Vector3Int.down) && 
                    GridManager.Instance.GridPosIsNull(GetGridPosition + Vector3Int.RoundToInt(transform.right) + Vector3Int.down))
                {
                    if (GridManager.Instance.MoveObject(GetGridPosition, GetGridPosition + Vector3Int.RoundToInt(transform.right) + Vector3Int.down))
                    {
                        await MoveAnimation(GetGridPosition + Vector3Int.RoundToInt(transform.right) + Vector3Int.down, 1f);
                    }
                }
            }
            if (GridManager.Instance.GridPosIsNull(GetGridPosition + Vector3Int.down))
                await FallAnimation(GetGridPosition);
        }
        await Task.Delay(10);
        StartNextAction();
    }
    public async void RotatePlayerLeft()
    {
        Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y - 90, 0);
        while (transform.eulerAngles.y != targetRotation.eulerAngles.y)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 100);
            await Task.Delay(1);
        }
        await Task.Delay(10);
        StartNextAction();
    }
    public async void RotatePlayerRight()
    {
        Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + 90, 0);
        while (transform.eulerAngles.y != targetRotation.eulerAngles.y)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 100);
            await Task.Delay(1);
        }
        await Task.Delay(10);
        StartNextAction();
    }
    //Animations----------------------------------------------------------------------
    async Task MoveAnimation(Vector3Int targetPos, float speed)
    {
        Move(targetPos);
        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.fixedDeltaTime * speed);
            await Task.Delay(1);
        }
        transform.position = targetPos;
    }
    async Task FallAnimation(Vector3Int currentPos)
    {
        Vector3Int targetPos = new Vector3Int(-1,-1,-1);
        while(targetPos.x < 0 && targetPos.y < 0 && targetPos.z < 0)
        {
            Vector3Int newPos = currentPos + Vector3Int.down;
            if (!GridManager.Instance.GridPosIsNull(newPos))
                targetPos = newPos + Vector3Int.up;
            else
                currentPos = newPos;
        }

        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.fixedDeltaTime);
            await Task.Delay(1);
        }
        Move(targetPos);
        transform.position = GetGridPosition;
    }

    string MethodNameToName(string methodName)
    {
        switch (methodName)
        {
            case "RotatePlayerRight":
                return "Rotate Right";
            case "RotatePlayerLeft":
                return "Rotate Left";
            case "MovePlayer":
                return "Move Forward";
            case "JumpPlayer":
                return "Jump";
            default:
                return "NaN";
        }
    }
}
