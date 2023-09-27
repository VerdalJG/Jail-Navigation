using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PatrolBehavior : StateMachineBehaviour
{
    public AgentTypes type; // Type of Agent

    private NavMeshAgent agent; // Access to NavMeshAgent Component of each Agent
    public AgentBase agentScript; // Reference to patrol script

    public int destPointNum = 0; // Destination point
    private bool cycleForwards = true; // Cycling of patrol

    private float agentInitSpeed; // Agent's initial speed
    private int layerMask; // Layermask to ignore specific objects on a specific layer

    private GameObject self; // Reference to own game object

    public GameObject dummy;

    [SerializeField]
    private Material indicatorMat;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //References
        agent = animator.gameObject.GetComponent<NavMeshAgent>();
        agentScript = animator.gameObject.GetComponent<AgentBase>();
        indicatorMat = animator.gameObject.transform.Find("Sphere").GetComponent<MeshRenderer>().material;

        agent.autoBraking = false;
        agentInitSpeed = 3.5f; // Store agent's initial velocity for later use in security speed boosts.

        // Set the agent to go to the currently selected destination.
        agent.destination = agentScript.points[destPointNum].transform.position;
        agent.isStopped = false;

        //Set Color indicator
        indicatorMat.color = agentScript.points[destPointNum].GetComponent<MeshRenderer>().material.color;

        //Own GameObject
        self = animator.gameObject;

        // Bit shift the index of the layer (8) to get a bit mask
        layerMask = 1 << 8;

        agentScript.agentMat.color = Color.white;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (type == AgentTypes.Guard)
        {
            NavMeshHit navArea;
            agent.SamplePathPosition(-1, 0.0f, out navArea); //Functions like a raycast? Towards the Navmesh and returns a variable of type NavMeshHit.
                                                             //The values of NavMeshHit.mask are of powers of 2. Example = Index = 5 ; navArea.mask = 32

            // Checks if the agent is a guard, and if they are standing in security.
            if (navArea.mask == 32)
            {
                //If they are in security, boost speed by 50 %
                agent.speed = agentInitSpeed * 1.5f;

                //Raycast hit info
                RaycastHit hit;

                //Elevated y position ray start to be more in line with eyes
                Vector3 rayStart = new Vector3(self.transform.position.x, self.transform.position.y + 1, self.transform.position.z);

                //Raycast
                if (Physics.Raycast(rayStart, self.transform.TransformDirection(Vector3.forward), out hit, 5, layerMask))
                {
                    if (hit.transform.gameObject.GetComponent<AgentBase>().type != AgentTypes.Guard)
                    {
                        //agentScript.target = hit.transform.position; // Store position of the gameobject hit by the ray
                        agent.ResetPath(); // Clear current path
                        //agent.destination = agentScript.target; // Set new target to that position
                        animator.SetBool("Detecting", true); // State transition
                    }
                }
            }
            else if (navArea.mask != 32)
            {
                agent.speed = agentInitSpeed;
            }
        }

        else if (type == AgentTypes.Visitor)
        {
            //Raycast hit info
            RaycastHit hit;

            //Elevated y position ray start to be more in line with eyes
            Vector3 rayStart = new Vector3(self.transform.position.x, self.transform.position.y + 1, self.transform.position.z);

            if (Physics.Raycast(rayStart, self.transform.TransformDirection(Vector3.forward), out hit, 5, layerMask))
            {
                if (hit.transform.gameObject.GetComponent<AgentBase>().type != AgentTypes.Guard && hit.transform.gameObject.GetComponent<AgentBase>().type != AgentTypes.Visitor)
                {
                    agentScript.dir = (self.transform.position - hit.transform.position);
                    //agentScript.target = (self.transform.position + agentScript.dir.normalized * 5);
                    //Instantiate(dummy, agentScript.target, Quaternion.identity);
                    animator.SetBool("Fleeing", true);
                }
            }
        }

        // Choose the next destination point when the agent gets close to the current one.
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            // Returns if no points have been set up
            if (agentScript.points.Length == 0)
            {
                return;
            }

            // Once the sequence of points have all been used, revert in the other direction (could technically be done with a boolean to improve performance(extremely minor)).
            if (destPointNum == agentScript.points.Length - 1)
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
            agent.destination = agentScript.points[destPointNum].transform.position;

            //Set Color indicator
            indicatorMat.color = agentScript.points[destPointNum].GetComponent<MeshRenderer>().material.color;
        }

        //if (!agent.autoTraverseOffMeshLink)
        //{
        //    if (agent.isOnOffMeshLink)
        //    {
        //        OffMeshLink currentOML = agent.currentOffMeshLinkData.offMeshLink;
        //        Door doorScript = currentOML.gameObject.GetComponentInParent<Door>();
        //        if (currentOML.area == 3) // Index 3 is Prison doors
        //        {
        //            if (type == AgentTypes.Inmate)
        //            {
        //                if (doorScript.inmateValid/*currentOML.gameObject.tag == "Toilet Door"*/)
        //                {
        //                    agent.CompleteOffMeshLink();
        //                }
        //                else
        //                {
        //                    agent.ResetPath();
        //                }
        //            }

        //            else if (type == AgentTypes.Guard)
        //            {
        //                if (doorScript.guardValid)
        //                {
        //                    agent.CompleteOffMeshLink();
        //                }
        //                else
        //                {
        //                    agent.ResetPath();
        //                }
        //            }

        //        }
        //        else if (currentOML.area == 4) // Index 4 is Visit doors
        //        {
        //            agent.CompleteOffMeshLink();
        //        }
        //        else if (currentOML.area == 5) // Index 5 is Security doors
        //        {
        //            if (type == AgentTypes.Guard)
        //            {
        //                agent.CompleteOffMeshLink();
        //            }
        //            else
        //            {
        //                agent.ResetPath();
        //            }

        //        }
        //    }
        //}

    }



    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    //override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
