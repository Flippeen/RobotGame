using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Linq;

public class PlayerMovement : MoveObjectController
{
    [Header("Input List Settings")]
    [SerializeField] GameObject actionItem;
    [SerializeField] GameObject actionsCanvas;
    //[SerializeField] Canvas gameHUD;
    [SerializeField] TMPro.TMP_Text text;
    [SerializeField] int textOffset;

    [Header("Battery Settings")]
    [SerializeField] TMPro.TMP_Text batteryText;
    [SerializeField] Image batteryImage;
    [SerializeField] int batteryMaxValue;
    [SerializeField] Gradient gradient;
    int batteryValueUI, batteryValue;

    protected Queue<ActionObjectUI> actionQueueObj = new Queue<ActionObjectUI>();
    bool isMoving;
    public bool isRobotMoving { get { return isMoving; } }
    protected override void Awake()
    {
        Invoke("LateStart", 0.001f);
    }
    void LateStart()
    {
        //actionsCanvas = GetComponentInChildren<Canvas>().gameObject;
        actionsCanvas.SetActive(false);
        batteryValueUI = batteryMaxValue;
        batteryValue = batteryMaxValue;
        batteryText.text = batteryValueUI.ToString();
        MovementManager.Instance.AddListning(StartNextAction);
    }
    public override void Died()
    {
        GameManager.Instance.RobotDied();
        Destroy(gameObject, 0.1f);
    }
    public void StartRobot()
    {
        actionsCanvas.SetActive(false);
        StartNextAction();
    }
    public override void AddToQueue(string methodName, Vector3Int direction = default(Vector3Int))
    {
        idCounter++;

        MovementAction newMovementAction = new MovementAction();
        newMovementAction.SetValues(methodName, MethodNameToName(methodName), idCounter, direction);
        movementQueue.Enqueue(newMovementAction);

        GameObject newObj = Instantiate(actionItem);
        ActionObjectUI newActionObj = newObj.GetComponent<ActionObjectUI>();
        newActionObj.SetMovementAction(newMovementAction);
        newObj.transform.SetParent(text.transform);
        newObj.gameObject.GetComponent<TMPro.TMP_Text>().text = newActionObj._moveAction._displayName;
        actionQueueObj.Enqueue(newActionObj);
        List<ActionObjectUI> actionList = new List<ActionObjectUI>();
        actionList.AddRange(actionQueueObj.ToArray());
        newObj.transform.localPosition = new Vector3(0, actionList.IndexOf(newActionObj) * -textOffset, 0);
        text.rectTransform.sizeDelta = new Vector2(0, actionList.Count * textOffset);

        if(newActionObj._moveAction._actionName != "WaitObject" && newActionObj._moveAction._actionName != "FallObject")
            ChangeBatteryValueUI(-1);
    }
    //public override void AddFirstToQueue(string methodName)
    //{
    //    idCounter++;
    //    GameObject newObj = Instantiate(actionItem);
    //    ActionObject newActionObj = newObj.GetComponent<ActionObject>();
    //    newActionObj.SetValues(methodName, MethodNameToName(methodName), idCounter);
    //    newObj.transform.SetParent(text.transform);
    //    newObj.gameObject.GetComponent<TMPro.TMP_Text>().text = newActionObj._actionName;

    //    List<ActionObject> actionObjList = movementQueue.ToList();
    //    actionObjList.Insert(0, newActionObj);
    //    movementQueue.Clear();
    //    for (int i = 0; i < actionObjList.Count; i++)
    //    {
    //        movementQueue.Enqueue(actionObjList[i]);
    //    }

    //    List<ActionObject> actionList = new List<ActionObject>();
    //    actionList.AddRange(movementQueue.ToArray());
    //    newObj.transform.localPosition = new Vector3(0, actionList.IndexOf(newActionObj) * -textOffset, 0);
    //    text.rectTransform.sizeDelta = new Vector2(0, actionList.Count * textOffset);
    //}
    public override void RemoveFromQueue(double eventId)
    {
        string eventName = "";
        Queue<ActionObjectUI> newQueue = new Queue<ActionObjectUI>();
        Queue<MovementAction> newMovementQueue = new Queue<MovementAction>();
        foreach (var actionObj in actionQueueObj)
        {
            if (actionObj._moveAction._id != eventId)
            {
                newQueue.Enqueue(actionObj);
                newMovementQueue.Enqueue(actionObj._moveAction);
                List<ActionObjectUI> actionList = new List<ActionObjectUI>();
                actionList.AddRange(newQueue.ToArray());
                actionObj.transform.localPosition = new Vector3(0, actionList.IndexOf(actionObj) * -textOffset, 0);
            }
            else
            {
                eventName = actionObj._moveAction._actionName;
                Destroy(actionObj.gameObject);
            }
        }
        text.rectTransform.sizeDelta = new Vector2(0, 100 + newQueue.Count * textOffset);
        actionQueueObj.Clear();
        actionQueueObj = newQueue;
        movementQueue = newMovementQueue;

        if (eventName != "WaitObject" && eventName != "FallObject")
            ChangeBatteryValueUI(1);
    }
    public override void StartNextAction()
    {
        isMoving = false;
        if (movementQueue.Count <= 0 || batteryValue <= 0 && movementQueue.Peek()._actionName != "FallObject")
            return;

        isMoving = true;
        var evItem = movementQueue.Dequeue();
        print(evItem._actionName);
        if (evItem._actionName != "WaitObject" && evItem._actionName != "FallObject")
            batteryValue--;

        Task newTask = MoveSelector(evItem._actionName, evItem._direction);
        currentTask = newTask;
        taskID = MovementManager.Instance.AddTask(newTask);
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
        return actionQueueObj.Count;
    }

    public void OnClick()
    {
        print("onclick");
        actionsCanvas.SetActive(!actionsCanvas.activeSelf);
        //gameHUD.gameObject.SetActive(false);
    }
}
