using UnityEngine;
using UnityEditor;

public class WaypointMenuItem : Editor
{
    [MenuItem("GameObject/Waypoints/Waypoint Parent", false, 0)]
    private static void Create()
    {
        WaypointParent waypointParent = new GameObject("WaypointParent", typeof(WaypointParent)).GetComponent<WaypointParent>();
        waypointParent.transform.position = Vector3.zero;

        Undo.RegisterCreatedObjectUndo(waypointParent.gameObject, "Create Waypoint Parent");

        CreateFirstWaypoint(waypointParent);
    }

    private static void CreateFirstWaypoint(WaypointParent waypointParent)
    {
        Waypoint newWaypoint = new GameObject("Waypoint_" + waypointParent.waypoints.Count.ToString(), typeof(Waypoint)).GetComponent<Waypoint>();

        waypointParent.waypoints.Add(newWaypoint);
        newWaypoint.transform.SetParent(waypointParent.transform);
        newWaypoint.transform.position = waypointParent.transform.position;
        Selection.activeObject = newWaypoint;
    }
}
