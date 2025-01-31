using System.Collections.Generic;
using UnityEngine;

public enum WaypointType { Regular, Entry, Exit }

public class Waypoint : MonoBehaviour
{
    public Waypoint previousWaypoint;
    public Waypoint nextWaypoint;
    public Branch branch;
 

    public WaypointType waypointType = WaypointType.Regular;

    public Vector3 GetInitialPosition()
    {
        Vector3 position = transform.position;
        Vector3 bias = transform.right * transform.localScale.x * 0.5f;
        Vector3 lerpPosition = position;
        Vector3 lerpBias = bias;

        if (previousWaypoint != null)
        {
            lerpPosition = previousWaypoint.transform.position;
            lerpBias = previousWaypoint.transform.right * previousWaypoint.transform.localScale.x * 0.5f;
        }
        else if (nextWaypoint != null)
        {
            lerpPosition = nextWaypoint.transform.position;
            lerpBias = nextWaypoint.transform.right * nextWaypoint.transform.localScale.x * 0.5f;
        }

        float random = Random.value;
        position = Vector3.Lerp(position, lerpPosition, random);
        bias = Vector3.Lerp(bias, lerpBias, random);

        return position + Vector3.Lerp(-bias, bias, Random.value);
    }

    public Vector3 GetWaypointPosition(Transform agentTransform, UnityEngine.AI.NavMeshAgent agent)
    {
        Vector3 forwardAgent = agentTransform.forward.normalized;
        Vector3 forwardWaypoint = transform.forward.normalized;

        float dotProduct = Vector3.Dot(forwardAgent, forwardWaypoint);

        Vector3 bias = (transform.right * transform.localScale.x * 0.5f);
        Vector3 waypointPosition = Vector3.zero;
        if (dotProduct >= 0.3f)
        {
            Debug.Log(agent.name);
            if(agent.name == "Bike(Clone)")
            {
                if(agent.speed == 3)
                    waypointPosition = transform.position + bias;
                else
                    waypointPosition = transform.position - bias;
            }
            else 
            {
                waypointPosition = transform.position + Vector3.Lerp(-bias + bias, bias, Random.value);   
            }
        }
        else
        {
            waypointPosition = transform.position + Vector3.Lerp(-bias + bias, -bias, Random.value);
        }
            
        //Debug.Log(waypointPosition);
        return waypointPosition;
    }

    public Waypoint GetWaypoint(ref bool direction)
    {
        Waypoint waypoint = direction ? nextWaypoint : previousWaypoint;
        if(waypoint == null)
        {
            if(branch != null)
            {
                waypoint = branch.GetWaypoint(this);

                if ((direction && waypoint.nextWaypoint == null) ||
                    (!direction && waypoint.previousWaypoint == null))
                {
                    direction = !direction;
                }
            }
            else
            {
                direction = !direction;
                waypoint = direction ? nextWaypoint : previousWaypoint;
            }
        }

        return waypoint;
    }
}
