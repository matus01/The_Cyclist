using System.Collections.Generic;

namespace UnityEngine.AI
{
    [ExecuteInEditMode]
    [AddComponentMenu("Navigation/NavMeshModifierVolume", 31)]
    [HelpURL("https://github.com/Unity-Technologies/NavMeshComponents#documentation-draft")]
    public class NavMeshModifierVolume : MonoBehaviour
    {
        [SerializeField]
        Vector3 m_Size = new Vector3(4.0f, 3.0f, 4.0f);
        public Vector3 size { get { return m_Size; } set { m_Size = value; } }

        [SerializeField]
        Vector3 m_Center = new Vector3(0, 1.0f, 0);
        public Vector3 center { get { return m_Center; } set { m_Center = value; } }

        [SerializeField]
        int m_Area;
        public int area { get { return m_Area; } set { m_Area = value; } }

        // List of agent types the modifier is applied for.
        // Special values: empty == None, m_AffectedAgents[0] =-1 == All.
        [SerializeField]
        List<int> m_AffectedAgents = new List<int>(new int[] { -1 });    // Default value is All

        static readonly List<NavMeshModifierVolume> s_NavMeshModifiers = new List<NavMeshModifierVolume>();

        public static List<NavMeshModifierVolume> activeModifiers
        {
            get { return s_NavMeshModifiers; }
        }

        private void Reset()
        {
            ResetSizeNCenter();
        }

        [ContextMenu("Reset Size And Center")]
        private void ResetSizeNCenter()
        {
            Renderer renderer = GetComponent<Renderer>();
            if (renderer != null)
            {
                m_Size = renderer.bounds.extents * 2f;
                m_Center = renderer.bounds.center;
            }
        }

        void OnEnable()
        {
            if(Application.isPlaying)
            {
                return;
            }

            if (!s_NavMeshModifiers.Contains(this))
                s_NavMeshModifiers.Add(this);
        }

        void OnDisable()
        {
            if (Application.isPlaying)
            {
                return;
            }

            s_NavMeshModifiers.Remove(this);
        }

        public bool AffectsAgentType(int agentTypeID)
        {
            if (m_AffectedAgents.Count == 0)
                return false;
            if (m_AffectedAgents[0] == -1)
                return true;
            return m_AffectedAgents.IndexOf(agentTypeID) != -1;
        }
    }
}
