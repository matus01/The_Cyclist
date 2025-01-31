using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

[CustomEditor(typeof(Branch))]
public class BranchEditor : Editor
{
    private const float ARROW_SIZE = 2f;
    private const string INSPECTOR_TEXT_RUNTIME = "Branch Editor can't work in runtime.";
    private const string INSPECTOR_TEXT_EDITOR = "Start creating waypoints by KeyCode in the scene view.\n\n" +
        "KeyCode.Alpha1: Create Waypoint\n" +
        "KeyCode.Alpha2: Attach Ground";

    private WaypointParent m_waypointParent;
    private Branch m_branch;
    private Dictionary<KeyCode, Action> m_keyCodeActions;

    private void OnEnable()
    {
        if (Selection.activeGameObject == null)
        {
            return;
        }

        m_branch = Selection.activeGameObject.GetComponent<Branch>();
        m_waypointParent = m_branch.transform.parent.GetComponent<WaypointParent>();

        m_keyCodeActions = new Dictionary<KeyCode, Action>()
        {
            { KeyCode.Alpha1, CreateWaypoint },
            { KeyCode.Alpha2, AttachGround },
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

    private void CreateWaypoint()
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        RaycastHit raycastHit;

        if (Physics.Raycast(ray, out raycastHit))
        {
            Waypoint waypoint = new GameObject("Waypoint_" + m_waypointParent.waypoints.Count.ToString(), typeof(Waypoint)).GetComponent<Waypoint>();
            waypoint.transform.SetParent(m_waypointParent.transform);
            waypoint.transform.position = raycastHit.point;

            Vector3 forward = waypoint.transform.position - m_branch.transform.position;
            forward.y = 0f;
            waypoint.transform.forward = forward;
            waypoint.transform.localScale = m_branch.transform.localScale;
            waypoint.branch = m_branch;

            Undo.RegisterCreatedObjectUndo(waypoint.gameObject, "Create Waypoint");

            Undo.RecordObject(m_waypointParent, "Assign Waypoint to Parent");
            m_waypointParent.waypoints.Add(waypoint);

            Undo.RecordObject(m_branch, "Assign Waypoint to branch");
            m_branch.waypoints.Add(waypoint);

            Selection.activeGameObject = waypoint.gameObject;
        }
    }

    private void AttachGround()
    {
        Ray ray = new Ray(m_branch.transform.position + Vector3.up * 9999, Vector3.down);
        RaycastHit raycastHit;

        if (Physics.Raycast(ray, out raycastHit))
        {
            Undo.RecordObject(m_branch, "Attach Ground");
            m_branch.transform.position = raycastHit.point;
        }
    }

    [DrawGizmo(GizmoType.NonSelected | GizmoType.Selected | GizmoType.Pickable)]
    public static void OnDrawSceneGizmo(Branch branch, GizmoType gizmoType)
    {
        float alpha = (gizmoType & GizmoType.NonSelected) != 0 ? 0.25f : 1f;

        Color color = Color.blue;
        color.a = alpha;

        Handles.color = color;
        Gizmos.color = color;
        Gizmos.DrawSphere(branch.transform.position, 0.5f);

        for(int i = 0; i < branch.waypoints.Count; i++)
        {
            Vector3 startPosition = branch.transform.position;
            Vector3 endPosition = branch.waypoints[i].transform.position;
            Handles.ArrowHandleCap(0, startPosition, Quaternion.LookRotation(endPosition - startPosition, Vector3.up), ARROW_SIZE, EventType.Repaint);
            Handles.DrawLine(startPosition, endPosition);
        }
    }
}
