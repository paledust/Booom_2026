using UnityEngine;

public class InterestPointBasic : MonoBehaviour
{
    [SerializeField] private PhotoDistributorData photo;
    [SerializeField] private Collider2D hitbox;

    public void RevealPoint()
    {
        hitbox.enabled = false;
    }
    public void HidePoint()
    {
        hitbox.enabled = true;
    }
    public PhotoDistributor GetNextPhoto()
    {
        Destroy(gameObject);
        return photo.GetPhotoInstance();
    }
}