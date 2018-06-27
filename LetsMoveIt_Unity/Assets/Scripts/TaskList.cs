using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TaskList {

    [HideInInspector]
    public string name = "";

    public Robot robot = null;
    public Task[] list;
}
