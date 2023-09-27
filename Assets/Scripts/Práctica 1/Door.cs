using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Door : MonoBehaviour
{
    Vector3 startPos;
    Vector3 finalPos;
    bool open = false;
    public bool escapistValid;
    public bool guardValid;
    public bool inmateValid;
    public bool visitorValid;

    private void Start()
    {
         startPos = transform.position;

        if (gameObject.name.StartsWith("SM_Door_B2_Left"))
        {
            finalPos = new Vector3(transform.position.x, transform.position.y, transform.position.z + 2);
        }
        else if (gameObject.name.StartsWith("SM_Door_B2_Right"))
        {
            finalPos = new Vector3(transform.position.x, transform.position.y, transform.position.z - 2);
        }

        else
        {
            if (transform.rotation.eulerAngles.y == 0)
            {
                finalPos = new Vector3(transform.position.x - 2, transform.position.y, transform.position.z);
            }

            else if (transform.rotation.eulerAngles.y == 270)
            {
                finalPos = new Vector3(transform.position.x, transform.position.y, transform.position.z - 2);
            }

            else if (transform.rotation.eulerAngles.y == 90)
            {
                finalPos = new Vector3(transform.position.x, transform.position.y, transform.position.z - 2);
            }
        }
        

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Agent")
        {
            AgentBase agentScript = other.GetComponent<AgentBase>();
            if (escapistValid)
            {
                if (agentScript.type == AgentTypes.Escapist)
                {
                    open = true;
                }   
            }

            if (guardValid)
            {
                if (agentScript.type == AgentTypes.Guard)
                {
                    open = true;
                }
            }
            if (inmateValid)
            {
                if (agentScript.type == AgentTypes.Inmate)
                {
                    open = true;
                }
            }
            if (visitorValid)
            {
                if (agentScript.type == AgentTypes.Visitor)
                {
                    open = true;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Agent")
        {
            open = false;
        }
    }

    private void Update()
    {
        if (open)
        {
            transform.position = Vector3.MoveTowards(transform.position, finalPos, 0.1f);
        }
        else if (!open)
        {
            transform.position = Vector3.MoveTowards(transform.position, startPos, 0.1f);
        }
    }


}
