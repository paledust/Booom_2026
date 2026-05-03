using System;
using UnityEngine;

namespace Interaction
{
    public abstract class Interactable : MonoBehaviour
    {
        [SerializeField] private Collider m_collider;
        public bool m_isInteractable{get; protected set;} = true;
        void Reset()
        {
            m_collider = GetComponent<Collider>();
        }
        public abstract void OnInteract(InteractionController controller, Vector3 hitPoint);
        public abstract void OnRelease();
        public abstract void OnHover();
        public abstract void OnExitHover();
    }
}
