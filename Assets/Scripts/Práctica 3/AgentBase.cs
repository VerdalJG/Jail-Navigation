using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AgentTypes
{
    Inmate, Escapist, Guard, Visitor
}

public class AgentBase : MonoBehaviour
{
    public GameObject[] points; // Array to store waypoints
    public AgentTypes type;
    public Vector3 dir;

    public Vector3 fleeTarget;
    public Transform chaseTarget;

    public Material agentMat;

    void Start()
    {
        //points = GameObject.FindGameObjectsWithTag("Waypoint"); //Store waypoints in array - Old

        agentMat = transform.Find("Mesh").GetComponent<MeshRenderer>().material;

        //Store waypoints based on agent

        if (type == AgentTypes.Inmate)
        {
            points = GameObject.FindGameObjectsWithTag("InmateWaypoint");
        }

        else if (type == AgentTypes.Visitor)
        {
            points = GameObject.FindGameObjectsWithTag("VisitorWaypoint");
        }

        else if (type == AgentTypes.Escapist)
        {
            points = GameObject.FindGameObjectsWithTag("EscapistWaypoint");
        }
    }
}
