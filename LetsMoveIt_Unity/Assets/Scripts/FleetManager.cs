using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FleetManager : MonoBehaviour {

    #region Singleton

    public static FleetManager instance;

    private void Awake()
    {
        instance = this;
    }

    #endregion

    List<RobotController> robots;
    Dictionary<RobotController, Queue<Task>> robotList;

    void Start()
    {
        robots = new List<RobotController>();
        robotList = new Dictionary<RobotController, Queue<Task>>();
    }

    void Update()
    {
        foreach (RobotController robot in robots)
        {
            //No more tasks to do
            if(robotList[robot].Count <= 0)
            {
                continue;
            }

            Task currentTask = CurrentTask(robot);

            switch (currentTask.task)
            {
                case TaskOption.teleport:
                    robot.Teleport(currentTask.newPosition);
                    NextTask(robot);
                    break;
                case TaskOption.move:
                    Debug.Log("test " + robot.name);
                    break;
                case TaskOption.wait:
                    break;
            }

            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(robot.transform.position, new Vector3(0, 0, 0), 1, path);

            for (int i = 0; i < path.corners.Length; i++)
            {
                if (i == 0)
                {
                    Debug.DrawLine(robot.transform.position, path.corners[i], Color.red);
                }
                else
                {
                    Debug.DrawLine(path.corners[i - 1], path.corners[i], Color.red);
                }
            }
        }
    }

    public void AddRobot(RobotController robotController, TaskList taskList)
    {
        Queue<Task> newTaskList = new Queue<Task>();
        
        foreach (Task task in taskList.list)
        {
            newTaskList.Enqueue(task);
        }

        robots.Add(robotController);
        robotList.Add(robotController, newTaskList);
    }

    Task CurrentTask(RobotController robotController)
    {
        return robotList[robotController].Peek();
    }

    void NextTask(RobotController robotController)
    {
        robotList[robotController].Dequeue();
    }



    //PathFinding
    //Tasks
    //Move Different Robots
    //Let Robots do stuff
}
