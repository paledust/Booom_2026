using UnityEngine;

public class InterestPointBasic : MonoBehaviour
{
    [SerializeField] private PhotoData photo;
    [SerializeField] private Collider2D hitbox;

    public void RevealPoint()
    {
        hitbox.enabled = false;
    }
    public void HidePoint()
    {
        hitbox.enabled = true;
    }
    public PhotoData GetNextPhoto()
    {
        Destroy(gameObject);
        return photo;
    }
}