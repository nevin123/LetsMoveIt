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

    NavMeshLink[] allPathLinks;

    float timer = 0;

    void Start()
    {
        robots = new List<RobotController>();
        robotList = new Dictionary<RobotController, Queue<Task>>();
        allPathLinks = FindObjectsOfType<NavMeshLink>();
    }

    void Update()
    {
        foreach (RobotController robot in robots)
        {
            //No more tasks to do
            if(robotList[robot].Count <= 0)
            {
                Debug.LogWarning(robot + " had finished all the tasks given");
                continue;
            }

            Task currentTask = CurrentTask(robot);

            switch (currentTask.task)
            {
                case TaskOption.teleport:
                    robot.carver.enabled = true;
                    robot.Teleport(currentTask.newPosition);
                    NextTask(robot);
                    break;
                case TaskOption.move:
                    //Check if robots is has to wait for other robots to pass
                    if (robot.waitingForOtherRobot > 0)
                    {
                        robot.carver.enabled = true;
                        continue;
                    }

                    robot.carver.enabled = false;

                    //Check if robot reached destination
                    if(Vector3.Distance(robot.transform.position, currentTask.moveTo) < 0.3f)
                    {
                        robot.carver.enabled = true;
                        robot.path = null;
                        NextTask(robot);
                    }

                    //Check if a new path needs to be calculated
                    if (robot.path == null || robot.path.corners.Length <= 1 || robot.robotsWaitingForThis > 0 || Vector3.Distance(robot.transform.position, robot.path.corners[1]) < 0.3f)// ||true)
                    {
                        if(CalculateNewPath(robot) == false)
                        {
                            continue;
                        }
                    }

                    //Check if path is valid
                    if (robot.path == null || robot.path.corners.Length <= 1)
                    {
                        continue;
                    }

                    //Set position to move towards
                    Vector3 nextPos = robot.path.corners[1];
                    nextPos.y = 0;

                    //Move/Rotate robot  
                    //Calculate movement
                    float distance = Vector3.Distance(robot.transform.position, nextPos);

                    //Calculate rotation
                    Vector3 dir = nextPos - robot.transform.position;
                    Quaternion rot = Quaternion.LookRotation(dir);

                    //Apply Rotation and Movement
                    if (rot != robot.transform.rotation)
                    {
                        robot.Rotate(rot);
                    } else
                    {   
                        robot.Move(Mathf.Clamp(distance, -robot.speed * Time.deltaTime, robot.speed * Time.deltaTime));
                    }
                    break;
                case TaskOption.wait:
                    robot.carver.enabled = true;

                    if(robot.Wait(currentTask.waitForSeconds))
                    {
                        NextTask(robot);
                    }

                    break;
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
        if (robotList[robotController].Count > 0)
        {
            return robotList[robotController].Peek();
        }

        return null;
    }

    void NextTask(RobotController robotController)
    {
        robotList[robotController].Dequeue();
    }

    void DrawPath(Vector3 start, Vector3[] path)
    {
        for (int i = 0; i < path.Length; i++)
        {
            if (i > 0)
            {
                Debug.DrawLine(path[i - 1], path[i], Color.red);
            }
        }
    }

    public bool GetRobotPriority(RobotController robot1, RobotController robot2)
    {
        if (robots.IndexOf(robot1) > robots.IndexOf(robot2) || robot2.path == null || robot2.carver.enabled == true)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool CalculateNewPath(RobotController robot)
    {
        Task currentTask = CurrentTask(robot);

        if(currentTask == null)
        {
            return false;
        }

        robot.GetComponent<NavMeshObstacle>().enabled = false;
        //Generate new path
        NavMeshPath path = new NavMeshPath();
        NavMesh.CalculatePath(robot.transform.position, currentTask.moveTo, NavMesh.AllAreas, path);

        //Check if path is valid
        if (path.status == NavMeshPathStatus.PathInvalid)
        {
            //Debug.LogWarning(robot.name + " cannot reacht point " + currentTask.moveTo + " - path is invalid");
            return false;
        }

        if (path.corners.Length > 1)
        {
            robot.path = path;
        }

        //Debugging
        DrawPath(robot.transform.position, path.corners);

        return true;
    }
}
