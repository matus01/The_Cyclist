using UnityEngine;
using System.Collections.Generic;

public class AgentWithSpeed
{
    public UnityEngine.AI.NavMeshAgent agent;
    public float preferredSpeed;

    public AgentWithSpeed(UnityEngine.AI.NavMeshAgent agent, float preferredSpeed)
    {
        this.agent = agent;
        this.preferredSpeed = preferredSpeed;
    }
}

public class YieldArea : MonoBehaviour
{
    private int intersectionID;
    private int activeAgents;
    private List<AgentWithSpeed> waitingAgents = new List<AgentWithSpeed>();
    private List<UnityEngine.AI.NavMeshAgent> agentsToBeRemoved = new List<UnityEngine.AI.NavMeshAgent>();

    void Start()
    {
        Component parentIntersection = GetComponentInParent<Crosswalk>() as Component ?? GetComponentInParent<Intersection>();

        if (parentIntersection is Crosswalk crosswalkParent)
        {
            intersectionID = crosswalkParent.intersectionID;
            Debug.Log($"YieldArea {gameObject.name} initialized with IntersectionID (Crosswalk): {intersectionID}");
        }
        else if (parentIntersection is Intersection intersectionParent)
        {
            intersectionID = intersectionParent.intersectionID;
            Debug.Log($"YieldArea {gameObject.name} initialized with IntersectionID (Intersection): {intersectionID}");
        }
        else
        {
            Debug.LogWarning($"No valid parent Intersection or Crosswalk found for YieldArea {gameObject.name}.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        const int firstRegularCrossingIntersectionID = 10;
        if(intersectionID < firstRegularCrossingIntersectionID)
        {
            if (other.CompareTag("Cyclist"))
            {
                
                activeAgents = IntersectionManager.Instance.GetActiveAgentsAtIntersection(intersectionID);
                UnityEngine.AI.NavMeshAgent agent = other.GetComponent<UnityEngine.AI.NavMeshAgent>();
                
                if (agent != null &&  IntersectionManager.Instance.GetActiveAgentsAtIntersection(intersectionID) != 0)
                {
                    AddWaitingAgent(agent);
                    agent.speed = 0;
                }
                Intersection parentIntersection = GetComponentInParent<Intersection>();
                if (parentIntersection == null)
                {
                    Debug.LogWarning("No parent intersection found for yield area.");
                    return;
                }

                if (!parentIntersection.GetIgnorableIntersectionAreaAgents().Contains(agent))
                {
                    parentIntersection.AddIgnorableIntersectionAreaAgent(agent);
                    StartCoroutine(parentIntersection.RemoveIgnorableIntersectionAreaAgentAfterDelay(agent, 10.0f));
                }

            }
        }
        else
        {
            if (other.CompareTag("Pedestrian"))
            {
                activeAgents = IntersectionManager.Instance.GetActiveAgentsAtIntersection(intersectionID);
                UnityEngine.AI.NavMeshAgent agent = other.GetComponentInParent<UnityEngine.AI.NavMeshAgent>();
                if(agent != null && intersectionID >= firstRegularCrossingIntersectionID)
                {
                    Intersection parentIntersection = GetComponentInParent<Intersection>();
                    if(!parentIntersection.GetIgnorableYieldAreaAgents().Contains(agent))
                    {
                        Debug.Log("added agent " + agent);
                        if (agent != null &&  IntersectionManager.Instance.GetActiveAgentsAtIntersection(intersectionID) != 0){
                            AddWaitingAgent(agent);
                            agent.speed = 0;
                        }
                        parentIntersection.AddIgnorableYieldAreaAgent(agent);
                    }
                    else
                    {
                        Debug.Log("removed agent " + agent);
                        parentIntersection.RemoveIgnorableYieldAreaAgent(agent);
                    }
                }
                if (agent != null && intersectionID < firstRegularCrossingIntersectionID &&  IntersectionManager.Instance.GetActiveAgentsAtIntersection(intersectionID) != 0)
                {
                    AddWaitingAgent(agent);
                    agent.speed = 0;
                }
            }
        }
    }

    public void AddWaitingAgent(UnityEngine.AI.NavMeshAgent agent)
    {

        if (!waitingAgents.Exists(x => x.agent == agent))
        {
            waitingAgents.Add(new AgentWithSpeed(agent,agent.speed));
        }
    }

    public void RemoveWaitingAgent(UnityEngine.AI.NavMeshAgent agent)
    {
        AgentWithSpeed agentToRemove = waitingAgents.Find(a => a.agent == agent);
        if (agentToRemove != null)
        {
            waitingAgents.RemoveAll(x => x.agent == agent);
        }
    }

    void Update()
    {
        activeAgents = IntersectionManager.Instance.GetActiveAgentsAtIntersection(intersectionID);
        if(waitingAgents != null)
            
            foreach (AgentWithSpeed agentWithSpeed in waitingAgents)
            {
                if(activeAgents == 0)
                {
                    agentWithSpeed.agent.speed = agentWithSpeed.preferredSpeed;
                    agentsToBeRemoved.Add(agentWithSpeed.agent);
                    //RemoveWaitingAgent(agentWithSpeed.agent);
                }
            }
            foreach(UnityEngine.AI.NavMeshAgent Agent in agentsToBeRemoved)
                RemoveWaitingAgent(Agent);
            agentsToBeRemoved.Clear();
    }

}
