using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GameManager : MonoBehaviour
{
    PlayerMovement[] robots;
    RaycastHit hit;
    LevelManager lvlManager;
    Vector3 mouseLocation;
    bool robotsStarted, playerWon;
    private void Awake()
    {
        lvlManager = GetComponent<LevelManager>();
    }
    public void PlayerWon()
    {
        playerWon = true;
        Debug.Log("Player won!!");
        lvlManager.LevelCompleted();
    }
    void Update()
    {
        //if (robotsStarted && !playerWon)
        //{
        //    int robotsDone = 0;
        //    foreach (var robot in robots)
        //    {
        //        if (!robot.isRobotMoving && (robot.GetBatteryValue() <= 0 || robot.GetEventQueueLength() <= 0))
        //        {
        //            robotsDone++;
        //        }
        //    }
        //    if (robotsDone == robots.Length)
        //    {
        //        Invoke("RestartCheck", 1);
        //    }
        //    return;
        //}
        if (Input.GetMouseButtonDown(0))
            mouseLocation = Input.mousePosition;
        if (Input.GetMouseButtonUp(0) && Vector2.Distance(Input.mousePosition, mouseLocation) < 5f)
        {
            if (EventSystem.current.IsPointerOverGameObject())
                return;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100))
            {
                PlayerMovement player = hit.transform.GetComponent<PlayerMovement>();
                if (player != null)
                {
                    player.OnClick();
                }
            }
        }
    }
    void RestartCheck()
    {
        if (!playerWon)
        {
            lvlManager.RestartLevel();
        }
    }

    public void StartRobots()
    {
        robotsStarted = true;
        robots = FindObjectsOfType<PlayerMovement>();
        foreach (PlayerMovement robot in robots)
        {
            robot.StartRobot();
        }
    }

    public bool IsMenuOpen()
    {
        return false;
    }
}
