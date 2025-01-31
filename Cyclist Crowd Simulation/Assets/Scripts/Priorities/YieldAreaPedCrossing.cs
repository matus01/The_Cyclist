using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class AgentWithSpeed2
{
    public UnityEngine.AI.NavMeshAgent agent;
    public float preferredSpeed;

    public AgentWithSpeed2(UnityEngine.AI.NavMeshAgent agent, float preferredSpeed)
    {
        this.agent = agent;
        this.preferredSpeed = preferredSpeed;
    }
}

public class YieldAreaPedCrossing : MonoBehaviour
{
    public int intersectionID;
    private int activeAgents;
    private PedCrossingOverhead parentIntersection;
    private List<AgentWithSpeed2> waitingAgents = new List<AgentWithSpeed2>();
    private List<UnityEngine.AI.NavMeshAgent> agentsToBeRemoved = new List<UnityEngine.AI.NavMeshAgent>();
    private Dictionary<UnityEngine.AI.NavMeshAgent, float> cooldownAgents = new Dictionary<UnityEngine.AI.NavMeshAgent, float>();
    private float cooldownDuration = 5f;


    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Pedestrian") && !IsAgentOnCooldown(other.GetComponentInParent<UnityEngine.AI.NavMeshAgent>()))
        {
            Debug.Log("Pedestrian entered yield area");
            activeAgents = IntersectionManager.Instance.GetActiveAgentsAtIntersection(intersectionID);
            UnityEngine.AI.NavMeshAgent agent = other.GetComponentInParent<UnityEngine.AI.NavMeshAgent>();
            if(agent != null)
            {
                parentIntersection = GetComponentInParent<PedCrossingOverhead>();
                if(parentIntersection == null)
                {
                    Debug.Log("Parent intersection not found");
                    return;
                }    
                if(!parentIntersection.GetIgnorableAgents().Contains(agent))
                {
                    Debug.Log("added agent " + agent);
                    if (IntersectionManager.Instance.GetActiveAgentsAtIntersection(intersectionID) != 0){
                        AddWaitingAgent(agent);
                        agent.speed = 0;
                    }
                    else{
                        parentIntersection.AddRunningAgent(agent);
                        // runningAgents.Add(new AgentWithSpeed2(agent,agent.speed));
                        List<AgentWithSpeed3> runningAgents = parentIntersection.GetRunningAgents();
                        Debug.Log("Added running agent " + runningAgents.Count);
                        agent.speed = 1.5f;
                    }
                    parentIntersection.AddIgnorableAgent(agent);
                }
                else
                {
                    Debug.Log("Speed reverted1");
                    List<AgentWithSpeed3> runningAgentsList = parentIntersection.GetRunningAgents();
                    AgentWithSpeed3 runningAgent = runningAgentsList.Find(a => a.agent == agent);

                    if (runningAgent != null)
                    {
                        Debug.Log("Speed reverted2");
                        agent.speed = runningAgent.preferredSpeed;
                        Debug.Log("Removed running agent");
                        parentIntersection.RemoveRunningAgent(agent);
                    }
                    parentIntersection.RemoveIgnorableAgent(agent);
                }
            }
            if (agent != null && intersectionID < 5 &&  IntersectionManager.Instance.GetActiveAgentsAtIntersection(intersectionID) != 0)
            {
                AddWaitingAgent(agent);
                agent.speed = 0;
            }
            AddCooldownAgent(agent);
        }
    }

    public void AddWaitingAgent(UnityEngine.AI.NavMeshAgent agent)
    {
        // RemoveRunningAgent(agent);
        if (!waitingAgents.Exists(x => x.agent == agent))
        {
            waitingAgents.Add(new AgentWithSpeed2(agent,agent.speed));
        }
    }

    public void RemoveWaitingAgent(UnityEngine.AI.NavMeshAgent agent)
    {
        AgentWithSpeed2 agentToRemove = waitingAgents.Find(a => a.agent == agent);
        if (agentToRemove != null)
        {
            waitingAgents.RemoveAll(x => x.agent == agent);
        }
    }

     

    void Update()
    {
        RemoveExpiredCooldownAgents();
        // Debug.Log("xxx: " + runningAgents.Count);
        activeAgents = IntersectionManager.Instance.GetActiveAgentsAtIntersection(intersectionID);
        if(waitingAgents != null)
            
            foreach (AgentWithSpeed2 agentWithSpeed in waitingAgents)
            {
                if(activeAgents == 0)
                {
                    agentWithSpeed.agent.speed = agentWithSpeed.preferredSpeed;
                    parentIntersection.AddRunningAgent(agentWithSpeed.agent);
                    agentWithSpeed.agent.speed = 1.5f;
                    agentsToBeRemoved.Add(agentWithSpeed.agent);
                }
            }
            foreach(UnityEngine.AI.NavMeshAgent Agent in agentsToBeRemoved)
                RemoveWaitingAgent(Agent);
            agentsToBeRemoved.Clear();
    }

    private void RemoveExpiredCooldownAgents()
    {
        // Get the current time
        float currentTime = Time.time;

        // Find all agents whose cooldown has expired
        var expiredAgents = cooldownAgents.Where(kvp => kvp.Value <= currentTime).ToList();

        // Remove expired agents from the dictionary
        foreach (var agent in expiredAgents)
        {
            cooldownAgents.Remove(agent.Key);
            Debug.Log($"Removed agent {agent.Key} from cooldown list.");
        }
    }

    public void AddCooldownAgent(UnityEngine.AI.NavMeshAgent agent)
    {
        if (!cooldownAgents.ContainsKey(agent))
        {
            // Add the agent with the current time + cooldown duration
            cooldownAgents[agent] = Time.time + cooldownDuration;
            Debug.Log($"Added agent {agent} to cooldown list.");
        }
    }

    public bool IsAgentOnCooldown(UnityEngine.AI.NavMeshAgent agent)
    {
        // Check if the agent is in the cooldown list
        return cooldownAgents.ContainsKey(agent);
    }

}
