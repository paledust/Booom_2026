using UnityEngine;
using UnityEngine.Rendering;

public class PhotoFrame : MonoBehaviour
{
    [SerializeField] private SpriteRenderer frame;
    [SerializeField] private SpriteRenderer selectingFrame;
    [SerializeField] private SpriteRenderer photo;
    [SerializeField] private SortingGroup sortingGroup;
    
    private const string FRAME_LAYER = "Frame";
    private const string DEFAULT_LAYER = "Default";

    public void Init(Vector2 worldPos, int layerIndex, Sprite frameSprite)
    {
        transform.position = worldPos;
        frame.sprite = frameSprite;
        sortingGroup.sortingOrder = layerIndex;
        sortingGroup.sortingLayerID = SortingLayer.NameToID(FRAME_LAYER);
        selectingFrame.gameObject.SetActive(true);
        photo.gameObject.SetActive(false);
    }
    public void FixPhoto(Sprite photoSprite)
    {
        selectingFrame.gameObject.SetActive(false);
        photo.gameObject.SetActive(true);
        photo.sprite = photoSprite;
        sortingGroup.sortingLayerID = SortingLayer.NameToID(DEFAULT_LAYER);
    }
    public void UpdateFrame(Rect rect)
    {
        transform.position = rect.center;
        Vector2 size = rect.size;
        if(size.x < 0)
        {
            size.x = -size.x;
        }
        if(size.y < 0)
        {
            size.y = -size.y;
        }
        frame.size = size + Vector2.one * 0.1f;
        photo.size = size;
        selectingFrame.size = size;
    }
}
