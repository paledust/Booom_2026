#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
#endif
using UnityEngine;

public enum Direction
{
    Forward,
    Backward,
    Right,
    Left
}

[CreateAssetMenu(fileName = "LocationData", menuName = "Game/Location/LocationData")]
public class LocationData : ScriptableObject
{
    [Header("Sprite")]
    [SerializeField] private Sprite locationSpriteNight;
    [SerializeField] private Sprite locationSpriteDay;

    [Header("Mapping")]
    [SerializeField] private LocationData forwardLocation;
    [SerializeField] private LocationData backwardLocation;
    [SerializeField] private LocationData rightLocation;
    [SerializeField] private LocationData leftLocation;

    public Location GetLocation()
    {
        Dictionary<Direction, string> adjacentLoc = new Dictionary<Direction, string>();
        if(forwardLocation != null)
            adjacentLoc.Add(Direction.Forward, forwardLocation.name);
        if(backwardLocation != null)
            adjacentLoc.Add(Direction.Backward, backwardLocation.name);
        if(rightLocation != null)
            adjacentLoc.Add(Direction.Right, rightLocation.name);
        if(leftLocation != null)
            adjacentLoc.Add(Direction.Left, leftLocation.name);
            
        return new Location(locationSpriteNight, locationSpriteDay, adjacentLoc);
    }
#if UNITY_EDITOR
    public void Validate()
    {
        if(forwardLocation!=null)
        {
            if(forwardLocation.backwardLocation != this)
            {
                if(forwardLocation.backwardLocation!=null)
                {
                    Debug.LogError($"Location not match: forward location - {forwardLocation.name} of {name} is not the others' backward location");
                }
                else
                {
                    forwardLocation.backwardLocation = this;
                    EditorUtility.SetDirty(forwardLocation);
                }
            }
        }
        if(backwardLocation!=null)
        {
            if(backwardLocation.forwardLocation != this)
            {
                if(backwardLocation.forwardLocation!=null)
                {
                    Debug.LogError($"Location not match: forward location - {forwardLocation.name} of {name} is not the others' forward location");
                }
                else
                {
                    backwardLocation.forwardLocation = this;
                    EditorUtility.SetDirty(backwardLocation);
                }
            }
        }
        if(rightLocation!=null)
        {
            if(rightLocation.leftLocation != this)
            {
                if(rightLocation.leftLocation!=null)
                {
                    Debug.LogError($"Location not match: forward location - {rightLocation.name} of {name} is not the others' leftLocation location");
                }
                else
                {
                    rightLocation.leftLocation = this;
                    EditorUtility.SetDirty(rightLocation);
                }
            }
        }
        if(leftLocation!=null)
        {
            if(leftLocation.rightLocation != this)
            {
                if(leftLocation.rightLocation!=null)
                {
                    Debug.LogError($"Location not match: forward location - {leftLocation.name} of {name} is not the others' rightLocation location");
                }
                else
                {
                    leftLocation.rightLocation = this;
                    EditorUtility.SetDirty(leftLocation);
                }
            }
        }
    }
#endif
}
