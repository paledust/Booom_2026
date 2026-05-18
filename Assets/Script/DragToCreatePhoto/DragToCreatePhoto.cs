using UnityEngine;
using UnityEngine.InputSystem;

using Interaction;
using System.Collections.Generic;

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
            maxPoint = worldPos;

            if(edgeHandler.m_hasEdge)
            {
                maxPoint = edgeHandler.GetEdgeCorrectPoint(minPoint, maxPoint);
            }

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
            Vector2 min = new Vector2(Mathf.Min(rect.min.x, rect.max.x), Mathf.Min(rect.min.y, rect.max.y));
            Vector2 max = new Vector2(Mathf.Max(rect.min.x, rect.max.x), Mathf.Max(rect.min.y, rect.max.y));
            rect.min = min;
            rect.max = max;

            minPoint = rect.min;
            maxPoint = rect.max;

            if(Mathf.Abs(rect.width) < minSize.x || Mathf.Abs(rect.height) < minSize.y)
            {
                CancelFrame();
                return;
            }
            
            var interestPoint = FrameDetector.DetectSelectingFrame(rect.min, rect.max);
            if(interestPoint == null)
            {
                CancelFrame();
                return;    
            }

            layerIndex++;
            currentFrame.UpdateFrame(rect, FRAME_OFFSET);
            currentFrame.FixPhoto(interestPoint.GetNextPhoto(), layerIndex);

            TestEdge(interestPoint, rect, EdgeType.Top);
            TestEdge(interestPoint, rect, EdgeType.Bottom);
            TestEdge(interestPoint, rect, EdgeType.Left);
            TestEdge(interestPoint, rect, EdgeType.Right);

            currentFrame = null;
            maskHandler.ClearMask();
        }
    }
    void TestEdge(InterestPointBasic interestPoint, Rect rect, EdgeType edgeType)
    {
        if((interestPoint.m_edgeType & edgeType)!=0)
        {
            edgeHandler.AddEdgeToRect(rect, edgeType);
        }
    }
    void CancelFrame()
    {
        maskHandler.ClearMask();
        Destroy(currentFrame.gameObject);
        currentFrame = null;
    }
}