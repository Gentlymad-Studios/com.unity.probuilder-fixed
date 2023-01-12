using UnityEngine;
using UnityEngine.Serialization;

namespace UnityEngine.ProBuilder
{
    /// <summary>
    /// Provides some additional functionality to GameObjects, like managing visiblity and colliders.
    /// </summary>
    [DisallowMultipleComponent]
    [AddComponentMenu("")]
    sealed class Entity : MonoBehaviour
    {
        public UnityEngine.ProBuilder.EntityType entityType { get { return m_EntityType; } }

        [SerializeField]
        [HideInInspector]
        [FormerlySerializedAs("_entityType")]
        UnityEngine.ProBuilder.EntityType m_EntityType;

        /// <summary>
        /// Performs Entity specific initialization tasks (turn off renderer for nodraw faces, hide colliders, etc)
        /// </summary>
        public void Awake()
        {
            MeshRenderer mr = GetComponent<MeshRenderer>();

            if (!mr) return;

            switch (entityType)
            {
                case UnityEngine.ProBuilder.EntityType.Occluder:
                    break;

                case UnityEngine.ProBuilder.EntityType.Detail:
                    break;

                case UnityEngine.ProBuilder.EntityType.Trigger:
                    mr.enabled = false;
                    break;

                case UnityEngine.ProBuilder.EntityType.Collider:
                    mr.enabled = false;
                    break;
            }
        }

        /// <summary>
        /// Set the entity type.
        /// </summary>
        /// <param name="t"></param>
        public void SetEntity(UnityEngine.ProBuilder.EntityType t)
        {
            m_EntityType = t;
        }
    }
}
