using UnityEngine;
using UnityEngine.Rendering;

public class PhotoFrame : MonoBehaviour
{
    [SerializeField] private SpriteRenderer frame;
    [SerializeField] private SpriteRenderer selectingFrame;
    [SerializeField] private SpriteRenderer photo;
    [SerializeField] private SortingGroup sortingGroup;

    public void Init(Vector2 worldPos, int layerIndex)
    {
        transform.position = worldPos;
        sortingGroup.sortingOrder = layerIndex;
        sortingGroup.sortingLayerID = SortingLayer.NameToID("Frame");
    }
    public void FixPhoto()
    {
        selectingFrame.gameObject.SetActive(false);
        photo.gameObject.SetActive(true);
        sortingGroup.sortingLayerID = SortingLayer.NameToID("Default");
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
