using UnityEngine;

[CreateAssetMenu(fileName = "PhotoDistributorDataSingle", menuName = "Game/Photo/PhotoDistributorDataSingle")]
public class PhotoDistributorDataSingle : PhotoDistributorData
{
    [SerializeField] private GameObject photoPrefab;
    public override PhotoDistributor GetPhotoInstance() => new PhotoDistributorSingle(edgeConfig, photoPrefab);
}
public class PhotoDistributorSingle : PhotoDistributor
{
    private readonly GameObject photoPrefab;
    public PhotoDistributorSingle(PhotoEdgeConfig edgeConfig, GameObject prefab) : base(edgeConfig)
    {
        photoPrefab = prefab;
    }
    public override GameObject GetPhotoPrefab() => photoPrefab;
}