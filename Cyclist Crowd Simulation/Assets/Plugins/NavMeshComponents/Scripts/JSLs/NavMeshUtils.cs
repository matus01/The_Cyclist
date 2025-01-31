
namespace UnityEngine.AI
{
    public static class NavMeshUtils
    {
        private const float ADDITIONAL_VALID_DISTANCE = 1.5f;
        private static NavMeshPath m_navMeshPath = new NavMeshPath();
        private static NavMeshHit m_navMeshHit;
        private static Vector3 m_cacheDestination;

        public static bool HasTargetValidPath(Vector3 sourcePosition, Vector3 targetPosition, float validDistance)
        {
            if ((targetPosition - sourcePosition).sqrMagnitude <= 1)
            {
                return true;
            }

            if (NavMesh.CalculatePath(sourcePosition, targetPosition, NavMesh.AllAreas, m_navMeshPath))
            {
                if (m_navMeshPath.corners.Length == 0)
                {
                    if (m_navMeshPath.status != NavMeshPathStatus.PathComplete)
                    {
                        return false;
                    }
                }
                else
                {
                    m_cacheDestination = m_navMeshPath.corners[m_navMeshPath.corners.Length - 1];
                    if ((m_cacheDestination - targetPosition).sqrMagnitude > validDistance)
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        public static bool HasMouseValidPath(Vector3 sourcePosition, Vector3 targetPosition, float validDistance)
        {
            CalculatePath(targetPosition, sourcePosition, out m_cacheDestination, NavMesh.AllAreas);

            if ((m_cacheDestination - sourcePosition).sqrMagnitude <= 1)
            {
                return true;
            }

            if (NavMesh.CalculatePath(sourcePosition, m_cacheDestination, NavMesh.AllAreas, m_navMeshPath))
            {
                if (m_navMeshPath.corners.Length == 0)
                {
                    if ((targetPosition - sourcePosition).sqrMagnitude > 1)
                    {
                        if (m_navMeshPath.status != NavMeshPathStatus.PathComplete)
                        {
                            return false;
                        }
                    }
                }
                else
                {
                    Vector3 lastCorner = m_navMeshPath.corners[m_navMeshPath.corners.Length - 1];
                    if ((lastCorner - sourcePosition).sqrMagnitude > validDistance &&
                        (lastCorner - targetPosition).sqrMagnitude > validDistance)
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        public static float GetValidDistance(Collider collider)
        {
            if(collider == null)
            {
                return ADDITIONAL_VALID_DISTANCE;
            }

            float distance = collider.bounds.size.x;
            if(collider.bounds.size.y < distance)
            {
                distance = collider.bounds.size.y;
            }

            if (collider.bounds.size.z < distance)
            {
                distance = collider.bounds.size.z;
            }

            distance /= 2;
            distance += ADDITIONAL_VALID_DISTANCE;

            return distance * distance;
        }

        public static bool CalculatePath(Vector3 sourcePosition, Vector3 targetPosition, out Vector3 destination, int areaMask = NavMesh.AllAreas)
        {
            destination = targetPosition;

            NavMesh.CalculatePath(sourcePosition, destination, areaMask, m_navMeshPath);
            if (m_navMeshPath.status != NavMeshPathStatus.PathComplete)
            {
                destination = GetValidPosition(sourcePosition, targetPosition, areaMask);
                NavMesh.CalculatePath(sourcePosition, destination, areaMask, m_navMeshPath);

                return m_navMeshPath.status == NavMeshPathStatus.PathComplete;
            }

            return true;
        }

        private static Vector3 GetValidPosition(Vector3 sourcePosition, Vector3 targetPosition, int areaMask = NavMesh.AllAreas)
        {
            Vector3 validPosition = targetPosition;

            if (NavMesh.Raycast(sourcePosition, validPosition, out m_navMeshHit, areaMask))
            {
                validPosition = m_navMeshHit.position;
            }

            return validPosition;
        }

        public static Vector3 SamplePosition(Vector3 destination, int areaMask = NavMesh.AllAreas)
        {
            if (NavMesh.SamplePosition(destination, out m_navMeshHit, Mathf.Infinity, NavMesh.AllAreas))
            {
                destination = m_navMeshHit.position;
            }

            return destination;
        }
    }
}