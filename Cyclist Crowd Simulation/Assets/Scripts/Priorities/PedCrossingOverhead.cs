using UnityEngine;
using System.Collections.Generic;

public class AgentWithSpeed3
{
    public UnityEngine.AI.NavMeshAgent agent;
    public float preferredSpeed;

    public AgentWithSpeed3(UnityEngine.AI.NavMeshAgent agent, float preferredSpeed)
    {
        this.agent = agent;
        this.preferredSpeed = preferredSpeed;
    }
}
public class PedCrossingOverhead : MonoBehaviour
{
    private List<UnityEngine.AI.NavMeshAgent> ignorableAgents = new List<UnityEngine.AI.NavMeshAgent>();

    private List<AgentWithSpeed3> runningAgents = new List<AgentWithSpeed3>();
    

    public List<UnityEngine.AI.NavMeshAgent> GetIgnorableAgents()
    {
        return ignorableAgents;
    }

    public void AddIgnorableAgent(UnityEngine.AI.NavMeshAgent agent)
    {
        ignorableAgents.Add(agent);
    }

    public void RemoveIgnorableAgent(UnityEngine.AI.NavMeshAgent agent)
    {
        ignorableAgents.Remove(agent);
    }

    public List<AgentWithSpeed3> GetRunningAgents()
    {
        return runningAgents;
    }

    public void AddRunningAgent(UnityEngine.AI.NavMeshAgent agent)
    {
        if (!runningAgents.Exists(x => x.agent == agent))
        {
            runningAgents.Add(new AgentWithSpeed3(agent,agent.speed));
        }
    }

    public void RemoveRunningAgent(UnityEngine.AI.NavMeshAgent agent)
    {
        Debug.Log("Removed running agent2");
        AgentWithSpeed3 agentToRemove = runningAgents.Find(a => a.agent == agent);
        if (agentToRemove != null)
        {
            runningAgents.RemoveAll(x => x.agent == agent);
        }
    }

    private void OnTriggerEnter(Collider other)
    {   
        if (other.CompareTag("Pedestrian"))
        {
            if(!ignorableAgents.Contains(other.GetComponentInParent<UnityEngine.AI.NavMeshAgent>())){
                Debug.Log("agent spawned at crossing");
                ignorableAgents.Add(other.GetComponentInParent<UnityEngine.AI.NavMeshAgent>());
            }
        }
    }
}
