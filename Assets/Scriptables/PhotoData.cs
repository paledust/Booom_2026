using UnityEngine;
using System;

[CreateAssetMenu(fileName = "PhotoData", menuName = "Scriptable Objects/PhotoData")]
public class PhotoData : ScriptableObject
{
    [Flags]
    public enum PhotoTag
    {
        
    }
    [Serializable]
    public class PhotoEdgeConfig
    {
        public EdgeType edge;
        public PhotoData[] photoDatas;
    }
    [SerializeField] private PhotoTag tag;
    [SerializeField] private GameObject photoPrefab;
    [SerializeField] private PhotoEdgeConfig edgeConfig;

    public GameObject GetPhoto() => photoPrefab;
    public PhotoEdgeConfig GetEdgeConfig() => edgeConfig;
}