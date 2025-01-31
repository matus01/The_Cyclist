using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class WaypointNavigator : MonoBehaviour
{
    private readonly int FORWARD_ID = Animator.StringToHash("Forward");

    [SerializeField] private NavMeshAgent m_navMeshAgent = null;
    [SerializeField] private Animator m_animator = null;
    [SerializeField] private AnimationClip[] m_walkAnimationClips = null;
    [SerializeField] private float[] m_walkSpeed = null;

    private Transform m_transform;
    private float m_stoppingDistance;
    private bool m_direction;
    private int oldIndex;
    private bool running = false;
    private Waypoint m_curWaypoint;
    public Vector3 m_curDestination;

    private void Reset()
    {
        m_navMeshAgent = GetComponent<NavMeshAgent>();
        m_animator = GetComponent<Animator>();
    }

    private void Awake()
    {
        m_transform = transform;

        RuntimeAnimatorController runtimeAnimatorController = m_animator.runtimeAnimatorController;
        AnimatorOverrideController originalOverrideController = runtimeAnimatorController as AnimatorOverrideController;
        if (originalOverrideController != null)
        {
            runtimeAnimatorController = originalOverrideController.runtimeAnimatorController;
        }

        AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController();
        animatorOverrideController.runtimeAnimatorController = runtimeAnimatorController;

        if (m_walkAnimationClips.Length > 1)
        {
            int index = Random.Range(0, 2); // always one of the first 2
            //Debug.Log(index);
            oldIndex = index;
            animatorOverrideController["walking"] = m_walkAnimationClips[index];
            m_navMeshAgent.speed = m_walkSpeed[index];
        }

        m_animator.runtimeAnimatorController = animatorOverrideController;

        m_stoppingDistance = m_navMeshAgent.stoppingDistance;
        UpdateDirection();
        m_curDestination = m_transform.position;
    }

    private void UpdateDirection()
    {
        m_direction = Random.value <= 0.5f;
    }

    public void SetWaypointParent(WaypointParent waypointParent)
    {
        m_curWaypoint = waypointParent.waypoints[Random.Range(0, waypointParent.waypoints.Count)];
        m_curDestination = m_curWaypoint.GetInitialPosition();
        m_navMeshAgent.Warp(m_curDestination);
    }

    private void Update()
    {
        if (m_navMeshAgent.velocity != Vector3.zero)
        {
            m_animator.SetFloat(FORWARD_ID, 1f);
        }
        else
        {
            m_animator.SetFloat(FORWARD_ID, 0f);
        }

        if(m_navMeshAgent.remainingDistance <= m_navMeshAgent.stoppingDistance)
        {
            UpdateWaypoint();
        }

        if(m_navMeshAgent.name == "Bike(Clone)")
            return;
            
        if(m_navMeshAgent.speed > 1 && !running)
        {
            Debug.Log("Speed changed");
            running = true;
            RuntimeAnimatorController runtimeAnimatorController = m_animator.runtimeAnimatorController;
            AnimatorOverrideController originalOverrideController = runtimeAnimatorController as AnimatorOverrideController;
            if (originalOverrideController != null)
            {
                runtimeAnimatorController = originalOverrideController.runtimeAnimatorController;
            }

            AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController();
            animatorOverrideController.runtimeAnimatorController = runtimeAnimatorController;
    
            animatorOverrideController["walking"] = m_walkAnimationClips[2];

            m_animator.runtimeAnimatorController = animatorOverrideController;
        }
        else if(m_navMeshAgent.speed < 1 && running)
        {
            running = false;
            RuntimeAnimatorController runtimeAnimatorController = m_animator.runtimeAnimatorController;
            AnimatorOverrideController originalOverrideController = runtimeAnimatorController as AnimatorOverrideController;
            if (originalOverrideController != null)
            {
                runtimeAnimatorController = originalOverrideController.runtimeAnimatorController;
            }
            AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController();
            animatorOverrideController.runtimeAnimatorController = runtimeAnimatorController;
    
            animatorOverrideController["walking"] = m_walkAnimationClips[oldIndex];

            m_animator.runtimeAnimatorController = animatorOverrideController;
        }
    }

    private void UpdateWaypoint()
    {
        if(m_curWaypoint == null)
        {
            return;
        }

        m_curWaypoint = m_curWaypoint.GetWaypoint(ref m_direction);
        m_curDestination = m_curWaypoint.GetWaypointPosition(transform, m_navMeshAgent);
        m_navMeshAgent.SetDestination(m_curDestination);
    }
}
