# Jail-Navigation

This project served as my first contact with AI and Navigation systems in Unity. The task was to setup a prison with four different types of agents with different behaviours:

![](https://github.com/VerdalJG/Jail-Navigation/blob/main/BirdsEye.png)

Visitors: Move around the visiting waypoints, run away if they see a prisoner.
Guard: Assigned to random prison areas and patrol them. If they see an escapist outside of the prison, they go to the nearest telephone and turn the alarm on, after which all guards will run to catch the escapist.
Inmate: Basic prisoner that moves around the prison.
Escapist: Moves around different waypoints with the goal of escaping. If caught, the escapist returns to his respective cell and re-attempts to escape out of the prison

![](https://github.com/VerdalJG/Jail-Navigation/blob/main/Movement.gif)

The system functions using Unity's Navmesh and agent components along with code for detection and behaviour. Agents can perform jumps, open doors and use raycasts to detect other agents.

![](https://github.com/VerdalJG/Jail-Navigation/blob/main/Jumps.png)

