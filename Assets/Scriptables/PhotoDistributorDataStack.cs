using Sirenix.OdinInspector;
using UnityEngine;

[CreateAssetMenu(fileName = "PhotoDistributorDataStack", menuName = "Game/Photo/PhotoDistributorDataStack")]
public class PhotoDistributorDataStack : PhotoDistributorData
{
    [SerializeField, InfoBox("能够显示一系列的照片")]  private Sprite[] photoStack;
    [SerializeField] private GameObject[] prefabStack;
    [SerializeField] private bool isRandomOrder = false;
    public override PhotoDistributor GetPhotoInstance() => new PhotoDistributorStack(edgeConfig, photoStack, prefabStack, isRandomOrder);
}
public class PhotoDistributorStack : PhotoDistributor
{
    private readonly Photo[] photoStack;
    private int stackIndex;
    public PhotoDistributorStack(PhotoEdgeConfig edgeConfig, Sprite[] photoPics, GameObject[] photoObjs, bool isRandomOrder):base(edgeConfig)
    {
        this.photoStack = new Photo[photoStack.Length];
        stackIndex = 0;
        for(int i=0; i<photoStack.Length; i++)
        {
            var photoObj = i<photoObjs.Length?photoObjs[i]:null;
            photoStack[i] = new Photo(photoPics[i], photoObj);
        }
        if(isRandomOrder)
        {
            Service.Shuffle(ref photoStack);
        }
    }
    public override Photo GetPhoto()
    {
        var photo = photoStack[stackIndex%photoStack.Length];
        stackIndex ++;
        return photo;
    }
}
