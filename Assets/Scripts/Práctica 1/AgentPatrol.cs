using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class AgentPatrol : MonoBehaviour
{
    public GameObject[] points; // Array to store waypoints
    public int destPoint = 0; // Index for waypoints
    private NavMeshAgent agent; // Access to NavMeshAgent Component of each Agent
    public AgentTypes type; // Type of Agent
    private string cycleDirection = "Forwards"; // Cycling of patrol
    private float agentInitSpeed; // Agent's initial speed

    [SerializeField]
    private Material indicatorMat;


    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

        // Disabling auto-braking allows for continuous movement
        // between points (ie, the agent doesn't slow down as it
        // approaches a destination point).
        agent.autoBraking = false;

        agentInitSpeed = agent.speed; // Store agent's initial velocity for later use in security speed boosts.

        indicatorMat = gameObject.transform.Find("Sphere").GetComponent<MeshRenderer>().material;
        points = GameObject.FindGameObjectsWithTag("Waypoint"); //Store waypoints in array
        // Set the agent to go to the currently selected destination.
        agent.ResetPath();
        agent.destination = points[destPoint].transform.position;
        agent.isStopped = false; 

        //Set Color indicator
        indicatorMat.color = points[destPoint].GetComponent<MeshRenderer>().material.color;
    }


    void GotoNextPoint()
    {
        // Returns if no points have been set up
        if (points.Length == 0)
        {
            return;
        }

        // Once the sequence of points have all been used, revert in the other direction (could technically be done with a boolean to improve performance(extremely minor)).
        if (destPoint == points.Length - 1)
        {
            cycleDirection = "Backwards";
        }
        else if (destPoint == 0)
        {
            cycleDirection = "Forwards";
        }

        // Choose the next point in the array as the destination, depending on which direction the cycle is going in
        if (cycleDirection == "Forwards")
        {
            destPoint++;
        }
        else if (cycleDirection == "Backwards")
        {
            destPoint--;
        }

        // Set the agent to go to the currently selected destination.
        agent.destination = points[destPoint].transform.position;

        //Set Color indicator
        indicatorMat.color = points[destPoint].GetComponent<MeshRenderer>().material.color;

    }


    void Update()
    {

        // Choose the next destination point when the agent gets close to the current one.
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            GotoNextPoint();
        }

        NavMeshHit navArea;
        agent.SamplePathPosition(-1, 0.0f, out navArea); //Functions like a raycast? Towards the Navmesh and returns a variable of type NavMeshHit.
                                                         //The values of NavMeshHit.mask are of powers of 2. Example = Index = 5 ; navArea.mask = 32

        // Checks if the agent is a guard, and if they are standing in security. If they are in security, boost speed by 50%
        if (type == AgentTypes.Guard && navArea.mask == 32)
        {
            agent.speed = agentInitSpeed * 1.5f;
        }
        else if (type == AgentTypes.Guard && navArea.mask != 32)
        {
            agent.speed = agentInitSpeed;
        }

        if (!agent.autoTraverseOffMeshLink)
        {
            if (agent.isOnOffMeshLink)
            {
                OffMeshLink currentOML = agent.currentOffMeshLinkData.offMeshLink;
                Door doorScript = currentOML.gameObject.GetComponentInParent<Door>();
                if (currentOML.area == 3) // Index 3 is Prison doors
                {
                    if (type == AgentTypes.Inmate)
                    {
                        if (doorScript.inmateValid/*currentOML.gameObject.tag == "Toilet Door"*/)
                        {
                            agent.CompleteOffMeshLink();
                        }
                        else
                        {
                            agent.ResetPath();
                        }
                    }

                    else if (type == AgentTypes.Guard)
                    {
                        if (doorScript.guardValid)
                        {
                            agent.CompleteOffMeshLink();
                        }
                        else
                        {
                            agent.ResetPath();
                        }
                    }
                    
                }
                else if (currentOML.area == 4) // Index 4 is Visit doors
                {
                    agent.CompleteOffMeshLink();
                }
                else if (currentOML.area == 5) // Index 5 is Security doors
                {
                    if (type == AgentTypes.Guard)
                    {
                        agent.CompleteOffMeshLink();
                    }
                    else
                    {
                        agent.ResetPath();
                    }
                
                }
            }
        }
    }



}
