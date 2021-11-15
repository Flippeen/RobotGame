using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour, IInteractable
{
    public void UseInteractable(PlayerMovement player)
    {
        FindObjectOfType<GameManager>().Invoke("PlayerWon", 1);
    }
}
