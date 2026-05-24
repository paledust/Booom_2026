using UnityEngine;

public class InterestPointBasic : MonoBehaviour
{
    [SerializeField] private PhotoDistributorData photo;
    [SerializeField] private Collider2D hitbox;

    void Start()
    {
        Vector3 lossyScale = transform.lossyScale;
        Vector3 localScale = transform.localScale;
        localScale.y *= lossyScale.x/lossyScale.y;
        transform.localScale = localScale;
    }
    public void RevealPoint()
    {
        hitbox.enabled = false;
    }
    public void HidePoint()
    {
        hitbox.enabled = true;
    }
    public PhotoDistributorData GetNextPhoto()
    {
        Destroy(gameObject);
        return photo;
    }
}