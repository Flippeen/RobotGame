using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAction
{
    double id;
    string actionName;
    string displayName;
    Vector3Int direction;
    public double _id { get { return id; } }
    public string _actionName { get { return actionName; } }
    public string _displayName { get { return displayName; } }
    public Vector3Int _direction { get { return direction; } }
    public void SetValues(string newActionName, string newDisplayName, double newId, Vector3Int newDirection = default(Vector3Int))
    {
        actionName = newActionName;
        displayName = newDisplayName;
        id = newId;
        direction = newDirection;
    }
}
