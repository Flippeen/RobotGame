using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SimpleInput : MonoBehaviour
{
    [SerializeField] MovementOptions methodName;

    public void AddToQueue()
    {
        GetComponentInParent<MoveObjectController>().AddToQueue(methodName.ToString());
    }
}
