using UnityEngine;
using UnityEngine.AI;

public class RobotController : MonoBehaviour {

    public Vector3 positionToMoveTo;

    public float speed = 0;
    public float rotateSpeed = 0;

    float timer = 0;

    public void InitializeRobot(Robot robotValues)
    {
        speed = robotValues.speed;
        rotateSpeed = robotValues.rotateSpeed;

        gameObject.name = robotValues.name;
        gameObject.transform.localScale = new Vector3(robotValues.diameter, 1, robotValues.diameter);

        foreach (Material mat in gameObject.transform.GetChild(0).GetComponent<Renderer>().materials)
        {
            if (mat.name.Contains("color"))
            {
                mat.color = robotValues.robotColor;

                mat.SetColor("_EmissionColor", robotValues.robotColor * 6f);
            }
        }
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

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer != LayerMask.NameToLayer("Floor") && Time.time > 0.1f)
        {
            Debug.LogError(gameObject.name + " collided with " + col.gameObject.name + ". make sure this doesnt happen again!");
        }
    }

    public bool Wait(float time)
    {
        if(timer == 0)
        {
            timer = time;
            return false;
        }

        timer -= Time.deltaTime;

        if(timer <= 0)
        {
            timer = 0;
            return true;
        } else
        {
            return false;
        }
    }
}
