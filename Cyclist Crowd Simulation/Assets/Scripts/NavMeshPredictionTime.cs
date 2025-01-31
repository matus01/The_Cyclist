using UnityEngine;

public class NavMeshPredictionTime : MonoBehaviour
{
    void Awake()
    {
        UnityEngine.AI.NavMesh.avoidancePredictionTime = 5;
    }
}
