using System.Collections.Generic;
using UnityEngine;

public class LocationController : MonoBehaviour
{
    [SerializeField] private LocationDataCollection locationCollection;
    private Dictionary<string, Location> dictLocation;
    private Location currentLocation;

    void Awake()
    {
        dictLocation = new Dictionary<string, Location>();
        foreach(var location in locationCollection.allLocations)
        {
            dictLocation.Add(location.name, location.GetLocation());
        }
    }
    public void Init(string start)
    {
        dictLocation.TryGetValue(start, out currentLocation);
    }
    public bool TryGetLocation(string key, out Location location) => dictLocation.TryGetValue(key, out location);
    public Location GetLocation() => currentLocation;
}
