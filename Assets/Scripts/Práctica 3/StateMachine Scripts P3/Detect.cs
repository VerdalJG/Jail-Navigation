using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Detect : StateMachineBehaviour
{
    private NavMeshAgent agent; // Access to NavMeshAgent Component of each Agent
    public AgentBase agentScript; // Reference to patrol script

    private int layerMask; // Layermask to ignore specific objects on a specific layer

    private GameObject self;

    private float agentInitSpeed; // Agent's initial speed

    private float timer;

    private float rotateSpeed = 90f;

    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // References
        agent = animator.gameObject.GetComponent<NavMeshAgent>();
        agentScript = animator.gameObject.GetComponent<AgentBase>();

        // Agent initial speed
        agentInitSpeed = 3.5f;

        agent.isStopped = false;

        // Reset path on start
        agent.ResetPath();

        // Set destination to where the escapist was found
        agent.destination = AIManager.Instance.escapistPos;

        // Bit shift the index of the layer (8) to get a bit mask
        layerMask = 1 << 8;

        // Own GameObject
        self = animator.gameObject;

        // Color of state
        agentScript.agentMat.color = Color.blue;

        // Set timer to 0
        timer = 0;
        
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

        if (!agent.pathPending && agent.remainingDistance < 3f)
        {
            agent.isStopped = true;

            if (agent.velocity == Vector3.zero)
            {
                timer += Time.deltaTime;

                self.transform.Rotate(new Vector3(0, 1, 0) * rotateSpeed * Time.deltaTime);

                //Raycast hit info
                RaycastHit hit;

                //Elevated y position ray start to be more in line with eyes
                Vector3 rayStart = new Vector3(self.transform.position.x, self.transform.position.y + 1, self.transform.position.z);

                if (Physics.Raycast(rayStart, self.transform.TransformDirection(Vector3.forward), out hit, 5, layerMask))
                {
                    NavMeshHit otherAgentArea;
                    hit.transform.gameObject.GetComponent<NavMeshAgent>().SamplePathPosition(-1, 0.0f, out otherAgentArea);
                    if (otherAgentArea.mask == 32)
                    {
                        if (hit.transform.gameObject.GetComponent<AgentBase>().type != AgentTypes.Guard && hit.transform.gameObject.GetComponent<AgentBase>().type != AgentTypes.Visitor)
                        {
                            agentScript.chaseTarget = hit.transform.gameObject.transform;
                            timer = 0f;
                            agent.ResetPath();
                            animator.SetBool("Chase", true);
                        }
                    }
                }
                else
                {
                    if (timer >= 4f)
                    {
                        timer = 0f;
                        animator.SetBool("Warning", false);
                        animator.SetBool("Alarm", false);
                    }
                }
            }

        }
        
    }
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        timer = 0;
    }
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
