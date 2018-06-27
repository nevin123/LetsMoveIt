using UnityEngine;

[System.Serializable]
public class Task
{
    [HideInInspector]
    public string name;

    public TaskOption task;

    //Start
    public Vector3 startPosition = Vector3.zero;

    //Move
    public Vector3 moveTo = Vector3.zero;

    //Wait
    public float waitForSeconds = 1f;
}

public enum TaskOption
{
    start,
    move,
    wait
}
