using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolGuard : StateMachineBehaviour
{
    private NavMeshAgent agent; // Access to NavMeshAgent Component of each Agent
    private AgentBase agentScript; // Reference to patrol script

    public int destPointNum = 0; // Destination point
    private bool cycleForwards = true; // Cycling of patrol

    private float agentInitSpeed; // Agent's initial speed
    private int layerMask; // Layermask to ignore specific objects on a specific layer

    private GameObject self; // Reference to own game object

    //[SerializeField]
    private List<GameObject> patrolPoints;

    [SerializeField]
    private Material indicatorMat;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // References
        agent = animator.gameObject.GetComponent<NavMeshAgent>();
        agentScript = animator.gameObject.GetComponent<AgentBase>();
        indicatorMat = animator.gameObject.transform.Find("Sphere").GetComponent<MeshRenderer>().material;

        agent.autoBraking = false;
        agent.isStopped = false;
        agentInitSpeed = 3.5f; // Store agent's initial velocity for later use in security speed boosts.

        // Only get new patrol path if you don't have one.
        if (patrolPoints == null)
        {
            // Get patrol points from AI Manager
            patrolPoints = new List<GameObject>();

            patrolPoints = AIManager.Instance.GetPatrolArea();
        }

        // Warp agent to their patrol area
        //agent.Warp(patrolPoints[0].transform.position);

        // Set the agent to go to the currently selected destination.
        agent.destination = patrolPoints[destPointNum].transform.position;


        //Set Color indicator
        indicatorMat.color = patrolPoints[destPointNum].GetComponent<MeshRenderer>().material.color;

        //Own GameObject
        self = animator.gameObject;

        // Bit shift the index of the layer (8) to get a bit mask
        layerMask = 1 << 8;

        agentScript.agentMat.color = Color.white;
    }


    public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        NavMeshHit navArea;
        agent.SamplePathPosition(-1, 0.0f, out navArea); //Functions like a raycast? Towards the Navmesh and returns a variable of type NavMeshHit.
                                                         //The values of NavMeshHit.mask are of powers of 2. Example = Index = 5 ; navArea.mask = 32

        // Checks if the agent is a guard, and if they are standing in security.
        if (navArea.mask == 32)
        {
            //If they are in security, boost speed by 50 %
            agent.speed = agentInitSpeed * 1.5f;

           
        }
        else if (navArea.mask != 32)
        {
            agent.speed = agentInitSpeed;
        }

        //Raycast hit info
        RaycastHit hit;

        //Elevated y position ray start to be more in line with eyes
        Vector3 rayStart = new Vector3(self.transform.position.x, self.transform.position.y + 1, self.transform.position.z);

        //Raycast
        if (Physics.Raycast(rayStart, self.transform.TransformDirection(Vector3.forward), out hit, 5, layerMask))
        {
            NavMeshHit otherAgentArea;
            hit.transform.gameObject.GetComponent<NavMeshAgent>().SamplePathPosition(-1, 0.0f, out otherAgentArea);
            if (otherAgentArea.mask == 32)
            {
                if (hit.transform.gameObject.GetComponent<AgentBase>().type != AgentTypes.Guard)
                {
                    AIManager.Instance.escapistPos = hit.transform.position; // Store position of the gameobject hit by the ray
                    agent.ResetPath(); // Clear current path
                    animator.SetBool("Warning", true); // State transition
                }
            }
                
        }

        // Choose the next destination point when the agent gets close to the current one.
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            // Returns if no points have been set up
            if (patrolPoints.Count <= 1)
            {
                return;
            }

            // Once the sequence of points have all been used, revert in the other direction (could technically be done with a boolean to improve performance(extremely minor)).
            if (destPointNum == patrolPoints.Count - 1)
            {
                cycleForwards = false;
            }
            else if (destPointNum == 0)
            {
                cycleForwards = true;
            }

            // Choose the next point in the array as the destination, depending on which direction the cycle is going in
            if (cycleForwards)
            {
                destPointNum++;
            }
            else if (!cycleForwards)
            {
                destPointNum--;
            }

            // Set the agent to go to the currently selected destination.
            agent.destination = patrolPoints[destPointNum].transform.position;
        }
    }
}

    


