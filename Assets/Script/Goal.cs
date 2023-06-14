using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour, IInteractable
{
    public bool UseInteractable(Vector3Int objUsing)
    {
        if (GridManager.Instance.GetGridObject(objUsing).GetComponent<PlayerMovement>() == null)
            return false;

        FindObjectOfType<GameManager>().Invoke("PlayerWon", 1);
        return true;
    }
}
