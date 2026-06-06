using UnityEngine;
using UnityEngine.InputSystem;

using Interaction;

public class DragToCreatePhoto : MonoBehaviour
{
#region Handler
    [System.Serializable]
    private class MaskHandler
    {
        [SerializeField] public GameObject maskPrerfab;
        private SpriteRenderer currentMaskRender;

        internal void UpdateMask(Vector3 pos, Vector2 size)
        {
            var mask = GetMaskRender();
            mask.transform.position = pos;
            mask.size = size;
        }
        internal void ClearMask()
        {
            if(currentMaskRender!=null)
            {
                Destroy(currentMaskRender.gameObject);
                currentMaskRender = null;
            }
        }
        SpriteRenderer GetMaskRender()
        {
            if(currentMaskRender!=null)
                return currentMaskRender;
            currentMaskRender = Instantiate(maskPrerfab).GetComponent<SpriteRenderer>();
            return currentMaskRender;
        }
    }
    // private static class FrameDetector
    // {
    //     public static InterestPointBasic DetectSelectingFrame(Vector2 min, Vector2 max)
    //     {
    //         var overlap = Physics2D.OverlapArea(min, max, 1<<InteractionService.InteractableLayer);
    //         if(overlap == null)
    //             return null;
    //         if(overlap.TryGetComponent<InterestPointBasic>(out var interestPoint))
    //         {
    //             return interestPoint;
    //         }
    //         return null;
    //     }
    // }
#endregion

    [SerializeField] private GameObject framePrefab;
    [SerializeField] private GameObject photoPrefab;
    [SerializeField] private Sprite dragFrame;
    [SerializeField] private Sprite[] poolFrames;
    [SerializeField] private Vector2 maxSize;
    [SerializeField] private Vector2 minSize;
    [SerializeField] private MaskHandler maskHandler;
    [SerializeField] private LocationController locationController;
    
    private PlayerInputAction.PlayerActions playerActions;
    private PhotoFrame currentFrame;
    private Vector2 minPoint;
    private Vector2 maxPoint;
    private int layerIndex;
    private int frameIndex;

    private const float FRAME_OFFSET = 0.01f;

    void Start()
    {
        playerActions = new PlayerInputAction().Player;
        Service.Shuffle(ref poolFrames);
        frameIndex = 0;

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
            Vector2 targetPoint = ClampPoint(worldPos);

            maxPoint = Vector2.Lerp(maxPoint, targetPoint, Time.deltaTime * 40);
            var rect = Rect.MinMaxRect(minPoint.x, minPoint.y, maxPoint.x, maxPoint.y);
            
            currentFrame.UpdateFrame(rect, FRAME_OFFSET);
            maskHandler.UpdateMask(currentFrame.transform.position, rect.size);
        }
    }
    void OnInteract(InputAction.CallbackContext context)
    {
        if(currentFrame == null)
        {
            var worldPos = Camera.main.ScreenToWorldPoint(playerActions.PointerPosition.ReadValue<Vector2>());
            worldPos.z = 0;
            minPoint = worldPos;
            maxPoint = worldPos;

            var go = Instantiate(framePrefab, worldPos, Quaternion.identity);
            currentFrame = go.GetComponent<PhotoFrame>();
            currentFrame.Init(worldPos, layerIndex, dragFrame);
        }
    }
    void OnRelease(InputAction.CallbackContext context)
    {
        if(currentFrame != null)
        {
            var rect = Rect.MinMaxRect(minPoint.x, minPoint.y, maxPoint.x, maxPoint.y);
            
            if(Mathf.Abs(rect.width) < minSize.x || Mathf.Abs(rect.height) < minSize.y)
            {
                CancelFrame();
                return;
            }

            minPoint = rect.min;
            maxPoint = rect.max;

            Vector2 min = new Vector2(Mathf.Min(minPoint.x, maxPoint.x), Mathf.Min(minPoint.y, maxPoint.y));
            Vector2 max = new Vector2(Mathf.Max(minPoint.x, maxPoint.x), Mathf.Max(minPoint.y, maxPoint.y));
            rect = Rect.MinMaxRect(min.x, min.y, max.x, max.y);

            minPoint = min;
            maxPoint = max;

            FixPhoto(rect, locationController.GetLocation());
        }
    }
    Vector3 ClampPoint(Vector2 maxPoint)
    {
        if(maxPoint.x > maxSize.x + minPoint.x)
            maxPoint.x = minPoint.x + maxSize.x;
        else if(maxPoint.x < -maxSize.x + minPoint.x)
            maxPoint.x = minPoint.x - maxSize.x;
        if(maxPoint.y > maxSize.y + minPoint.y)
            maxPoint.y = minPoint.y + maxSize.y;
        else if(maxPoint.y < -maxSize.y + minPoint.y)
            maxPoint.y = minPoint.y - maxSize.y;
        return maxPoint;
    }
    void FixPhoto(Rect rect, Location distributor)
    {
        layerIndex++;
        currentFrame.SetFrameStyle(poolFrames[frameIndex]);
        frameIndex++;
        if(frameIndex >= poolFrames.Length)
        {
            frameIndex = 0;
            Service.Shuffle(ref poolFrames);
        }
        currentFrame.UpdateFrame(rect, FRAME_OFFSET);

        var photo = distributor.GetLocationSprite(false);
        currentFrame.FixPhoto(photoPrefab, photo, layerIndex);

        currentFrame = null;
        maskHandler.ClearMask();
    }
    void CancelFrame()
    {
        maskHandler.ClearMask();
        Destroy(currentFrame.gameObject);
        currentFrame = null;
    }
}