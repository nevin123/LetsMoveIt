using UnityEngine;

[System.Serializable]
public class TaskPart
{
    [HideInInspector]
    public string name;

    public TaskOption task;

    //Start
    public Vector3 newPosition = Vector3.zero;

    //Move
    public Vector3 moveTo = Vector3.zero;

    //Wait
    public float waitForSeconds = 1f;
}

public enum TaskOption
{
    teleport,
    move,
    wait
}
