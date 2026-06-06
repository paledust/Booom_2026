using System.Collections.Generic;
using UnityEngine;

public class Location
{
    private Sprite nightSprite;
    private Sprite daySprite;
    private Dictionary<Direction, string> locationConnection;
    public Location(Sprite nightSprite, Sprite daySprite, Dictionary<Direction, string> locationConnection)
    {
        this.nightSprite = nightSprite;
        this.daySprite = daySprite;
        this.locationConnection = locationConnection;
    }
    public Sprite GetLocationSprite(bool isDay) => isDay ? daySprite : nightSprite;
}
