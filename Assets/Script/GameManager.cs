using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] float animationSpeed;
    public float GetAnimationSpeed { get { return animationSpeed; } }
    static GameManager _instance;
    public static GameManager Instance { get { return _instance; } }

    PlayerMovement[] robots;
    int robotCounter;
    RaycastHit hit;
    LevelManager lvlManager;
    Vector3 mouseLocation;
    bool robotsStarted, playerWon;
    private void Awake()
    {
        if (Instance == null)
            _instance = this;
        else
            Destroy(gameObject);

        robots = FindObjectsOfType<PlayerMovement>();
        robotCounter = 2;
        lvlManager = GetComponent<LevelManager>();
    }
    public void PlayerWon()
    {
        playerWon = true;
        Debug.Log("Player won!!");
        GetComponent<MovementManager>().StopMovementCode();
        lvlManager.LevelCompleted();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            mouseLocation = Input.mousePosition;
        if (Input.GetMouseButtonUp(0) && Vector2.Distance(Input.mousePosition, mouseLocation) < 5f)
        {
            if (EventSystem.current.IsPointerOverGameObject() || IsPointerOverUIObject() ||  robotsStarted)
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
    public void RobotDied()
    {
        print(robotCounter);
        robotCounter--;
        if (robotCounter <= 0)
        {
            lvlManager.RestartLevel();
        }
    }

    public void StartRobots()
    {
        robotsStarted = true;
        MovementManager.Instance.BeginCode();
        //robots = FindObjectsOfType<PlayerMovement>();
        //foreach (PlayerMovement robot in robots)
        //{
        //    robot.StartRobot();
        //}
    }
    bool isMenuOpen;
    public bool IsMenuOpen { get { return isMenuOpen; } }
    public void ChangeMenuState(bool menuIsOpen)
    {
        isMenuOpen = menuIsOpen;
    }
    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
