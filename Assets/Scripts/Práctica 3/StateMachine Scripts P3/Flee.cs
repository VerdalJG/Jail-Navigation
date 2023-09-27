using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Flee : StateMachineBehaviour
{
    private NavMeshAgent agent; // Access to NavMeshAgent Component of each Agent
    public AgentBase agentScript; // Reference to patrol script

    private float agentInitSpeed = 3.5f; // Agent's initial speed

    private float timer;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent = animator.gameObject.GetComponent<NavMeshAgent>();
        agentScript = animator.gameObject.GetComponent<AgentBase>();

        agent.ResetPath();
        agent.destination = agentScript.fleeTarget; // Run away from the seen agent - calculated in patrol 
        
        //If they are fleeing, boost speed by 50 %
        agent.speed = agentInitSpeed * 1.5f;

        timer = 0f;

        agentScript.agentMat.color = Color.red;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer += Time.deltaTime;
        if (timer >= 5f)
        {
            animator.SetBool("Fleeing", false);
            animator.SetBool("Checking", true);
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //If exiting state
        agent.speed = agentInitSpeed;

        timer = 0f;
    }



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
