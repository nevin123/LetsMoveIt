using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Robot")]
public class Robot : ScriptableObject {

    public string name;

    public GameObject model;
    public Color robotColor;
    [Space]
    public float speed;
    public float rotateSpeed;
    public float diameter;

    public bool var1;
    public bool var2;
    public bool var3;
    public bool var4;
}
