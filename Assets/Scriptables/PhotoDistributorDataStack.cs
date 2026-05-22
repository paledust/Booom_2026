using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "PhotoDistributorDataStack", menuName = "Game/Photo/PhotoDistributorDataStack")]
public class PhotoDistributorDataStack : PhotoDistributorData
{
    [SerializeField, InfoBox("能够显示一系列的照片")]  private GameObject[] photoStack;
    [SerializeField] private bool isRandomOrder = false;
    public override PhotoDistributor GetPhotoInstance() => new PhotoDistributorStack(edgeConfig, photoStack, isRandomOrder);
}
public class PhotoDistributorStack : PhotoDistributor
{
    private readonly GameObject[] photoStack;
    private int stackIndex;
    public PhotoDistributorStack(PhotoEdgeConfig edgeConfig, GameObject[] stacks, bool isRandomOrder):base(edgeConfig)
    {
        photoStack = stacks;
        stackIndex = 0;
        if(isRandomOrder)
        {
            Service.Shuffle(ref photoStack);
        }
    }

    public override GameObject GetPhotoPrefab()
    {
        var photo = photoStack[stackIndex%photoStack.Length];
        stackIndex ++;
        return photo;
    }
}
