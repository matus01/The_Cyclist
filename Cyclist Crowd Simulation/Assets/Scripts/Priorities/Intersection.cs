using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public class Intersection : MonoBehaviour
{
    public int intersectionID = -1;
    private List<UnityEngine.AI.NavMeshAgent> ignorableYieldAreaAgents = new List<UnityEngine.AI.NavMeshAgent>();
    private List<UnityEngine.AI.NavMeshAgent> ignorableIntersectionAreaAgents = new List<UnityEngine.AI.NavMeshAgent>();

    public List<UnityEngine.AI.NavMeshAgent> GetIgnorableYieldAreaAgents()
    {
        return ignorableYieldAreaAgents;
    }

    public void AddIgnorableYieldAreaAgent(UnityEngine.AI.NavMeshAgent agent)
    {
        ignorableYieldAreaAgents.Add(agent);
    }

    public void RemoveIgnorableYieldAreaAgent(UnityEngine.AI.NavMeshAgent agent)
    {
        ignorableYieldAreaAgents.Remove(agent);
    }
    
    public List<UnityEngine.AI.NavMeshAgent> GetIgnorableIntersectionAreaAgents()
    {
        return ignorableIntersectionAreaAgents;
    }

    public void AddIgnorableIntersectionAreaAgent(UnityEngine.AI.NavMeshAgent agent)
    {
        ignorableIntersectionAreaAgents.Add(agent);
    }

    public void RemoveIgnorableIntersectionAreaAgent(UnityEngine.AI.NavMeshAgent agent)
    {
        ignorableIntersectionAreaAgents.Remove(agent);
    }
    
    // Coroutine to remove the agent after a delay
    public IEnumerator RemoveIgnorableIntersectionAreaAgentAfterDelay(UnityEngine.AI.NavMeshAgent agent, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (GetIgnorableIntersectionAreaAgents().Contains(agent))
        {
            ignorableIntersectionAreaAgents.Remove(agent);
        }
    }

    private void OnTriggerEnter(Collider other)
    {   
        if (other.CompareTag("Cyclist"))
        {
            // Add agent to intersection on entry
            if (intersectionID >= 0)
            {
                UnityEngine.AI.NavMeshAgent agent = other.GetComponent<UnityEngine.AI.NavMeshAgent>();
                if (GetIgnorableIntersectionAreaAgents().Contains(agent))
                {
                    if (intersectionID == 0)
                        Debug.Log("entered intersection, but agent is ignored (ignorableIntersectionAreaAgents: " + ignorableIntersectionAreaAgents.Count + ")");
                    return;
                }
                IntersectionManager.Instance.AddAgentToIntersection(intersectionID);
                
            }
            else
            {
                Debug.LogWarning("Invalid intersection ID at entry waypoint.");
            }
            
        }
        else if (other.CompareTag("Pedestrian"))
        {
            // Add agent to intersection on entry
            if (intersectionID >= 5)
            {
                if(!ignorableYieldAreaAgents.Contains(other.GetComponentInParent<UnityEngine.AI.NavMeshAgent>())){
                    Debug.Log("agent spawned at crossing");
                    ignorableYieldAreaAgents.Add(other.GetComponentInParent<UnityEngine.AI.NavMeshAgent>());
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cyclist"))
        {
            // Remove agent from intersection on exit
            if (intersectionID >= 0)
            {
                IntersectionManager.Instance.RemoveAgentFromIntersection(intersectionID);
            }
            else
            {
                Debug.LogWarning("Invalid intersection ID at exit waypoint.");
            }
        }
    }
}
