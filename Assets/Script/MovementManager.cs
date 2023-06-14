using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.Events;
using System;
using System.Linq;

public enum MovementOptions { Nothing, MoveObject, FallObject, JumpObject, RotateObjectRight, RotateObjectLeft, WaitObject}
public class MovementManager : MonoBehaviour
{
    Dictionary<double, Task> tasks = new Dictionary<double, Task>();

    UnityEvent allMovesDone = new UnityEvent();
    static MovementManager _instance;
    public static MovementManager Instance { get { return _instance; } }
    double taskID;
    public bool stopCode;
    private void Awake()
    {
        if (Instance == null)
            _instance = this;
        else
            Destroy(gameObject);
    }
    public void AddListning(UnityAction startNextMove)
    {
        allMovesDone.AddListener(startNextMove);
    }
    public double AddTask(Task taskToAdd)
    {
        taskID++;
        tasks.Add(taskID, taskToAdd);
        return taskID;
    }
    public void ChangeTask(double id, Task newTask)
    {
        tasks[id] = newTask;
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
            BeginCode();
    }

    public async void BeginCode()
    {
        if (!Application.isPlaying || stopCode)
            return;

        await Task.WhenAll(tasks.Values);
        tasks.Clear();
        allMovesDone.Invoke();
        await Task.Yield();
        BeginCode();
    }
    public void StopMovementCode()
    {
        stopCode = true;
    }
    public Dictionary<double, Task> GetTasks()
    {
        return tasks;
    }
}
