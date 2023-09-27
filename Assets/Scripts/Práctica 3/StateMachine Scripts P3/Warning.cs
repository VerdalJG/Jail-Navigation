using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Warning : StateMachineBehaviour
{
    private NavMeshAgent agent; // Access to NavMeshAgent Component of each Agent

    private float agentInitSpeed; // Agent's initial speed

    public AgentBase agentScript; // Reference to patrol script

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent = animator.gameObject.GetComponent<NavMeshAgent>();

        agentInitSpeed = 3.5f; // Store agent's initial velocity for later use in security speed boosts.

        agent.destination = AIManager.Instance.GetNearestPhone(animator.gameObject.transform.position);

        agentScript = animator.gameObject.GetComponent<AgentBase>();

        agentScript.agentMat.color = Color.yellow;
        agent.isStopped = false;

    }


    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
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

        if (!agent.pathPending && agent.remainingDistance < 1f)
        {
            AIManager.Instance.AlarmState();
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
