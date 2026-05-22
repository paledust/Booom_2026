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
    private static class FrameDetector
    {
        public static InterestPointBasic DetectSelectingFrame(Vector2 min, Vector2 max)
        {
            var overlap = Physics2D.OverlapArea(min, max, 1<<InteractionService.InteractableLayer);
            if(overlap == null)
                return null;
            if(overlap.TryGetComponent<InterestPointBasic>(out var interestPoint))
            {
                return interestPoint;
            }
            return null;
        }
    }
#endregion

    [SerializeField] private GameObject photoPrefab;
    [SerializeField] private Sprite[] poolFrames;
    [SerializeField] private Vector2 maxSize;
    [SerializeField] private Vector2 minSize;
    [SerializeField] private MaskHandler maskHandler;
    [SerializeField] private EdgeHandler edgeHandler;
    
    private PlayerInputAction.PlayerActions playerActions;
    private PhotoFrame currentFrame;
    private PhotoDistributor currentPhoto;
    private Vector2 minPoint;
    private Vector2 maxPoint;
    private int layerIndex;
    private int frameIndex;
    private EdgeType currentBlockEdge;

    private const float FRAME_OFFSET = 0.01f;

    void Start()
    {
        currentPhoto = null;
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
            Vector2 targetPoint = worldPos;

            if(edgeHandler.m_hasEdge)
            {
                targetPoint = edgeHandler.GetEdgeCorrectPoint(minPoint, targetPoint, out var alignEdge);
                if(alignEdge == EdgeType.None && currentBlockEdge != EdgeType.None)
                    currentBlockEdge = EdgeType.None;
                if(alignEdge != EdgeType.None && currentBlockEdge == EdgeType.None)
                    currentBlockEdge = alignEdge;
            }

            maxPoint = Vector2.Lerp(maxPoint, targetPoint, Time.deltaTime * 40);
            var rect = Rect.MinMaxRect(minPoint.x, minPoint.y, maxPoint.x, maxPoint.y);
            if(rect.width > maxSize.x)
                rect.xMax = rect.xMin + maxSize.x;
            else if(rect.width < -maxSize.x)
                rect.xMax = rect.xMin - maxSize.x;
            if(rect.height > maxSize.y)
                rect.yMax = rect.yMin + maxSize.y;
            else if(rect.height < -maxSize.y)
                rect.yMax = rect.yMin - maxSize.y;
            
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
            Vector2 min = new Vector2(Mathf.Min(minPoint.x, maxPoint.x), Mathf.Min(minPoint.y, maxPoint.y));
            Vector2 max = new Vector2(Mathf.Max(minPoint.x, maxPoint.x), Mathf.Max(minPoint.y, maxPoint.y));
            var rect = Rect.MinMaxRect(min.x, min.y, max.x, max.y);

            minPoint = rect.min;
            maxPoint = rect.max;

            if(Mathf.Abs(rect.width) < minSize.x || Mathf.Abs(rect.height) < minSize.y)
            {
                CancelFrame();
                return;
            }
            
            if(currentBlockEdge!=EdgeType.None && currentPhoto!=null)
            {
                var edgeConfig = currentPhoto.GetEdgeConfig();
                if((edgeConfig.edge & currentBlockEdge) > 0)
                {
                    FixPhoto(rect, edgeConfig.photoDatas.GetPhotoInstance(), false);
                    edgeHandler.CompleteEdge(currentBlockEdge);
                    currentBlockEdge = EdgeType.None;
                }
                return;
            }

            var interestPoint = FrameDetector.DetectSelectingFrame(rect.min, rect.max);
            if(interestPoint == null)
            {
                CancelFrame();
                return;    
            }

            FixPhoto(rect, interestPoint.GetNextPhoto(), true);
        }
    }
    void FixPhoto(Rect rect, PhotoDistributor photo, bool setAsMainPhoto)
    {

        layerIndex++;
        currentFrame.UpdateFrame(rect, FRAME_OFFSET);
        currentFrame.FixPhoto(photo.GetPhotoPrefab(), layerIndex);

        currentFrame = null;
        maskHandler.ClearMask();

        if(setAsMainPhoto)
        {
            currentPhoto = photo;
            edgeHandler.CreateConstraintRect(rect, photo.GetEdgeConfig().edge);
        }
    }
    void CancelFrame()
    {
        maskHandler.ClearMask();
        Destroy(currentFrame.gameObject);
        currentFrame = null;
    }
}