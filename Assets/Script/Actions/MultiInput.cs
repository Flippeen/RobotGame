using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MultiInput : MonoBehaviour
{
    [SerializeField] MovementOptions methodName;
    TMPro.TMP_InputField stepsIF;
    void Start()
    {
        stepsIF = GetComponentInChildren<TMPro.TMP_InputField>();
    }

    public void Action()
    {
        if (stepsIF.text == "" || int.Parse(stepsIF.text) <= 0)
        {
            Debug.LogError("Please input a number bigger than 0");
            return;
        }

        for (int i = 0; i < int.Parse(stepsIF.text); i++)
        {
            FindObjectOfType<PlayerMovement>().AddToQueue(methodName.ToString());
        }
    }
}
