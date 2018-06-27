using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    public GameObject robotParent;
    public Robot[] robots;

    public TaskList[] taskLists;

    void Start()
    {
        if(Application.isPlaying)
        GenerateAllRobots();
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

            TaskList list = new TaskList();

            foreach (TaskList taskList in taskLists)
            {
                if(taskList.robot == robot)
                {
                    Debug.Log("bingo");
                    list = taskList;
                }
            }

            FleetManager.instance.AddRobot(controller, list);
        }
    }

    void Update()
    {
        NameInspector();
    }

    void NameInspector()
    {
        foreach (TaskList list in taskLists)
        {
            list.name = (list.robot != null) ? list.robot.name : ("Robot") + " - " + list.list.Length.ToString() + " tasks";

            for (int i = 0; i < list.list.Length; i++)
            {
                list.list[i].name = "task " + (i + 1).ToString();
            }
        }
    }
}
