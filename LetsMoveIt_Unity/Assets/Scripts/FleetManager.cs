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

    float timer = 0;

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
                Debug.LogWarning(robot + " had finished all the tasks given");
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
                    
                    //Check if robot reached destination
                    if(Vector3.Distance(robot.transform.position, currentTask.moveTo) < 0.5f)
                    {
                        Debug.Log(robot.name + "arrived at: " + currentTask.moveTo);
                        NextTask(robot);
                        continue;
                    }

                    //Generate new path
                    NavMeshPath path = new NavMeshPath();
                    NavMesh.CalculatePath(robot.transform.position, currentTask.moveTo, NavMesh.AllAreas, path);

                    //Check if path is valid
                    if (path.status != NavMeshPathStatus.PathComplete)
                    {
                        //Debug.LogWarning(robot.name + " cannot reacht point " + currentTask.moveTo + " - path is invalid");
                        //NextTask(robot);
                        //continue;
                    }

                    //Debugging
                    DrawPath(robot.transform.position, path.corners);

                    //Move/Rotate robot
                    if (path.corners.Length > 1)
                    {
                        Vector3 nextPos = path.corners[1];
                        nextPos.y = 0;

                        //Calculate movement
                        float distance = Vector3.Distance(robot.transform.position, nextPos);

                        //Calculate rotation
                        Vector3 dir = (nextPos - robot.transform.position).normalized;
                        float angle = Vector3.Angle(robot.transform.forward, dir);

                        float angleToRotate = Mathf.DeltaAngle(angle, 0);
                        
                        if(Vector3.Cross(robot.transform.forward, dir).y > 0)
                        {
                            angleToRotate = -angleToRotate;
                        }
                        
                        foreach (NavMeshLink item in FindObjectsOfType<NavMeshLink>())
                        {
                            Debug.Log(item.name + " - " + (item.transform.position + item.startPoint));

                            Vector3 startPoint = item.startPoint;

                            float dis = Vector3.Distance(item.transform.position + item.startPoint, robot.transform.position);

                            if (dis < 0.2f)
                            {
                                Debug.Log(robot.name + " wants to cross the link");
                            }
                        }

                        //Apply Rotation and Movement
                        if (Mathf.Abs(angleToRotate) > 0.5f)
                        {
                            robot.Rotate(Mathf.Clamp(angleToRotate, -robot.rotateSpeed * Time.deltaTime, robot.rotateSpeed * Time.deltaTime));
                        } else
                        {   
                            robot.Move(Mathf.Clamp(distance, -robot.speed * Time.deltaTime, robot.speed * Time.deltaTime));
                        }
                    }
                    break;
                case TaskOption.wait:
                    if (timer <= 0)
                    {
                        timer = currentTask.waitForSeconds;
                    }

                    timer -= Time.deltaTime;

                    if(timer <= 0)
                    {
                        NextTask(robot);
                        continue;
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
        return robotList[robotController].Peek();
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

    //PathFinding
    //Tasks
    //Move Different Robots
    //Let Robots do stuff
}
