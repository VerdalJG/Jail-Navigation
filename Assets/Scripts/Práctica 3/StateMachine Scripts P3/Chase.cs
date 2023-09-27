using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Chase : StateMachineBehaviour
{
    private NavMeshAgent agent; // Access to NavMeshAgent Component of each Agent
    public AgentBase agentScript; // Reference to patrol script

    private int layerMask; // Layermask to ignore specific objects on a specific layer            

    private GameObject self;

    public Transform target;

    private float agentInitSpeed; // Agent's initial speed

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //References
        agent = animator.gameObject.GetComponent<NavMeshAgent>();
        agentScript = animator.gameObject.GetComponent<AgentBase>();
        target = agentScript.chaseTarget;

        agent.destination = target.position;
        agent.isStopped = false;

        // Agent initial speed
        agentInitSpeed = 3.5f;

        // Bit shift the index of the layer (8) to get a bit mask
        layerMask = 1 << 8;                                                                         

        //Own GameObject
        self = animator.gameObject;

        agentScript.agentMat.color = Color.red;


    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {

        NavMeshHit navArea;
        agent.SamplePathPosition(-1, 0.0f, out navArea); //Functions like a raycast? Towards the Navmesh and returns a variable of type NavMeshHit.
                                                         //The values of NavMeshHit.mask are of powers of 2. Example = Index = 5 ; navArea.mask = 32


        // Checks if the agent is a guard, and if they are standing in security.
        if (navArea.mask == 32)
        {
            agent.speed = agentInitSpeed * 1.5f;
        }
        else
        {
            agent.speed = agentInitSpeed;
        }

        //Raycast hit info
        RaycastHit hit;

        //Elevated y position ray start to be more in line with eyes
        Vector3 rayStart = new Vector3(self.transform.position.x, self.transform.position.y + 1, self.transform.position.z);

        if (Physics.Raycast(rayStart, self.transform.TransformDirection(Vector3.forward), out hit, 5, layerMask))
        {
            if (hit.transform.gameObject.GetComponent<AgentBase>().type != AgentTypes.Guard)
            {
                NavMeshHit otherAgentArea;
                hit.transform.gameObject.GetComponent<NavMeshAgent>().SamplePathPosition(-1, 0.0f, out otherAgentArea);

                if (otherAgentArea.mask == 32)
                {
                    agent.destination = target.position;
                }
                else
                {
                    animator.SetBool("Chase", false);
                    animator.SetBool("Warning", false);
                    animator.SetBool("Alarm", false);
                }
            }

        }
        
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
