using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.Rendering;

public class BikeDriverAI2 : MonoBehaviour
{
    private BicycleVehicle bikeDriver;
    // private NavMeshAgent navMeshAgent;
    private Vector3 waypoint;
    private Vector3 initBikePosition;
    public float distanceToTarget;

    // private Vector3 targetPosition;
    // private int currentWaypointIndex = 0;
    // private bool hasPath = false;

    // [SerializeField] private Transform targetPositionTransform;
    [Range(0, 70f)] [SerializeField] private float stoppingDistance = 10f;
    [Range(0, 70f)] [SerializeField] private float stoppingSpeed = 20f;
    [Range(0, 20f)] [SerializeField] public float reachedTargetDistance = 3f;
    // [Range(0, 1f)] [SerializeField] private float speedFactor = 0.5f;  // How much speed affects stopping distance

    // [Range(0, 70f)] [SerializeField] private float baseStoppingDistance = 10f;

    // void PrintAvailableNavMeshAreas()
    // {
    //     // Find all NavMesh components in the scene
    //     NavMeshSurface[] navMeshSurfaces = FindObjectsOfType<NavMeshSurface>();
    //
    //     Debug.Log("Available NavMesh Area Indexes:");
    //
    //     foreach (NavMeshSurface navMeshSurface in navMeshSurfaces)
    //     {
    //         // Assuming NavMeshSurface stores the area index in its area field
    //         Debug.Log($"Area Index: {navMeshSurface}");
    //     }
    // }
    
    private void Awake()
    {

        bikeDriver = GetComponent<BicycleVehicle>();
        // navMeshAgent = GetComponent<NavMeshAgent>();
        
        // navMeshAgent.updateRotation = false; // We control the rotation manually
        // navMeshAgent.updatePosition = true;  // Allow NavMeshAgent to update position

    }

    public void setInitBikePosition(Vector3 pos)
    {
        Debug.Log("setInitBikePosition: " + pos);
        Vector3 dirToMovePosition = (waypoint - transform.position).normalized;
        bikeDriver.setInitPositionBicycleVehicle(pos,dirToMovePosition);
    }

    public void SetWaypoint(Vector3 newWaypoint)
    {
        waypoint = newWaypoint;
    }
    
    private void Update()
    {
        if (waypoint != null)
        {
            MoveToNextWaypoint();
        }
    }
    
    private void MoveToNextWaypoint()
    {

        distanceToTarget = Vector3.Distance(transform.position, waypoint);
        float forwardInput = 1f;

        
        if (distanceToTarget > reachedTargetDistance)
        {
            Vector3 dirToMovePosition = (waypoint - transform.position).normalized;
            float angleToDir = Vector3.SignedAngle(transform.forward, dirToMovePosition, Vector3.up);
            
            float turnInput = angleToDir / 90f;
            
            bikeDriver.SetInput(forwardInput, turnInput);


            // if (distanceToTarget < stoppingDistance && bikeDriver.GetSpeed() > stoppingSpeed)
            // {
            //     forwardInput = -1f;
            //     bikeDriver.SetBrake(true);
            // }
        }
        else
        {
            bikeDriver.SetBrake(false);  // Ensure brakes are released
        }
        // if (distanceToTarget <= reachedTargetDistance)
        // {
        //     // bikeDriver.SetBrake(true);
        //     Debug.Log("Waypoint reached!");
        // }
        
       

    }

    // private void SetTargetPositionFromMouse()
    // {
    //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    //     if (Physics.Raycast(ray, out RaycastHit hit))
    //     {
    //         targetPosition = hit.point;
    //         bikeDriver.SetBrake(false);
    //         Debug.Log($"New Target Position: {targetPosition}");
    //         bikeAgent.SetDestination(targetPosition);
    //     }
    // }
}

