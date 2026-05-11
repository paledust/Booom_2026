using UnityEngine;
using UnityEngine.InputSystem;

public class DragToCreatePhoto : MonoBehaviour
{
    [SerializeField] private GameObject photoPrefab;
    [SerializeField] private Sprite[] poolFrames;
    [SerializeField] private Sprite[] poolPhotos;
    [SerializeField] private Vector2 maxSize;
    [SerializeField] private Vector2 minSize;
    private PlayerInputAction.PlayerActions playerActions;
    private PhotoFrame currentFrame;
    private Vector2 minPoint;
    private Vector2 maxPoint;
    private int layerIndex;
    private int frameIndex;
    private int photoIndex;

    void Start()
    {
        playerActions = new PlayerInputAction().Player;
        Service.Shuffle(ref poolFrames);
        Service.Shuffle(ref poolPhotos);
        frameIndex = 0;
        photoIndex = 0;

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
            if(rect.width > maxSize.x)
                rect.xMax = rect.xMin + maxSize.x;
            else if(rect.width < -maxSize.x)
                rect.xMax = rect.xMin - maxSize.x;
            if(rect.height > maxSize.y)
                rect.yMax = rect.yMin + maxSize.y;
            else if(rect.height < -maxSize.y)
                rect.yMax = rect.yMin - maxSize.y;
            minPoint = rect.min;
            maxPoint = rect.max;
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
            currentFrame.Init(worldPos, layerIndex, poolFrames[frameIndex]);
            frameIndex++;
            if(frameIndex >= poolFrames.Length)
            {
                frameIndex = 0;
                Service.Shuffle(ref poolFrames);
            }
        }
    }
    void OnRelease(InputAction.CallbackContext context)
    {
        if(currentFrame != null)
        {
            var rect = Rect.MinMaxRect(minPoint.x, minPoint.y, maxPoint.x, maxPoint.y);
            if(Mathf.Abs(rect.width) < minSize.x || Mathf.Abs(rect.height) < minSize.y)
            {
                Destroy(currentFrame.gameObject);
                currentFrame = null;
                return;
            }
            currentFrame.UpdateFrame(rect);
            currentFrame.FixPhoto(poolPhotos[photoIndex]);
            photoIndex++;
            if(photoIndex >= poolPhotos.Length)
            {
                photoIndex = 0;
                Service.Shuffle(ref poolPhotos);
            }
            layerIndex++;
            currentFrame = null;
        }
    }
}