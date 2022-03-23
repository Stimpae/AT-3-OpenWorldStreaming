using UnityEngine;

namespace Objects.Interactables
{
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(MeshFilter))]
    public class InteractableObject : MonoBehaviour, IInteractable
    {
        public Material hoverMaterial;
        public Material selectionMaterial;

        private MeshRenderer m_meshRenderer;
        private Material m_defaultMaterial;
        
        void Start()
        {
            m_meshRenderer = gameObject.GetComponent<MeshRenderer>();
            m_defaultMaterial = m_meshRenderer.material;
        }
    
        public virtual void Hover()
        {
            m_meshRenderer.material = hoverMaterial;
        }

        public virtual void UnHover()
        {
            m_meshRenderer.material = m_defaultMaterial;
        }

        public virtual void Select()
        {
            m_meshRenderer.material = selectionMaterial;
        }

        public virtual void Deselect()
        {
            m_meshRenderer.material = m_defaultMaterial;
        }

        public virtual void Interact()
        {
            Destroy(gameObject);
        }
    }
}
