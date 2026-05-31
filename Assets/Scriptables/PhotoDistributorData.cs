using System;
using UnityEngine;

[Flags]
public enum PhotoTag{}
[Serializable]
public class Photo
{
    public readonly Sprite photoPic;
    public readonly GameObject photoObj;
    public Photo(Sprite pic, GameObject prefab)
    {
        this.photoObj = prefab;
        this.photoPic = pic;
    }
}

public abstract class PhotoDistributorData : ScriptableObject
{
    [SerializeField] protected PhotoTag tag;
    [SerializeField] protected PhotoEdgeConfig edgeConfig;

    public abstract PhotoDistributor GetPhotoInstance();
}
