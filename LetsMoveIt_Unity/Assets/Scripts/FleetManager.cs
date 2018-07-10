﻿using System.Collections.Generic;
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
                    //Check if robots is has to wait for other robots to pass
                    if (robot.waitingForOtherRobot > 0)
                    {
                        robot.carver.enabled = true;
                        continue;
                    }

                    robot.carver.enabled = false;

                    //Check if robot reached destination
                    if(Vector3.Distance(robot.transform.position, currentTask.moveTo) < 0.2f)
                    {
                        robot.path = null;
                        NextTask(robot);
                    }

                    //Check if a new path needs to be calculated
                    if (robot.path == null || robot.path.corners.Length < 1 || Vector3.Distance(robot.transform.position, robot.path.corners[1]) < 0.3f || robot.robotsWaitingForThis > 0)// ||true)
                    {
                        if (robot.name == "EV3_02")
                        {
                            Debug.Log("new path");
                        }
                        CalculateNewPath(robot);
                    }

                    foreach (NavMeshLink item in FindObjectsOfType<NavMeshLink>())
                    {
                        float dis1 = Vector3.Distance(item.transform.position + item.startPoint, robot.transform.position);
                        float dis2 = 0;
                        if(robot.path != null)
                        {
                            dis2 = Vector3.Distance(item.transform.position + item.startPoint, robot.path.corners[1]);
                        } else
                        {
                            dis2 = 1000;
                        }

                        if (dis1 < 0.3f && dis2 < 0.3f)
                        {
                            Debug.Log(robot.name + " wants to cross the link");
                            robot.crossing = true;
                            robot.path.corners[1] = item.transform.position + item.endPoint;
                        }
                    }

                    Vector3 nextPos = Vector3.zero;
                    nextPos = robot.path.corners[1];
                    nextPos.y = 0;

                    //Move/Rotate robot  

                    //Calculate movement
                    float distance = Vector3.Distance(robot.transform.position, nextPos);

                    //Calculate rotation

                    Vector3 dir = nextPos - robot.transform.position;
                    if(dir == Vector3.zero)
                    {
                        if (robot.path.corners.Length > 1)
                        {
                            nextPos = robot.path.corners[2];
                            dir = nextPos - robot.transform.position;
                        }
                    }

                    Quaternion rot = Quaternion.LookRotation(dir);

                    if(robot.name == "EV3_02")
                    {
                        //Debug.Log("test");
                    }
                    /*
                    Vector3 dir = (nextPos - robot.transform.position).normalized;
                    float angle = Vector3.Angle(robot.transform.forward, dir);

                    float angleToRotate = Mathf.DeltaAngle(angle, 0);
                        
                    if(Vector3.Cross(robot.transform.forward, dir).y > 0)
                    {
                        angleToRotate = -angleToRotate;
                    }*/

                    //Apply Rotation and Movement
                    if (rot != robot.transform.rotation)//Mathf.Abs(angle) > 0f)
                    {
                        robot.Rotate(rot);
                        //robot.Rotate(Mathf.Clamp(angle, -robot.rotateSpeed * Time.deltaTime, robot.rotateSpeed * Time.deltaTime));
                    } else
                    {   
                        robot.Move(Mathf.Clamp(distance, -robot.speed * Time.deltaTime, robot.speed * Time.deltaTime));
                    }
                    break;
                case TaskOption.wait:
                    //robot.path = null;
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
        if (path.status != NavMeshPathStatus.PathComplete)
        {
            Debug.LogWarning(robot.name + " cannot reacht point " + currentTask.moveTo + " - path is invalid");
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
