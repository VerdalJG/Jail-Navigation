using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Check : StateMachineBehaviour
{
    private NavMeshAgent agent; // Access to NavMeshAgent Component of each Agent
    public AgentBase agentScript; // Reference to patrol script

    private float rotateSpeed = 72f;

    private GameObject self;

    private int layerMask; // Layermask to ignore specific objects on a specific layer - ignoring doors

    private float agentInitSpeed = 3.5f; // Agent's initial speed

    private float timer;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //References
        agent = animator.gameObject.GetComponent<NavMeshAgent>();
        agentScript = animator.gameObject.GetComponent<AgentBase>();
        self = animator.gameObject;

        // Bit shift the index of the layer (8) to get a bit mask
        layerMask = 1 << 8;

        agent.speed = 0;
        timer = 0f;

        agentScript.agentMat.color = Color.yellow;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.transform.Rotate(new Vector3(0, 1, 0) * rotateSpeed * Time.deltaTime);
        timer += Time.deltaTime;
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
                animator.SetBool("Fleeing", true);
                animator.SetBool("Checking", false);
            }
        }
        else
        {
            if (timer >= 3.6f)
            {
                animator.SetBool("Fleeing", false);
                animator.SetBool("Checking", false);
            }
        }
    }

    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
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
