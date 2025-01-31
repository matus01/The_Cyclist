using System.Collections.Generic;
using UnityEngine;

public class IntersectionManager : MonoBehaviour
{
    public static IntersectionManager Instance { get; private set; }
    public int intersectionCount = 7;

    private Dictionary<int, int> activeAgentsPerIntersection = new Dictionary<int, int>();

    void Start()
    {
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        //Time.timeScale = 0.25f;

        int intersectionCount = 7;
        for (int i = 0; i < intersectionCount; i++)
        {
            activeAgentsPerIntersection[i] = 0; 
        }
    }

    public void AddAgentToIntersection(int intersectionID)
    {
        if (intersectionID < 0)
        {
            Debug.LogWarning("Invalid intersection ID: " + intersectionID);
            return;
        }

        if (!activeAgentsPerIntersection.ContainsKey(intersectionID))
            activeAgentsPerIntersection[intersectionID] = 0;

        activeAgentsPerIntersection[intersectionID]++;
        // if(intersectionID == 6)
        //     Debug.Log($"Intersection {intersectionID} now has {activeAgentsPerIntersection[intersectionID]} active agents.");
    }

    public void RemoveAgentFromIntersection(int intersectionID)
    {
        if (intersectionID < 0)
        {
            Debug.LogWarning("Invalid intersection ID: " + intersectionID);
            return;
        }

        if (activeAgentsPerIntersection.ContainsKey(intersectionID))
        {
            activeAgentsPerIntersection[intersectionID]--;
            // if(intersectionID >= 5)
            //     Debug.Log($"Intersection {intersectionID} now has {activeAgentsPerIntersection[intersectionID]} active agents.");

            if (activeAgentsPerIntersection[intersectionID] <= 0)
                activeAgentsPerIntersection[intersectionID] = 0;
        }
    }

    public int GetActiveAgentsAtIntersection(int intersectionID)
    {
        if (activeAgentsPerIntersection.ContainsKey(intersectionID))
            return activeAgentsPerIntersection.TryGetValue(intersectionID, out int count) ? count : 0;
        
        Debug.LogWarning($"Intersection {intersectionID} not found.");
        return 0;
    }

    public void LogAllIntersectionCounts()
    {
        Debug.Log("Current intersection agent counts:");
        foreach (var entry in activeAgentsPerIntersection)
        {
            Debug.Log($"Intersection {entry.Key}: {entry.Value} active agents.");
        }
    }
}
