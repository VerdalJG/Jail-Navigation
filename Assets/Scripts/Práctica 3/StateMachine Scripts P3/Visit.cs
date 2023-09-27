using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Visit : StateMachineBehaviour
{

    #region Variables

    private NavMeshAgent agent; // Access to NavMeshAgent Component of each Agent
    public AgentBase agentScript; // Reference to patrol script

    public int destPointNum = 0; // Destination point
    private bool cycleForwards = true; // Cycling of patrol

    private GameObject self; // Reference to own game object

    private Transform cell;
    private int layerMask; // Layermask to ignore specific objects on a specific layer

    [SerializeField]
    private Material indicatorMat;

    private AIManager aiScript;

    public GameObject dummy;

    #endregion

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // References
        agent = animator.gameObject.GetComponent<NavMeshAgent>();
        agentScript = animator.gameObject.GetComponent<AgentBase>();
        indicatorMat = animator.gameObject.transform.Find("Sphere").GetComponent<MeshRenderer>().material;

        agent.autoBraking = false;

        // Set the agent to go to the currently selected destination.
        agent.destination = agentScript.points[destPointNum].transform.position;
        agent.isStopped = false;

        // Set Color indicator
        indicatorMat.color = agentScript.points[destPointNum].GetComponent<MeshRenderer>().material.color;

        // Own GameObject
        self = animator.gameObject;

        // Bit shift the index of the layer (8) to get a bit mask
        layerMask = 1 << 8;

        agentScript.agentMat.color = Color.white;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
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
                agentScript.fleeTarget = (self.transform.position + agentScript.dir.normalized * 5);
                Instantiate(dummy, agentScript.fleeTarget, Quaternion.identity);
                animator.SetBool("Fleeing", true);
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
    }

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
