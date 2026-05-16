using UnityEngine;

[CreateAssetMenu(fileName = "SO_Photo", menuName = "Scriptable Objects/SO_Photo")]
public class SO_Photo : ScriptableObject
{
    [System.Flags]
    public enum PhotoTag
    {
        
    }
    [SerializeField] private Sprite photo_sprite;
    [SerializeField] private PhotoTag tag;
}