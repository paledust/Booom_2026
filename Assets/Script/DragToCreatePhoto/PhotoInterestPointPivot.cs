using UnityEngine;

public class PhotoInterestPointPivot : MonoBehaviour
{
    [System.Serializable]
    public struct PhotoPivot
    {
        public Vector2 localPos;
        public SpriteRenderer photo;
    }
}
