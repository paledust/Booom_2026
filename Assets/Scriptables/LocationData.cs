#if UNITY_EDITOR
using System.Collections.Generic;
using Sirenix.OdinInspector;
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
    [SerializeField, OnValueChanged("ValidateLocationForward")] private LocationData forwardLocation;
    [SerializeField, OnValueChanged("ValidateLocationBackward")] private LocationData backwardLocation;
    [SerializeField, OnValueChanged("ValidateLocationRight")] private LocationData rightLocation;
    [SerializeField, OnValueChanged("ValidateLocationLeft")] private LocationData leftLocation;

    void OnReset()
    {
        forwardLocation = null;
        backwardLocation = null;
        rightLocation = null;
        leftLocation = null;
    }
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
    [Button("Reset")]
    public void ResetLocation()
    {
        forwardLocation = null;
        backwardLocation = null;
        rightLocation = null;
        leftLocation = null;
        EditorUtility.SetDirty(this);
    }
    void ValidateLocationForward()=>ValidateLocation(Direction.Forward);
    void ValidateLocationBackward()=>ValidateLocation(Direction.Backward);
    void ValidateLocationRight()=>ValidateLocation(Direction.Right);
    void ValidateLocationLeft()=>ValidateLocation(Direction.Left);
    void ValidateLocation(Direction dir)
    {
        LocationData data = null;
        switch(dir)
        {
            case Direction.Forward:
                data = forwardLocation;
                if(forwardLocation!=null)
                    forwardLocation.backwardLocation = this;
                break;
            case Direction.Backward:
                data = backwardLocation;
                if(backwardLocation!=null)
                    backwardLocation.forwardLocation = this;
                break;
            case Direction.Right:
                data = rightLocation;
                if(rightLocation!=null)
                    rightLocation.leftLocation = this;
                break;
            case Direction.Left:
                data = leftLocation;
                if(leftLocation!=null)
                    leftLocation.rightLocation = this;
                break;
        }
        if(data!=null)
        {
            EditorUtility.SetDirty(data);
        }
    }
#endif
}
