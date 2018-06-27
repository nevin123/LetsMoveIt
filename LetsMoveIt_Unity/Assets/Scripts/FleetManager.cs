using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleetManager : MonoBehaviour {

    #region Singleton

    public static FleetManager instance;

    private void Awake()
    {
        instance = this;
    }

    #endregion

    public Dictionary<RobotController, Queue<Task>> robotList;

    void Start()
    {
        robotList = new Dictionary<RobotController, Queue<Task>>();
    }

    public void Test()
    {
        Debug.Log("test");
    }

    public void AddRobot(RobotController robotController, TaskList taskList)
    {
        Queue<Task> newTaskList = new Queue<Task>();
        
        foreach (Task task in taskList.list)
        {
            newTaskList.Enqueue(task);
        }

        robotList.Add(robotController, newTaskList);

        DebugRobot(robotController);
    }

    void DebugRobot(RobotController robotController)
    {
        Debug.Log(robotList[robotController].Peek().task);
    }



    //PathFinding
    //Tasks
    //Move Different Robots
    //Let Robots do stuff
}
