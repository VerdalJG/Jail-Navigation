using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AlarmVisitor : StateMachineBehaviour
{
    public AgentBase agentScript; // Reference to patrol script

    private NavMeshAgent agent; // Access to NavMeshAgent Component of each Agent

    public Transform exit; // Reference to exit


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // References 
        agentScript = animator.gameObject.GetComponent<AgentBase>();
        agent = animator.gameObject.GetComponent<NavMeshAgent>();

        exit = GameObject.Find("Exit").transform;

        agent.destination = exit.position;

        agentScript.agentMat.color = Color.blue;
    }


    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Stop agent once at exit, using a remaining distance of 3, so that the agents have some space to stack up (if there were to be multiple visitors)
        if (!agent.pathPending && agent.remainingDistance < 3f)
        {
            agent.isStopped = true;
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
