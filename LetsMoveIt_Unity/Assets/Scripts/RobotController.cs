﻿using UnityEngine;
using UnityEngine.AI;

public class RobotController : MonoBehaviour {

    public NavMeshPath path;

    //public Vector3 positionToMoveTo;
    public int waitingForOtherRobot = 0;
    public int robotsWaitingForThis = 0;

    public float speed = 0;
    public float rotateSpeed = 0;

    float timer = 0;

    public NavMeshObstacle carver;

    public void InitializeRobot(Robot robotValues)
    {
        speed = robotValues.speed;
        rotateSpeed = robotValues.rotateSpeed;

        gameObject.name = robotValues.name;
        gameObject.transform.localScale = new Vector3(robotValues.diameter, 1, robotValues.diameter);

        carver = GetComponent<NavMeshObstacle>();
        carver.enabled = true;

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

    public void Rotate(Quaternion rot)
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, rotateSpeed * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.layer != LayerMask.NameToLayer("Floor") && Time.time > 0.1f)
        {
            Debug.LogError(gameObject.name + " collided with " + col.gameObject.name + ". make sure this doesnt happen again!");
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.layer != LayerMask.NameToLayer("Floor") && Time.time > 0.1f)
        {
            if(col.GetComponent<RobotController>())
            {
                if(FleetManager.instance.GetRobotPriority(this, col.GetComponent<RobotController>()))
                {
                    robotsWaitingForThis++;
                    
                } else
                {
                    waitingForOtherRobot++;
                }
            }
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (col.gameObject.layer != LayerMask.NameToLayer("Floor") && Time.time > 0.1f)
        {
            if (col.GetComponent<RobotController>())
            {
                if (FleetManager.instance.GetRobotPriority(this, col.GetComponent<RobotController>()))
                {
                    robotsWaitingForThis --;
                    robotsWaitingForThis = Mathf.Clamp(robotsWaitingForThis, 0, int.MaxValue);
                } else
                {
                    waitingForOtherRobot--;
                    waitingForOtherRobot = Mathf.Clamp(waitingForOtherRobot, 0, int.MaxValue);
                }
            }
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
