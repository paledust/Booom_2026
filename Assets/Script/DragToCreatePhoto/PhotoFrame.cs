using UnityEngine;
using UnityEngine.Rendering;

public class PhotoFrame : MonoBehaviour
{
    [SerializeField] private SpriteRenderer frame;
    [SerializeField] private SpriteRenderer selectingFrame;
    [SerializeField] private SortingGroup sortingGroup;
    private GameObject photo;
    private const string FRAME_LAYER = "Frame";
    private const string DEFAULT_LAYER = "Default";

    public void Init(Vector2 worldPos, int layerIndex, Sprite frameSprite)
    {
        transform.position = worldPos;
        frame.sprite = frameSprite;
        sortingGroup.sortingOrder = layerIndex;
        sortingGroup.sortingLayerID = SortingLayer.NameToID(FRAME_LAYER);
        selectingFrame.gameObject.SetActive(true);
    }
    public void FixPhoto(GameObject photoPrefab, int layerIndex)
    {
        selectingFrame.gameObject.SetActive(false);
        photo = Instantiate(photoPrefab, transform);
        photo.transform.localScale = new Vector3(frame.size.x/5f, frame.size.y/5, 0);
        photo.transform.SetSiblingIndex(0);
        sortingGroup.sortingLayerID = SortingLayer.NameToID(DEFAULT_LAYER);
        sortingGroup.sortingOrder = layerIndex;
    }
    public void UpdateFrame(Rect rect, float frameOffset)
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
        frame.size = size + Vector2.one * frameOffset;
        selectingFrame.size = size;
    }
}
