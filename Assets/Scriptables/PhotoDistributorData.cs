using System;
using UnityEngine;

[Flags]
public enum PhotoTag{}
public abstract class PhotoDistributorData : ScriptableObject
{
    [SerializeField] protected PhotoTag tag;
    [SerializeField] protected PhotoEdgeConfig edgeConfig;

    public abstract PhotoDistributor GetPhotoInstance();
}
