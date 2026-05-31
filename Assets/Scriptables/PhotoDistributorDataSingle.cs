using UnityEngine;

[CreateAssetMenu(fileName = "PhotoDistributorDataSingle", menuName = "Game/Photo/PhotoDistributorDataSingle")]
public class PhotoDistributorDataSingle : PhotoDistributorData
{
    [SerializeField] private Sprite photo;
    [SerializeField] private GameObject prefab;
    public override PhotoDistributor GetPhotoInstance() => new PhotoDistributorSingle(edgeConfig, photo, prefab);
}
public class PhotoDistributorSingle : PhotoDistributor
{
    private readonly Photo photo;
    public PhotoDistributorSingle(PhotoEdgeConfig edgeConfig, Sprite photo, GameObject prefab) : base(edgeConfig)
    {
        this.photo = new Photo(photo, prefab);
    }
    public override Photo GetPhoto() => photo;
}