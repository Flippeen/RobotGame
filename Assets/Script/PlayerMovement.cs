using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlayerMovement : GridObject
{
    [Header("Input List Settings")]
    [SerializeField] GameObject actionItem;
    [SerializeField] GameObject actionsCanvas;
    [SerializeField] Canvas gameHUD;
    [SerializeField] TMPro.TMP_Text text;
    [SerializeField] int textOffset;

    [Header("Battery Settings")]
    [SerializeField] TMPro.TMP_Text batteryText;
    [SerializeField] Image batteryImage;
    [SerializeField] int batteryMaxValue;
    [SerializeField] Gradient gradient;
    int batteryValueUI, batteryValue;

    Queue<ActionObject> eventQueue = new Queue<ActionObject>();
    double idCounter;
    bool isMoving;
    public bool isRobotMoving { get { return isMoving; } }
    void Awake()
    {
        Invoke("LateStart", 0.001f);
    }
    void LateStart()
    {
        actionsCanvas = GetComponentInChildren<Canvas>().gameObject;
        actionsCanvas.SetActive(false);
        batteryValueUI = batteryMaxValue;
        batteryValue = batteryMaxValue;
        batteryText.text = batteryValueUI.ToString();
    }
    void Update()
    {
        if (transform.position.y <= 0)
        {
            FindObjectOfType<LevelManager>().RestartLevel();
            Destroy(gameObject, 0.5f);
        }
    }
    public void StartRobot()
    {
        actionsCanvas.SetActive(false);
        StartNextAction();
    }
    public void AddEventToQueue(UnityEvent ev)
    {
        idCounter++;
        GameObject newObj = Instantiate(actionItem);
        ActionObject newActionObj = newObj.GetComponent<ActionObject>();
        newActionObj.SetValues(ev, MethodNameToName(ev.GetPersistentMethodName(0)), idCounter);
        newObj.transform.SetParent(text.transform);
        newObj.gameObject.GetComponent<TMPro.TMP_Text>().text = newActionObj._actionName;
        eventQueue.Enqueue(newActionObj);
        List<ActionObject> actionList = new List<ActionObject>();
        actionList.AddRange(eventQueue.ToArray());
        newObj.transform.localPosition = new Vector3(0, actionList.IndexOf(newActionObj) * -textOffset, 0);
        text.rectTransform.sizeDelta = new Vector2(0, actionList.Count * textOffset);

        if(newActionObj._actionName != "Wait")
            ChangeBatteryValueUI(-1);
    }
    public void RemoveEventToQueue(double eventId)
    {
        string eventName = "";
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
                eventName = actionObj._actionName;
                Destroy(actionObj.gameObject);
            }
        }
        text.rectTransform.sizeDelta = new Vector2(0, 100 + newQueue.Count * textOffset);
        eventQueue.Clear();
        eventQueue = newQueue;

        if (eventName != "Wait")
            ChangeBatteryValueUI(1);
    }
    void StartNextAction()
    {
        isMoving = false;
        if (eventQueue.Count <= 0 && GridManager.Instance.GridPosIsGoal(GetGridPosition + Vector3Int.down))
        {
            //GridManager.Instance.GetGridObjectOnPosition(GetGridPosition + Vector3Int.down).GetComponent<Goal>().OnGoal();
        }
        if (eventQueue.Count <= 0 || batteryValue <= 0)
            return;

        isMoving = true;
        var evItem = eventQueue.Dequeue();
        if (evItem._actionName != "Wait")
            batteryValue--;

        evItem._eventItem.Invoke();
    }
    public void MovePlayer()
    {
        if (GridManager.Instance.MoveObject(GetGridPosition, GetGridPosition + Vector3Int.RoundToInt(transform.right)))
        {
            PlayerMovement playerAbove = GridManager.Instance.GetGridObjectOnPosition(GetGridPosition + Vector3Int.up)?.GetComponent<PlayerMovement>();
            if (playerAbove != null)
                playerAbove.FloorRemoved();

            StartCoroutine(MoveAnimation(GetGridPosition + Vector3Int.RoundToInt(transform.right), 5));
            //StartCoroutine(MoveAnimation(GetGridPosition + Vector3Int.RoundToInt(transform.right), 2));
        }
        else
        {
            StartNextAction();
        }
    }
    public void JumpPlayer()
    {
        if (GridManager.Instance.GridPosIsNull(GetGridPosition + Vector3Int.up) && GridManager.Instance.MoveObject(GetGridPosition, GetGridPosition + Vector3Int.up + Vector3Int.RoundToInt(transform.right)))
        {
            StartCoroutine(MoveAnimation(GetGridPosition + Vector3Int.up + Vector3Int.RoundToInt(transform.right), 5));
            //StartCoroutine(MoveAnimation(GetGridPosition + Vector3Int.up + Vector3Int.RoundToInt(transform.right), 2.5f));
        }
        else
        {
            StartNextAction();
        }
    }
    public void RotatePlayerRight()
    {
        StartCoroutine(RotateRightAnimation());
    }
    public void RotatePlayerLeft()
    {
        StartCoroutine(RotateLeftAnimation());
    }

    public void WaitPlayer()
    {
        StartCoroutine(WaitPlayerAnimation());
    }
    public void FloorRemoved()
    {
        if (eventQueue.Count <= 0)
            StartCoroutine(FallAnimation(GetGridPosition));
    }
    //Animations---------------------------------------
    IEnumerator WaitPlayerAnimation()
    {
        yield return new WaitForSeconds(0.35f);
        if (GridManager.Instance.GridPosIsNull(GetGridPosition + Vector3Int.down))
            StartCoroutine(FallAnimation(GetGridPosition));
        else
        {
            yield return new WaitForSeconds(0.2f);
            StartNextAction();
        }
    }
    IEnumerator RotateLeftAnimation()
    {
        Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y - 90, 0);
        float timerStart = Time.time;
        while (transform.eulerAngles.y != targetRotation.eulerAngles.y)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 280);
            yield return new WaitForFixedUpdate();
        }
        if (GridManager.Instance.GridPosIsNull(GetGridPosition + Vector3Int.down))
            StartCoroutine(FallAnimation(GetGridPosition));
        else
        {
            yield return new WaitForSeconds(0.2f);
            StartNextAction();
        }
    }
    IEnumerator RotateRightAnimation()
    {
        Quaternion targetRotation = Quaternion.Euler(0, transform.eulerAngles.y + 90, 0);
        while (transform.eulerAngles.y != targetRotation.eulerAngles.y)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 280);
            yield return new WaitForFixedUpdate();
        }
        if (GridManager.Instance.GridPosIsNull(GetGridPosition + Vector3Int.down))
            StartCoroutine(FallAnimation(GetGridPosition));
        else
        {
            yield return new WaitForSeconds(0.2f);
            StartNextAction();
        }
    }
    IEnumerator MoveAnimation(Vector3Int targetPos, float speed)
    {
        Move(targetPos);
        while (Vector3.Distance(transform.position, targetPos) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * speed);
            yield return new WaitForSeconds(0.01f);
        }
        transform.position = targetPos;
        if (GridManager.Instance.GridPosIsNull(GetGridPosition + Vector3Int.down))
            StartCoroutine(FallAnimation(GetGridPosition));
        else
        {
            yield return new WaitForSeconds(0.2f);
            StartNextAction();
        }
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
            //transform.position = Vector3.MoveTowards(transform.position, targetPos, Time.deltaTime * 2);
            GridManager.Instance.MoveObject(GetGridPosition, Vector3Int.RoundToInt(transform.position));
            yield return new WaitForSeconds(0.01f);
        }
        Move(targetPos);
        transform.position = GetGridPosition;
        yield return new WaitForSeconds(0.1f);
        StartNextAction();
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
            case "WaitPlayer":
                return "Wait";
            default:
                return "NaN";
        }
    }

    void ChangeBatteryValueUI(int newValue)
    {
        batteryValueUI += newValue;
        batteryText.text = batteryValueUI.ToString();
        float precentValue = (float)batteryValueUI / (float)batteryMaxValue;
        batteryImage.color = gradient.Evaluate(precentValue);
    }
    public void ChangeBatteryValue(int newValue)
    {
        batteryValue = newValue;
    }
    public int GetBatteryValue()
    {
        return batteryValue;
    }
    public int GetEventQueueLength()
    {
        return eventQueue.Count;
    }

    public void OnClick()
    {
        actionsCanvas.SetActive(!actionsCanvas.activeSelf);
        gameHUD.gameObject.SetActive(false);
    }
}
