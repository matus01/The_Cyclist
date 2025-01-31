using UnityEngine;

public class Crosswalk : MonoBehaviour
{
    public int intersectionID = -1;

    private void OnTriggerEnter(Collider other)
    {   
        if (other.CompareTag("Pedestrian"))
        {
            //Debug.Log("pedcrossing " + intersectionID);
            // Add agent to intersection on entry
            if (intersectionID >= 0)
            {
                IntersectionManager.Instance.AddAgentToIntersection(intersectionID);
            }
            else
            {
                Debug.LogWarning("Invalid intersection ID at entry waypoint.");
            }
            
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Pedestrian"))
        {
            // Remove agent from intersection on exit
            if (intersectionID >= 0)
            {
                IntersectionManager.Instance.RemoveAgentFromIntersection(intersectionID);
            }
            else
            {
                Debug.LogWarning("Invalid intersection ID at exit waypoint.");
            }
        }
    }
}
