using UnityEngine;

[System.Serializable]
public class PhotoEdgeConfig
{
    public EdgeType edge;
    public PhotoDistributorData photoDatas;
}
//实例化Photo
public abstract class PhotoDistributor
{
    protected readonly PhotoEdgeConfig edgeConfig;
    public PhotoDistributor(PhotoEdgeConfig edgeConfig)
    {
        this.edgeConfig = edgeConfig;
    }
    public PhotoEdgeConfig GetEdgeConfig()=>edgeConfig;
    public abstract Photo GetPhoto();
}