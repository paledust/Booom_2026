using UnityEngine;
using UnityEngine.InputSystem;

public class DragToCreatePhoto : MonoBehaviour
{
    [SerializeField] private GameObject photoPrefab;
    private PlayerInputAction.PlayerActions playerActions;
    private PhotoFrame currentFrame;
    private Vector2 minPoint;
    private Vector2 maxPoint;
    private int layerIndex;

    void Start()
    {
        playerActions = new PlayerInputAction().Player;
        playerActions.Enable();
        playerActions.Interact.performed += OnInteract;
        playerActions.Interact.canceled += OnRelease;
    }
    void OnDisable()
    {
        playerActions.Interact.performed -= OnInteract;
        playerActions.Interact.canceled -= OnRelease;
        playerActions.Disable();
    }
    void Update()
    {
        if(currentFrame!=null)
        {
            var worldPos = Camera.main.ScreenToWorldPoint(playerActions.PointerPosition.ReadValue<Vector2>());
            worldPos.z = 0;
            maxPoint = worldPos;
            var rect = Rect.MinMaxRect(minPoint.x, minPoint.y, maxPoint.x, maxPoint.y);
            currentFrame.UpdateFrame(rect);
        }
    }
    void OnInteract(InputAction.CallbackContext context)
    {
        if(currentFrame == null)
        {
            var worldPos = Camera.main.ScreenToWorldPoint(playerActions.PointerPosition.ReadValue<Vector2>());
            worldPos.z = 0;
            minPoint = worldPos;
            var go = Instantiate(photoPrefab, worldPos, Quaternion.identity);
            currentFrame = go.GetComponent<PhotoFrame>();
            currentFrame.Init(worldPos, layerIndex);
        }
    }
    void OnRelease(InputAction.CallbackContext context)
    {
        if(currentFrame != null)
        {
            var worldPos = Camera.main.ScreenToWorldPoint(playerActions.PointerPosition.ReadValue<Vector2>());
            worldPos.z = 0;
            maxPoint = worldPos;
            var rect = Rect.MinMaxRect(minPoint.x, minPoint.y, maxPoint.x, maxPoint.y);
            currentFrame.UpdateFrame(rect);
            currentFrame.FixPhoto();
            layerIndex++;
            currentFrame = null;
        }
    }
}