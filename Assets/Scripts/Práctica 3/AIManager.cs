using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIManager : MonoBehaviour
{

    //private struct AssignedBeds
    //{
    //    public GameObject availableBeds;
    //    public GameObject agent;

    //}

    public static AIManager Instance { get; private set; } // Get and Private set is useless in this case, but allows other objects to get information from it, but not destroy the singleton

    //private AssignedBeds[] agentCells; 

    private GameObject[] agents;
    private GameObject[] telephones;
    public GameObject[] guardWaypoints;


    public List<GameObject> availableGuardWaypoints;
    public List<GameObject> prisonAgents;
    public List<GameObject> availableBeds;
    public List<GameObject> areaCenters;

    public Vector3 escapistPos;

    private void Awake()
    {
        // Creation of singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else // Destroy duplicate instances of this script
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        // Create new list of beds
        availableBeds = new List<GameObject>();

        // Create new list of prisoners
        prisonAgents = new List<GameObject>();

        // Create a new list of area centers
        areaCenters = new List<GameObject>();

        // Create a new list of guard waypoints for removal later
        availableGuardWaypoints = new List<GameObject>();

        // Store beds
        availableBeds.AddRange(GameObject.FindGameObjectsWithTag("Bed"));

        // Store all area centers in a list
        areaCenters.AddRange(GameObject.FindGameObjectsWithTag("AreaCenter"));

        // Store all agents in array
        agents = GameObject.FindGameObjectsWithTag("Agent");

        // Store all guard waypoints in array
        guardWaypoints = GameObject.FindGameObjectsWithTag("GuardWaypoint");

        // Store all guard waypoints in a list
        availableGuardWaypoints.AddRange(GameObject.FindGameObjectsWithTag("GuardWaypoint"));

        // Store all phones in array
        telephones = GameObject.FindGameObjectsWithTag("Telephone");

        // This loop takes the inmates and escapists and adds them to the list of prisoners, excluding guards and visitors
        for (int i = 0; i < agents.Length; i++)
        {
            // Check for inmate/escapist
            if (agents[i].GetComponent<AgentBase>().type == AgentTypes.Inmate || agents[i].GetComponent<AgentBase>().type == AgentTypes.Escapist)
            {
                prisonAgents.Add(agents[i]); // Add to the list
                //AssignedBeds hola = new AssignedBeds();
                //hola.agent = agents[i];
            }
        }

    }


    // Method that sets alarm state for all agents
    public void AlarmState()
    {
        for (int i = 0; i < agents.Length; i++)
        {
            if (!agents[i].name.StartsWith("Escapist"))
            {
                agents[i].GetComponent<Animator>().SetBool("Alarm", true);
            }
        }
    }

    // Method that assigns bed to each agent
    public Vector3 AssignBed()
    {
        int num = Random.Range(0, availableBeds.Count - 1);
        Vector3 bedAssigned = availableBeds[num].transform.position;
        availableBeds.Remove(availableBeds[num]);
        return bedAssigned;
    }

    // Method that finds the nearest phone
    public Vector3 GetNearestPhone(Vector3 guardPosition)
    {
        float currentMinimum = Vector3.Distance(telephones[0].transform.position, guardPosition);
        Vector3 nearestPhone = Vector3.zero;
        for (int i = 0; i < telephones.Length; i++)
        {
            if (Vector3.Distance(telephones[i].transform.position, guardPosition) < currentMinimum)
            {
                currentMinimum = Vector3.Distance(telephones[i].transform.position, guardPosition);
                nearestPhone = telephones[i].transform.position;
            }
        }
        return nearestPhone;
    }

    // Method that assigns patrol areas
    public List<GameObject> GetPatrolArea()
    {
        // Reference to list of patrolPoints for specific area
        List<GameObject> assignedPatrol;

        // Initialize list
        assignedPatrol = new List<GameObject>();

        //Random area center to check so guard patrol is random every time
        int num = Random.Range(0, areaCenters.Count);

        // Get area color
        Material areaMat = areaCenters[num].GetComponent<MeshRenderer>().material;

        // Loop to check if waypoint is within the radius of an area center
        for (int i = 0; i < guardWaypoints.Length; i++)
        {
            if (availableGuardWaypoints.Contains(guardWaypoints[i])) // Check if waypoint is available
            {
                if (Vector3.Distance(areaCenters[num].transform.position, guardWaypoints[i].transform.position) < 20) // Check if distance to area center is less than 20
                {
                    GameObject assignedWaypoint = availableGuardWaypoints.Find(x => x.name == guardWaypoints[i].name);
                    assignedPatrol.Add(assignedWaypoint); // Add to path
                    assignedWaypoint.GetComponent<MeshRenderer>().material = areaMat; // Set color to all waypoints in the area
                    availableGuardWaypoints.Remove(assignedWaypoint); // Remove waypoint from the list of available points
                }
            }
        }

        areaCenters.Remove(areaCenters[num]);

        return assignedPatrol;
    }


}
