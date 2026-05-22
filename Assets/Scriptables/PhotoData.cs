using System;
using UnityEngine;

[Flags]
public enum PhotoTag{}
[Serializable]
public class PhotoEdgeConfig
{
    public EdgeType edge;
    public PhotoDataSingle[] photoDatas;
}
public abstract class PhotoData : ScriptableObject
{
    [SerializeField] protected PhotoTag tag;
    [SerializeField] protected PhotoEdgeConfig edgeConfig;

    public abstract GameObject GetPhoto();
    public PhotoEdgeConfig GetEdgeConfig() => edgeConfig;
}
