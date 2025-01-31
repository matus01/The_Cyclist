using UnityEngine;

public class CrowdGenerator : MonoBehaviour
{
    public WaypointParent waypointParent;
    public int crowdCount = 100;
    public WaypointNavigator[] waypointNavigators;

    private void Awake()
    {
        for(int i = 0; i < crowdCount; i++)
        {
            WaypointNavigator waypointNavigator = GameObject.Instantiate(waypointNavigators[Random.Range(0, waypointNavigators.Length)]);
            waypointNavigator.SetWaypointParent(waypointParent);
            waypointNavigator.transform.SetParent(transform);
        }
    }
}
