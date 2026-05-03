using UnityEngine;
using UnityEngine.InputSystem;

namespace Interaction
{
    public class InteractionController : MonoBehaviour
    {
        private PlayerInputAction.PlayerActions playerInputAction;
        private Interactable hoveringInteractable;
        private Interactable holdingInteractable;
        private Vector2 pointerScrPos;
        private Vector3 hoverPos;
        private Camera mainCam;

        void Start()
        {
            mainCam = Camera.main;
            playerInputAction = new PlayerInputAction().Player;
            playerInputAction.Enable();
            playerInputAction.Interact.performed += OnInteract;
            playerInputAction.Interact.canceled += OnRelease;
        }
        void OnDisable()
        {
            playerInputAction.Interact.performed -= OnInteract;
            playerInputAction.Interact.canceled -= OnRelease;
            playerInputAction.Disable();
        }
        void Update()
        {
            if(holdingInteractable != null)
            {
                return;
            }
            pointerScrPos = playerInputAction.PointerPosition.ReadValue<Vector2>();
            if(Physics.Raycast(mainCam.ScreenPointToRay(pointerScrPos), out RaycastHit hit, 100f, 1<<InteractionService.InteractableLayer))
            {
                if(hit.collider.TryGetComponent(out Interactable interactable))
                {
                    hoverPos = hit.point;
                    if(hoveringInteractable != null)
                    {
                        hoveringInteractable.OnExitHover();
                    }
                    hoveringInteractable = interactable;
                    hoveringInteractable.OnHover();
                }
                else
                {
                    ClearHovering();
                }
            }
            else
            {
                ClearHovering();
            }
        }
        void OnInteract(InputAction.CallbackContext context)
        {
            if(holdingInteractable != null) return;
            if(hoveringInteractable == null) return;
            
            if(hoveringInteractable.m_isInteractable){
                hoveringInteractable.OnInteract(this, hoverPos);
            }
        }
        void OnRelease(InputAction.CallbackContext context)
        {
            if(holdingInteractable != null){
                var holding = holdingInteractable;
                holdingInteractable = null;
                holding.OnRelease();
            }
        }
        void ClearHovering()
        {
            if(hoveringInteractable != null)
            {
                hoveringInteractable.OnExitHover();
                hoveringInteractable = null;
            }
        }
        public void SetHoldingInteractable(Interactable interactable)
        {
            holdingInteractable = interactable;
        }
    }
}
