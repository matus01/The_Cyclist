using System.Collections.Generic;
using UnityEngine;

public class Branch : MonoBehaviour
{
    public List<Waypoint> waypoints = new List<Waypoint>();
    private List<Waypoint> m_waypoints = new List<Waypoint>();

    public Waypoint GetWaypoint(Waypoint exceptWaypoint)
    {
        m_waypoints.Clear();
        m_waypoints.AddRange(waypoints);
        m_waypoints.Remove(exceptWaypoint);
        return m_waypoints[Random.Range(0, m_waypoints.Count)];
    }
}
