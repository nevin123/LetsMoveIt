using UnityEngine;

[System.Serializable]
[CreateAssetMenu(fileName = "New Robot")]
public class Robot : ScriptableObject {

    public string name;
    [Space]
    public GameObject model;
    public Color robotColor;
    [Space]
    public float speed;
    public float rotateSpeed;
    public float diameter;
}
