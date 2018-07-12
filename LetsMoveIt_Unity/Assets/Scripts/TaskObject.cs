using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TaskObject
{
    [HideInInspector]
    public string name = "";
    public TaskPart[] list;

    public bool var1;
    public bool var2;
    public bool var3;
    public bool var4;
}
