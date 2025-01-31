using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(Waypoint))]
public class WaypointEditor : Editor
{
    private const float ARROW_SIZE = 2f;
    private const string INSPECTOR_TEXT_RUNTIME = "Waypoint Editor can't work in runtime.";
    private const string INSPECTOR_TEXT_EDITOR = "Start creating waypoints by KeyCode in the scene view.\n\n" +
        "KeyCode.Alpha1: Create Next Waypoint\n" +
        "KeyCode.Alpha2: Create Previous Waypoint\n" +
        "KeyCode.Alpha3: Create Branch\n" +
        "KeyCode.Alpha4: Attach Ground";

    private WaypointParent m_waypointParent;
    private Waypoint m_waypoint;
    private Dictionary<KeyCode, Action> m_keyCodeActions;

    private void OnEnable()
    {
        if (Selection.activeGameObject == null)
        {
            return;
        }

        m_waypoint = Selection.activeGameObject.GetComponent<Waypoint>();
        m_waypointParent = m_waypoint.transform.parent.GetComponent<WaypointParent>();

        m_keyCodeActions = new Dictionary<KeyCode, Action>()
        {
            { KeyCode.Alpha1, CreateNextWaypoint },
            { KeyCode.Alpha2, CreatePreviousWaypoint },
            { KeyCode.Alpha3, CreateBranch },
            { KeyCode.Alpha4, AttachGround },
        };
    }

    public override void OnInspectorGUI()
    {
        GUI.enabled = false;

        if (Application.isPlaying)
        {
            EditorGUILayout.TextArea(INSPECTOR_TEXT_RUNTIME);
        }
        else
        {
            EditorGUILayout.TextArea(INSPECTOR_TEXT_EDITOR);
        }

        GUI.enabled = true;

        base.OnInspectorGUI();
    }

    protected void OnSceneGUI()
    {
        if (Application.isPlaying)
        {
            return;
        }

        if (Event.current.type == EventType.KeyUp)
        {
            if (!m_keyCodeActions.ContainsKey(Event.current.keyCode))
            {
                return;
            }

            m_keyCodeActions[Event.current.keyCode].Invoke();
            Event.current.Use();
        }
    }

    private void CreateNextWaypoint()
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit raycastHit;

        if (Physics.Raycast(ray, out raycastHit))
        {
            Waypoint nextWaypoint = new GameObject("Waypoint_" + m_waypointParent.waypoints.Count.ToString(), typeof(Waypoint)).GetComponent<Waypoint>();
            nextWaypoint.transform.SetParent(m_waypointParent.transform);
            nextWaypoint.transform.position = raycastHit.point;

            Vector3 forward = nextWaypoint.transform.position - m_waypoint.transform.position;
            forward.y = 0f;
            nextWaypoint.transform.forward = forward;
            nextWaypoint.transform.localScale = m_waypoint.transform.localScale;
            nextWaypoint.previousWaypoint = m_waypoint;

            Undo.RegisterCreatedObjectUndo(nextWaypoint.gameObject, "Create Next Waypoint");

            Undo.RecordObject(m_waypointParent, "Assign Waypoints");
            m_waypointParent.waypoints.Add(nextWaypoint);

            Undo.RecordObject(m_waypoint, "Assign Next Waypoint");
            m_waypoint.nextWaypoint = nextWaypoint;

            Selection.activeGameObject = nextWaypoint.gameObject;
        }
    }

    private void CreatePreviousWaypoint()
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit raycastHit;

        if (Physics.Raycast(ray, out raycastHit))
        {
            Waypoint previousWaypoint = new GameObject("Waypoint_" + m_waypointParent.waypoints.Count.ToString(), typeof(Waypoint)).GetComponent<Waypoint>();
            previousWaypoint.transform.SetParent(m_waypointParent.transform);
            previousWaypoint.transform.position = raycastHit.point;

            Vector3 forward = m_waypoint.transform.position - previousWaypoint.transform.position;
            forward.y = 0f;
            previousWaypoint.transform.forward = forward;
            previousWaypoint.transform.localScale = m_waypoint.transform.localScale;
            previousWaypoint.nextWaypoint = m_waypoint;

            Undo.RegisterCreatedObjectUndo(previousWaypoint.gameObject, "Create Previous Waypoint");

            Undo.RecordObject(m_waypointParent, "Assign Waypoints");
            m_waypointParent.waypoints.Add(previousWaypoint);

            Undo.RecordObject(m_waypoint, "Assign Previous Waypoint");
            m_waypoint.previousWaypoint = previousWaypoint;

            Selection.activeGameObject = previousWaypoint.gameObject;
        }
    }

    private void CreateBranch()
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit raycastHit;

        if (Physics.Raycast(ray, out raycastHit))
        {
            Branch branch = new GameObject("Branch_" + m_waypointParent.branches.Count.ToString(), typeof(Branch)).GetComponent<Branch>();
            branch.transform.SetParent(m_waypointParent.transform);
            branch.transform.position = raycastHit.point;

            Vector3 forward = branch.transform.position - m_waypoint.transform.position;
            forward.y = 0f;
            branch.transform.forward = forward;
            branch.transform.localScale = m_waypoint.transform.localScale;
            branch.waypoints.Add(m_waypoint);

            Undo.RegisterCreatedObjectUndo(branch.gameObject, "Create Branch");

            Undo.RecordObject(m_waypointParent, "Assign Branches");
            m_waypointParent.branches.Add(branch);

            Undo.RecordObject(m_waypoint, "Assign Branch");
            m_waypoint.branch = branch;

            Selection.activeGameObject = branch.gameObject;
        }
    }

    private void AttachGround()
    {
        Ray ray = new Ray(m_waypoint.transform.position + Vector3.up * 9999, Vector3.down);
        RaycastHit raycastHit;

        if (Physics.Raycast(ray, out raycastHit))
        {
            Undo.RecordObject(m_waypoint, "Attach Ground");
            m_waypoint.transform.position = raycastHit.point;
        }
    }

    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
    public static void OnDrawSceneGizmo(Waypoint waypoint, GizmoType gizmoType)
    {
        float alpha = (gizmoType & GizmoType.NonSelected) != 0 ? 0.25f : 1f;

        Color color = Color.yellow;
        color.a = alpha;

        Gizmos.color = color;
        Gizmos.DrawSphere(waypoint.transform.position, 0.5f);

        color = Color.white;
        color.a = alpha;
        Handles.color = color;
        Vector3 bias = waypoint.transform.right * waypoint.transform.localScale.x * 0.5f + Vector3.up * 0.01f;
        Handles.DrawLine(waypoint.transform.position - bias, waypoint.transform.position + bias);

        Waypoint previousWaypoint = waypoint.previousWaypoint;
        if (previousWaypoint != null)
        {
            color = Color.red;
            color.a = alpha;

            Handles.color = color;

            Vector3 previousBias = previousWaypoint.transform.right * previousWaypoint.transform.localScale.x * 0.5f;
            Vector3 previousDirection = previousWaypoint.transform.position - previousBias - (waypoint.transform.position - bias);
            Handles.ArrowHandleCap(0, waypoint.transform.position - bias, Quaternion.LookRotation(previousDirection, Vector3.up), ARROW_SIZE, EventType.Repaint);
            Handles.DrawLine(waypoint.transform.position - bias, previousWaypoint.transform.position - previousBias);
        }

        Waypoint nextWaypoint = waypoint.nextWaypoint;
        if (nextWaypoint != null)
        {
            color = Color.green;
            color.a = alpha;

            Handles.color = color;

            Vector3 nextBias = nextWaypoint.transform.right * nextWaypoint.transform.localScale.x * 0.5f;
            Vector3 nextDirection = nextWaypoint.transform.position + nextBias - (waypoint.transform.position + bias);
            Handles.ArrowHandleCap(0, waypoint.transform.position + bias, Quaternion.LookRotation(nextDirection, Vector3.up), ARROW_SIZE, EventType.Repaint);
            Handles.DrawLine(waypoint.transform.position + bias, nextWaypoint.transform.position + nextBias);
        }

        Branch branch = waypoint.branch;
        if (branch != null)
        {
            color = Color.blue;
            color.a = alpha;

            Handles.color = color;

            Vector3 startPosition = waypoint.transform.position;
            Vector3 endPosition = waypoint.branch.transform.position;
            Handles.ArrowHandleCap(0, startPosition, Quaternion.LookRotation(endPosition - startPosition, Vector3.up), ARROW_SIZE, EventType.Repaint);
            Handles.DrawLine(startPosition, endPosition);
        }
    }
}