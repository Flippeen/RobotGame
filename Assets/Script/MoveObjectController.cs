using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;

public class MoveObjectController : GridObject
{
    [SerializeField] bool isPushable = true;
    protected Queue<MovementAction> movementQueue = new Queue<MovementAction>();
    protected double idCounter, taskID;
    protected Task currentTask;
    protected virtual void Awake()
    {
        Invoke("LateStart", 0.001f);
    }
    void LateStart()
    {
        MovementManager.Instance.AddListning(StartNextAction);
    }
    public virtual void AddToQueue(string methodName, Vector3Int direction = default(Vector3Int))
    {
        idCounter++;
        MovementAction newMovementAction = new MovementAction();
        newMovementAction.SetValues(methodName, MethodNameToName(methodName), idCounter, direction);
        movementQueue.Enqueue(newMovementAction);
    }
    public virtual void RemoveFromQueue(double eventId)
    {
        Queue<MovementAction> newQueue = new Queue<MovementAction>();
        foreach (var actionObj in movementQueue)
        {
            if (actionObj._id != eventId)
                newQueue.Enqueue(actionObj);
        }
        movementQueue.Clear();
        movementQueue = newQueue;
    }
    public virtual void AddFirstToQueue(string methodName, Vector3Int direction = default(Vector3Int))
    {
        idCounter++;
        MovementAction newMovementAction = new MovementAction();
        newMovementAction.SetValues(methodName, MethodNameToName(methodName), idCounter, direction);

        List<MovementAction> actionObjList = movementQueue.ToList();
        actionObjList.Insert(0, newMovementAction);
        movementQueue.Clear();
        for (int i = 0; i < actionObjList.Count; i++)
        {
            movementQueue.Enqueue(actionObjList[i]);
        }
    }
    protected virtual void ChangeTask(Task newTask)
    {
        MovementManager.Instance.ChangeTask(taskID, newTask);
    }
    public virtual void StartNextAction()
    {
        if (movementQueue.Count <= 0)
            return;
        
        var evItem = movementQueue.Dequeue();
        Task newTask = MoveSelector(evItem._actionName, evItem._direction);
        currentTask = newTask;
        taskID = MovementManager.Instance.AddTask(newTask);
    }
    protected async Task MoveSelector(string methodName, Vector3Int direction = default(Vector3Int))
    {
        switch (methodName)
        {
            case "RotateObjectRight":
                await RotateObjectRight();
                break;
            case "RotateObjectLeft":
                await RotateObjectLeft();
                break;
            case "MoveObject":
                await MoveObject(direction);
                break;
            case "JumpObject":
                await JumpObject();
                break;
            case "WaitObject":
                await WaitObject();
                break;
            case "FallObject":
                await FallObject();
                break;
            default:
                break;
        }
    }
    //Push Functionallity-----------------------------------------------------------------
    public bool Pushable(Vector3Int pushersPosition)
    {
        if (!isPushable)
            return false;

        Vector3Int pushDir = GetGridPosition - pushersPosition;
        if (GridManager.Instance.MoveObject(GetGridPosition, GetGridPosition + pushDir))
        {
            //AddFirstToQueue("MoveObject", pushDir);
            //targetPosG = GetGridPosition + pushDir;

            //ChangeTask(MoveAnimation(GetGridPosition + pushDir));
            Task newTask = MoveAnimation(GetGridPosition + pushDir);
            currentTask = newTask;
            MovementManager.Instance.AddTask(newTask);
            return true;
        }
        else
            return false;
    }
    //Movement----------------------------------------------------------------------------
    protected async Task MoveObject(Vector3Int direction = default(Vector3Int))
    {
        if (direction == Vector3Int.zero)
            direction = Vector3Int.RoundToInt(transform.right);
        //if (GridManager.Instance.MoveObject(GetGridPosition, GetGridPosition + Vector3Int.RoundToInt(transform.right)))
        if (GridManager.Instance.MoveObject(GetGridPosition, GetGridPosition + direction))
        {
            MoveObjectController objectAbove = GridManager.Instance.GetGridObject(GetGridPosition + Vector3Int.up)?.GetComponent<MoveObjectController>();
            if (objectAbove != null)
                objectAbove.FloorRemoved();
            
            await MoveAnimation(GetGridPosition + direction);
        }
    }
    protected async Task FallObject()
    {
        if (GridManager.Instance.MoveObject(GetGridPosition, GetGridPosition + Vector3Int.down))
        {
            MoveObjectController objectAbove = GridManager.Instance.GetGridObject(GetGridPosition + Vector3Int.up)?.GetComponent<MoveObjectController>();
            if (objectAbove != null)
                objectAbove.FloorRemoved();

            await FallAnimation(GetGridPosition + Vector3Int.down);
        }
    }
    protected async Task JumpObject()
    {
        if (GridManager.Instance.GridPosIsNull(GetGridPosition + Vector3Int.up) && GridManager.Instance.MoveObject(GetGridPosition, GetGridPosition + Vector3Int.up + Vector3Int.RoundToInt(transform.right)))
        {
            await MoveAnimation(GetGridPosition + Vector3Int.up + Vector3Int.RoundToInt(transform.right));
        }
        else if (GridManager.Instance.GridPosIsNull(GetGridPosition + Vector3Int.up) && GridManager.Instance.MoveObject(GetGridPosition, GetGridPosition + Vector3Int.up))
            await MoveAnimation(GetGridPosition + Vector3Int.up);
    }
    protected async Task RotateObjectRight()
    {
        await RotateRightAnimation();
    }
    protected async Task RotateObjectLeft()
    {
        await RotateLeftAnimation();
    }

    protected async Task WaitObject()
    {
        await WaitAnimation();
    }
    protected void FloorRemoved()
    {
        AddFirstToQueue("FallObject");
    }
    //Animations---------------------------------------
    async Task WaitAnimation()
    {
        await Task.Delay(200);
        if (GridManager.Instance.GridPosIsNull(GetGridPosition + Vector3Int.down))
            AddFirstToQueue("FallObject");
    }
    async Task RotateLeftAnimation()
    {
        Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y - 90, 0);
        while (transform.eulerAngles.y != targetRotation.eulerAngles.y)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * GameManager.Instance.GetAnimationSpeed * 125);
            await Task.Yield();
        }
        if (GridManager.Instance.GridPosIsNull(GetGridPosition + Vector3Int.down))
            AddFirstToQueue("FallObject");
    }
    async Task RotateRightAnimation()
    {
        Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + 90, 0);
        while (transform.eulerAngles.y != targetRotation.eulerAngles.y)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * GameManager.Instance.GetAnimationSpeed * 125);
            await Task.Yield();
        }
        if (GridManager.Instance.GridPosIsNull(GetGridPosition + Vector3Int.down))
            AddFirstToQueue("FallObject");
    }
    async Task MoveAnimation(Vector3Int targetPos)
    {
        Move(targetPos);
        if(currentTask != null)
            await currentTask;
        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * GameManager.Instance.GetAnimationSpeed);
            await Task.Yield();
        }
        transform.position = targetPos;
        if (GridManager.Instance.GridPosIsNull(GetGridPosition + Vector3Int.down))
            AddFirstToQueue("FallObject");
    }
    async Task FallAnimation(Vector3Int targetPos)
    {
        Move(targetPos);
        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * GameManager.Instance.GetAnimationSpeed);
            await Task.Yield();
        }
        transform.position = targetPos;
        if (GridManager.Instance.GridPosIsNull(GetGridPosition + Vector3Int.down))
            AddFirstToQueue("FallObject");
    }
    protected string MethodNameToName(string methodName)
    {
        switch (methodName)
        {
            case "RotateObjectRight":
                return "Rotate Right";
            case "RotateObjectLeft":
                return "Rotate Left";
            case "MoveObject":
                return "Move Forward";
            case "JumpObject":
                return "Jump";
            case "WaitObject":
                return "Wait";
            case "FallObject":
                return "Fall";
            default:
                return "NaN";
        }
    }

    public virtual void Died()
    {
        Destroy(gameObject, 0.1f);
    }
}
