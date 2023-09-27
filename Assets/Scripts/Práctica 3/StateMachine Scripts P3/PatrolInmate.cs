using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/*
    COPY PASTAS

    #region
    #endregion


*/


public class PatrolInmate : StateMachineBehaviour
{
    #region Variables

    private NavMeshAgent agent; // Access to NavMeshAgent Component of each Agent
    public AgentBase agentScript; // Reference to patrol script

    public int destPointNum = 0; // Destination point
    private bool cycleForwards = true; // Cycling of patrol

    private GameObject self; // Reference to own game object

    private Transform cell;

    [SerializeField]
    private Material indicatorMat;

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

        agentScript.agentMat.color = Color.white;
    }


    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
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
