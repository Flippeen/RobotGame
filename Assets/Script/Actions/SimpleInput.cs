using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class SimpleInput : MonoBehaviour
{
    [SerializeField] UnityEvent myEvent;

    public void AddToQueue()
    {
        GetComponentInParent<PlayerMovement>().AddEventToQueue(myEvent);
    }
}
