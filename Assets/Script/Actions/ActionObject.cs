using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActionObject : MonoBehaviour
{
    UnityEvent eventItem;
    double id;
    string actionName;
    public UnityEvent _eventItem {get{return eventItem;}}
    public double _id {get{return id; }}
    public string _actionName { get{return actionName; }}
    public void SetValues(UnityEvent newEvent, string newName, double newId)
    {
        eventItem = newEvent;
        actionName = newName;
        id = newId;
    }
    public void RemoveThisAction()
    {
        GetComponentInParent<PlayerMovement>().RemoveEventToQueue(_id);
    }
}
