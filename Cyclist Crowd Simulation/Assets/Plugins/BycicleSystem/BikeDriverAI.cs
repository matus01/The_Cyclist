using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BikeDriverAI : MonoBehaviour
{
    private BicycleVehicle bikeDriver;

    private NavMeshPath navMeshPath;

    //public NavMeshAgent bikeAgent;
    private Vector3[] waypoints;

    // private Vector3 targetPosition;
    private int currentWaypointIndex = 0;
    private bool hasPath = false;

    // [SerializeField] private Transform targetPositionTransform;
    [Range(0, 70f)] [SerializeField] private float stoppingDistance = 10f;
    [Range(0, 70f)] [SerializeField] private float stoppingSpeed = 20f;
    [Range(0, 20f)] [SerializeField] private float reachedTargetDistance = 3f;
    // [Range(0, 1f)] [SerializeField] private float speedFactor = 0.5f;  // How much speed affects stopping distance

    // [Range(0, 70f)] [SerializeField] private float baseStoppingDistance = 10f;

    void PrintAvailableNavMeshAreas()
    {
        // Find all NavMesh components in the scene
        NavMeshSurface[] navMeshSurfaces = FindObjectsOfType<NavMeshSurface>();

        Debug.Log("Available NavMesh Area Indexes:");

        foreach (NavMeshSurface navMeshSurface in navMeshSurfaces)
        {
            // Assuming NavMeshSurface stores the area index in its area field
            Debug.Log($"Area Index: {navMeshSurface}");
        }
    }
    
    private void Awake()
    {
        PrintAvailableNavMeshAreas();

        bikeDriver = GetComponent<BicycleVehicle>();
        navMeshPath = new NavMeshPath();

        //bikeAgent = GetComponent<NavMeshAgent>();
        //bikeAgent.updateRotation = false; // We control the rotation manually
        //bikeAgent.updatePosition = true; // Allow NavMeshAgent to update position
    }

    private void Update()
    {
        // Set target position using mouse click
        if (Input.GetMouseButtonDown(0))
        {
            SetTargetPositionFromMouse();
        }

        // Follow the path if it exists
        if (hasPath && waypoints != null && currentWaypointIndex < waypoints.Length)
        {
            MoveToNextWaypoint();
        }
    }

    private void SetTargetPositionFromMouse()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 targetPosition = hit.point;

            // Calculate the path to the target position
            if (NavMesh.CalculatePath(transform.position, targetPosition, NavMesh.AllAreas, navMeshPath))
            {
                waypoints = navMeshPath.corners; // Get waypoints from the calculated path
                currentWaypointIndex = 0;
                hasPath = true;
                bikeDriver.SetBrake(false);
                Debug.Log($"Path calculated with {waypoints.Length} waypoints.");
            }
            else
            {
                Debug.Log("Failed to calculate path.");
            }
        }
    }

    private void MoveToNextWaypoint()
    {
        Vector3 targetPosition = waypoints[currentWaypointIndex];
// float reachedTargetDistance = 3f;
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        float forwardInput = 1f;
        float turnInput = 0f;

        // Debug.Log($"Distance to Target: {distanceToTarget}");
        
        if (distanceToTarget > reachedTargetDistance)
        {
            Vector3 dirToMovePosition = (targetPosition - transform.position).normalized;
            float angleToDir = Vector3.SignedAngle(transform.forward, dirToMovePosition, Vector3.up);

            // Debug.Log($"Direction to Target: {dirToMovePosition}");
            // Debug.Log($"Angle to Target: {angleToDir}");

            turnInput = angleToDir / 90f;
            // turnInput = angleToDir > 0 ? 1f : -1f;

            // float stoppingDistance = 10f;
            // float stoppingSpeed = 25f;
            // float dynamicStoppingDistance = baseStoppingDistance + (bikeDriver.GetSpeed() * speedFactor);

            // if (distanceToTarget < dynamicStoppingDistance && bikeDriver.GetSpeed() > 0f)
            // {
            //     forwardInput = -1f;
            //     bikeDriver.SetBrake(true);
            // }
            //
            if (distanceToTarget < stoppingDistance && bikeDriver.GetSpeed() > stoppingSpeed)
            {
                forwardInput = -1f;
                bikeDriver.SetBrake(true);
            }
        }
        else
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length)
            {
                hasPath = false;
                forwardInput = 0f; // Stop
                turnInput = 0f;
                bikeDriver.SetBrake(true);
                Debug.Log("Destination reached!");
            }
            // forwardInput = 0f;
            // turnInput = 0f;
            // bikeDriver.SetBrake(true);
            // forwardInput = 0f;
            // turnInput = 0f;
            // if (bikeDriver.GetSpeed() > 10f)
            // {
            //     forwardInput = -1f;
            //     // bool braking = true;
            //     bikeDriver.SetBrake(true);
            // }
            // else
            // {
            //     forwardInput = 0f;
            // }
        }

        bikeDriver.SetInput(forwardInput, turnInput);
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


// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.AI;
//
// public class BikeDriverAI : MonoBehaviour
// {
//     private BicycleVehicle bikeDriver;
//     private 
//     public NavMeshAgent bikeAgent; // NavMeshAgent for the bike
//     private Vector3 targetPosition;
//     private bool hasPath = false;
//
//     [Range(0, 70f)] [SerializeField] private float stoppingDistance = 5f;
//     [Range(0, 20f)] [SerializeField] private float reachedTargetDistance = 2f;
//
//     private void Awake()
//     {
//         bikeDriver = GetComponent<BicycleVehicle>();
//
//         // Ensure the NavMeshAgent is set up correctly
//         bikeAgent = GetComponent<NavMeshAgent>();
//         bikeAgent.updateRotation = false; // We control the rotation manually
//         bikeAgent.updatePosition = true; // Allow NavMeshAgent to update position
//         
//         if (bikeAgent == null)
//         {
//             Debug.LogError("NavMeshAgent component missing on the bike!");
//         }
//     }
//
//     private void Update()
//     {
//         // Set target position using mouse click
//         if (Input.GetMouseButtonDown(0))
//         {
//             SetTargetPositionFromMouse();
//         }
//
//         // If the agent has a path, control the bike to follow it
//         if (hasPath)
//         {
//             FollowPath();
//         }
//     }
//
//     private void SetTargetPositionFromMouse()
//     {
//         Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//         if (Physics.Raycast(ray, out RaycastHit hit))
//         {
//             targetPosition = hit.point;
//             bikeAgent.SetDestination(targetPosition);
//             hasPath = true;
//             bikeDriver.SetBrake(false); // Release the brake to allow movement
//             Debug.Log($"New Target Position: {targetPosition}");
//         }
//     }
//
//     private void FollowPath()
//     {
//         if (bikeAgent.pathPending || bikeAgent.remainingDistance <= 0.1f)
//         {
//             return; // Wait until the path is ready
//         }
//
//         float distanceToTarget = bikeAgent.remainingDistance;
//         float forwardInput = 0f;
//         float turnInput = 0f;
//
//         if (distanceToTarget > reachedTargetDistance)
//         {
//             // Determine direction to the next waypoint
//             Vector3 direction = bikeAgent.desiredVelocity.normalized;
//
//             // Align the bike's forward direction with the movement direction
//             float angleToTarget = Vector3.SignedAngle(transform.forward, direction, Vector3.up);
//             turnInput = Mathf.Clamp(angleToTarget / 45f, -1f, 1f);
//
//             // Forward input is positive unless we are braking
//             if (distanceToTarget > stoppingDistance || bikeDriver.GetSpeed() <= 5f)
//             {
//                 forwardInput = 1f; // Move forward
//                 bikeDriver.SetBrake(false);
//             }
//             else if (bikeDriver.GetSpeed() > 5f)
//             {
//                 forwardInput = -1f; // Apply reverse force to slow down
//                 bikeDriver.SetBrake(true);
//             }
//         }
//         else
//         {
//             forwardInput = 0f;
//             turnInput = 0f;
//             bikeDriver.SetBrake(true);
//             hasPath = false; // Stop path-following
//         }
//         
//         if (!bikeAgent.isOnNavMesh)
//         {
//             Debug.LogError("NavMeshAgent is not on a valid NavMesh.");
//             return;
//         }
//
//         if (bikeAgent.pathPending || bikeAgent.remainingDistance > 0.1f)
//         {
//             Debug.Log($"Remaining Distance: {bikeAgent.remainingDistance}");
//             return;
//         }
//
//         // Logic when the agent reaches its destination
//         Debug.Log("Target reached!");
//
//         bikeDriver.SetInput(forwardInput, turnInput);
//     }
// }


// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.AI;
//
// public class BikeDriverAI : MonoBehaviour
// {
//     private BicycleVehicle bikeDriver;
//     public NavMeshAgent bikeAgent;
//     private Vector3 targetPosition;
//     private bool hasPath = false;
//
//     // [SerializeField] private Transform targetPositionTransform;
//     [Range(0, 70f)][SerializeField] private float stoppingDistance = 10f;
//     // [Range(0, 70f)][SerializeField] private float stoppingSpeed = 20f;
//     [Range(0, 20f)][SerializeField] private float reachedTargetDistance = 3f;
//     // [Range(0, 1f)] [SerializeField] private float speedFactor = 0.5f;  // How much speed affects stopping distance
//
//     // [Range(0, 70f)] [SerializeField] private float baseStoppingDistance = 10f;
//     
//     private void Awake()
//     {
//         bikeDriver = GetComponent<BicycleVehicle>();
//         
//         bikeAgent = GetComponent<NavMeshAgent>();
//         bikeAgent.updateRotation = false; // We control the rotation manually
//         bikeAgent.updatePosition = true; // Allow NavMeshAgent to update position
//     }
//
//     private void Update()
//     {
//         // Set target position using mouse click
//         if (Input.GetMouseButtonDown(0))
//         {
//             SetTargetPositionFromMouse();
//         }
//
//         if (hasPath)
//         {
//             FollowPath();
//         }
//     }
//     
//     private void SetTargetPositionFromMouse()
//     {
//         Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//         if (Physics.Raycast(ray, out RaycastHit hit))
//         {
//             targetPosition = hit.point;
//             bikeAgent.SetDestination(targetPosition);
//             hasPath = true;
//             bikeDriver.SetBrake(false); // Release the brake to allow movement
//             Debug.Log($"New Target Position: {targetPosition}");
//         }
//     }
//     
//     private void FollowPath(){
//
//         if (bikeAgent.pathPending || bikeAgent.remainingDistance <= 0.1f)
//         {
//             return; // Wait until the path is ready
//         }
//         
//         float distanceToTarget = bikeAgent.remainingDistance;
//         float forwardInput = 0f;
//         float turnInput = 0f;
//         
//     // float reachedTargetDistance = 3f;
//         // float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
//         // float forwardInput = 1f;
//         // float turnInput = 0f;
//
//         // Debug.Log($"Distance to Target: {distanceToTarget}");
//         
//         if (distanceToTarget > reachedTargetDistance)
//         {
//             Vector3 dirToMovePosition = (targetPosition - transform.position).normalized;
//             float angleToDir = Vector3.SignedAngle(transform.forward, dirToMovePosition, Vector3.up);
//
//             // Debug.Log($"Direction to Target: {dirToMovePosition}");
//             // Debug.Log($"Angle to Target: {angleToDir}");
//
//             turnInput = angleToDir / 90f;
//             // turnInput = angleToDir > 0 ? 1f : -1f;
//
//             // float stoppingDistance = 10f;
//             // float stoppingSpeed = 25f;
//             // float dynamicStoppingDistance = baseStoppingDistance + (bikeDriver.GetSpeed() * speedFactor);
//
//             // if (distanceToTarget < dynamicStoppingDistance && bikeDriver.GetSpeed() > 0f)
//             // {
//             //     forwardInput = -1f;
//             //     bikeDriver.SetBrake(true);
//             // }
//             //
//             if (distanceToTarget < stoppingDistance && bikeDriver.GetSpeed() > stoppingSpeed)
//             {
//                 forwardInput = -1f;
//                 bikeDriver.SetBrake(true);
//             }
//         }
//         else
//         {
//             forwardInput = 0f;
//             turnInput = 0f;
//             bikeDriver.SetBrake(true);
//             // forwardInput = 0f;
//             // turnInput = 0f;
//             // if (bikeDriver.GetSpeed() > 10f)
//             // {
//             //     forwardInput = -1f;
//             //     // bool braking = true;
//             //     bikeDriver.SetBrake(true);
//             // }
//             // else
//             // {
//             //     forwardInput = 0f;
//             // }
//         }
//
//         bikeDriver.SetInput(forwardInput, turnInput);
//     }
//
//     // private void SetTargetPositionFromMouse()
//     // {
//     //     Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//     //     if (Physics.Raycast(ray, out RaycastHit hit))
//     //     {
//     //         targetPosition = hit.point;
//     //         bikeDriver.SetBrake(false);
//     //         Debug.Log($"New Target Position: {targetPosition}");
//     //         bikeAgent.SetDestination(targetPosition);
//     //     }
//     // }
// }


// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class BikeDriverAI : MonoBehaviour
// {
//     private BicycleVehicle bikeDriver;
//     private Vector3 targetPosition;
//     [SerializeField] private float forwardInput = 1; // Default forward input
//     [SerializeField] private float turnInput = 1;   // Default turn input
//     [SerializeField] private Transform targetPositionTransform;
//
//     private void Awake()
//     {
//         bikeDriver = GetComponent<BicycleVehicle>();
//     }
//
//     private void Update()
//     {
//         SetTargetPosition(SetTargetPositionFromMouse()); // Set target position using mouse click
//         float forwardInput = 0f;
//         float turnInput = 0f;
//
//         float reachedTargetDistance = 7f;
//         float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
//
//         if (distanceToTarget > reachedTargetDistance)
//         {
//             Vector3 dirToMovePosition = (targetPosition - transform.position).normalized;
//             float dot = Vector3.Dot(transform.forward, dirToMovePosition);
//             Debug.Log(dot);
//             Debug.Log(targetPosition);
//             Debug.Log(distanceToTarget);
//
//             float angleToDir = Vector3.SignedAngle(transform.forward, dirToMovePosition, Vector3.up);
//             
//             if (angleToDir > 0)
//             {
//                 turnInput = 1f; // turn right
//             }
//             else
//             {
//                 turnInput = -1f; // turn left
//             }
//         }
//         else
//         {
//             if (bikeDriver.GetSpeed() > 15f)
//             {
//                 forwardInput = -1f;
//             }
//             else
//             {
//                 forwardInput = 0f;
//             }
//             turnInput = 0f;
//         }
//
//         bikeDriver.SetInput(forwardInput, turnInput);
//     }
//
//     Vector3 SetTargetPositionFromMouse()
//     {
//         if (Input.GetMouseButtonDown(0))
//         {
//             Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
//             if (Physics.Raycast(ray, out RaycastHit hit))
//             {
//                 return hit.point;
//             }
//         }
//     }
//
//     private void SetTargetPosition(Vector3 targetPosition)
//     {
//         this.targetPosition = targetPosition;
//     }
// }

//
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class BikeDriverAI : MonoBehaviour
// {
//     private BicycleVehicle bikeDriver;
//     [SerializeField] private float forwardInput = 1; // Default forward input
//     [SerializeField] private float turnInput = 1;   // Default turn input
//     private UnityEngine.AI.NavMeshPath globalPath;  // AI-generated path
//     private int currentPathIndex = 0;               // Index of the current waypoint in the path
//     private bool isPathComputed = false;            // Flag to check if path is computed
//     public Camera cam;                             // Reference to the camera for path calculation
//
//     private void Awake()
//     {
//         bikeDriver = GetComponent<BicycleVehicle>();
//     }
//
//     private void Update()
//     {
//         // Compute path if needed
//         if (!isPathComputed)
//         {
//             ComputeGlobalPath();
//         }
//
//         // Follow the path
//         if (isPathComputed && currentPathIndex < globalPath.Length)
//         {
//             Vector3 targetPoint = globalPath[currentPathIndex];
//             Vector3 direction = (targetPoint - transform.position).normalized;
//
//             // Calculate forward and turn inputs based on the direction
//             forwardInput = Mathf.Clamp(Vector3.Dot(transform.forward, direction), -1, 1);
//             turnInput = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
//             
//             // Set bike inputs
//             bikeDriver.SetInput(forwardInput, turnInput);
//
//             // Check if the bike has reached the next waypoint
//             float distance = Vector3.Distance(transform.position, targetPoint);
//             if (distance < 0.5f) // Adjust this threshold value for accuracy
//             {
//                 currentPathIndex++;
//             }
//         }
//     }
//
//     void ComputeGlobalPath()
//     {
//         Ray ray = cam.ScreenPointToRay(Input.mousePosition);
//         if (Physics.Raycast(ray, out RaycastHit hit))
//         {
//             UnityEngine.AI.NavMeshPath path = new UnityEngine.AI.NavMeshPath();
//             UnityEngine.AI.NavMesh.CalculatePath(transform.position, hit.point, UnityEngine.AI.NavMesh.AllAreas, path);
//             if (path.status == UnityEngine.AI.NavMeshPathStatus.PathComplete)
//             {
//                 globalPath = path.corners;
//                 currentPathIndex = 0;
//                 isPathComputed = true;
//             }
//         }
//     }
// }

// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class BikeDriverAI : MonoBehaviour
// {
//     private BicycleVehicle bikeDriver;
//     [SerializeField] private float forwardInput = 1; // Default forward input
//     [SerializeField] private float turnInput = 1;   // Default turn input
//
//     private void Awake()
//     {
//         bikeDriver = GetComponent<BicycleVehicle>();
//     }
//
//     private void Update()
//     {
//         bikeDriver.SetInput(forwardInput, turnInput);
//     }
// }