using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HUDManager : MonoBehaviour
{
    bool isAROn;
    public void TurnOffAR()
    {

    }

    public void ChangeARState(bool arState)
    {
        isAROn = arState;
    }
}
