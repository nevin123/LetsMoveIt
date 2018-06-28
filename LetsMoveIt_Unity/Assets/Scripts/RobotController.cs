using UnityEngine;
using UnityEngine.AI;

public class RobotController : MonoBehaviour {

    public float speed = 0;
    public float rotateSpeed = 0;

    public void InitializeRobot(Robot robotValues)
    {
        speed = robotValues.speed;
        rotateSpeed = robotValues.rotateSpeed;

        gameObject.name = robotValues.name;
        gameObject.transform.localScale = new Vector3(robotValues.diameter, 1, robotValues.diameter);
        gameObject.transform.GetChild(0).GetComponent<Renderer>().material.color = robotValues.robotColor;
    }

    public void Teleport(Vector3 position)
    {
        transform.position = position;
    }

    public void Move(float speed)
    {
        transform.Translate(Vector3.forward * speed);
    }

    public void Rotate(float deg)
    {
        transform.Rotate(new Vector3(0,deg,0));
    }
}
