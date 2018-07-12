using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class GameManager : MonoBehaviour
{
    public GameObject robotParent;
    public Robot[] robots;

    public TaskObject[] taskLists;

    void Start()
    {
        if (Application.isPlaying)
        {
            FleetManager.instance.AddTaskList(taskLists);
            GenerateAllRobots();
        }
    }
    
    void OnApplicationQuit()
    {
        foreach (Transform child in robotParent.transform)
        {
            DestroyImmediate(child.gameObject);
        }
    }

    void GenerateAllRobots()
    {
        foreach (Robot robot in robots)
        {
            GameObject newRobot = Instantiate(robot.model, robotParent.transform);

            //newRobot.SetActive(false);

            RobotController controller = newRobot.AddComponent<RobotController>();
            controller.InitializeRobot(robot);

            FleetManager.instance.AddRobot(controller);
        }
    }

    void Update()
    {
        NameInspector();
    }

    void NameInspector()
    {
        for (int i = 0; i < taskLists.Length; i++)
        {
            taskLists[i].name = i+1 + ". " + taskLists[i].list.Length.ToString() + " steps to complete";

            for (int x = 0; x < taskLists[i].list.Length; x++)
            {
                taskLists[i].list[x].name = "task " + (x + 1).ToString();
            }
        }
    }
}
