using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ActionObjectUI : MonoBehaviour
{
    MovementAction moveAction;
    public MovementAction _moveAction { get { return moveAction; } }
    public void SetMovementAction(MovementAction newMovementAction)
    {
        moveAction = newMovementAction;
    }
    public void RemoveThisAction()
    {
        GetComponentInParent<PlayerMovement>().RemoveFromQueue(moveAction._id);
    }
}
