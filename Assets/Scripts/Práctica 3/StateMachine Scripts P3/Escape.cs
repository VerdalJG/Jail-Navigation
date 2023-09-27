using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Escape : StateMachineBehaviour
{
    private NavMeshAgent agent; // Access to NavMeshAgent Component of each Agent
    public AgentBase agentScript;

    public int destPointNum = 0; // Destination point

    private int layerMask; // Layermask to ignore specific objects on a specific layer

    private GameObject self; // Reference to own game object

    [SerializeField]
    private Material indicatorMat;


    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //References
        agent = animator.gameObject.GetComponent<NavMeshAgent>();
        agentScript = animator.gameObject.GetComponent<AgentBase>();

        indicatorMat = animator.gameObject.transform.Find("Sphere").GetComponent<MeshRenderer>().material;

        agent.autoBraking = false;

        //Own GameObject
        self = animator.gameObject;

        //Bit shift the index of the layer(8) to get a bit mask
        layerMask = 1 << 8;

        destPointNum = 0; // Destination point set to 0 to begin escape

        // Set the agent to go to the currently selected destination.
        agent.destination = agentScript.points[destPointNum].transform.position;

        //Set Color indicator
        indicatorMat.color = agentScript.points[destPointNum].GetComponent<MeshRenderer>().material.color;

        agentScript.agentMat.color = Color.white;
    }

    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        NavMeshHit navArea;
        agent.SamplePathPosition(-1, 0.0f, out navArea); //Functions like a raycast? Towards the Navmesh and returns a variable of type NavMeshHit.
                                                         //The values of NavMeshHit.mask are of powers of 2. Example = Index = 5 ; navArea.mask = 32

        // Checks if the agent is a guard, and if they are standing in security.
        if (navArea.mask == 32)
        {

            //RaycastHit hit;

            //Elevated y position ray start to be more in line with eyes
            Vector3 sphereStart = new Vector3(self.transform.position.x, self.transform.position.y + 1, self.transform.position.z);


            Collider[] hitColliders = Physics.OverlapSphere(sphereStart, 1, layerMask);

            if (hitColliders.Length != 0)
            {
                foreach (Collider hitCollider in hitColliders)
                {
                    if (hitCollider.gameObject.GetComponent<AgentBase>().type == AgentTypes.Guard)
                    {
                        animator.SetBool("Escaping", false);
                    }
                }
            }

            //OverlapSphere is better than sphere raycast to detect objects 
            
            //if (Physics.SphereCast(sphereStart, 1, Vector3.forward, out hit, Mathf.Infinity, layerMask))
            //{
            //    if (hit.transform.gameObject.GetComponent<AgentBase>().type == AgentTypes.Guard)
            //    {
            //        animator.SetBool("Escaping", false);
            //    }


            //}


        }

        // If we reached the escape point (index 1), stop - prevents index exception error.
        if (destPointNum == agentScript.points.Length - 1)
        {
            return;
        }

        // Choose the next destination point when the agent gets close to the current one.
        if (!agent.pathPending && agent.remainingDistance < 0.5f)
        {
            // Returns if no points have been set up
            if (agentScript.points.Length == 0)
            {
                return;
            }

            destPointNum++;

            // Set the agent to go to the currently selected destination.
            agent.destination = agentScript.points[destPointNum].transform.position;

            //Set Color indicator
            indicatorMat.color = agentScript.points[destPointNum].GetComponent<MeshRenderer>().material.color;
        }






        ////ESTO ES EL COMPORTAMIENTO QUE TU QUIERES PERO ME PARECE MAL EN LA LOGICA DE UN JUEGO


        ////Raycast hit info
        //RaycastHit hit;

        ////Elevated y position ray start to be more in line with eyes
        //Vector3 rayStart = new Vector3(self.transform.position.x, self.transform.position.y + 1, self.transform.position.z);

        //if (Physics.Raycast(rayStart, self.transform.TransformDirection(Vector3.forward), out hit, 1, layerMask))
        //{
        //    if (hit.transform.gameObject.GetComponent<AgentBase>().type == AgentTypes.Guard)
        //    {
        //        animator.SetBool("Escaping", false);
        //    }
        //}
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
