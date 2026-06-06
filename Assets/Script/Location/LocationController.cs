using System.Collections.Generic;
using UnityEngine;

public class LocationController : MonoBehaviour
{
    [SerializeField] private LocationDataCollection locationCollection;
    private Dictionary<string, Location> dictLocation;
    private Location currentLocation;
    private Vector2 startPos;
    private bool hasStarted = false;

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
    public bool EvaluateLocation(Vector2 nextPos)
    {
        if(!hasStarted)
        {
            hasStarted = true;
            startPos = nextPos;
            return true;
        }
        else
        {
            Vector2 dir = nextPos - startPos;
            if(Mathf.Abs(dir.y) > Mathf.Abs(dir.x))
            {
                if(currentLocation.TryGetNextLocation(dir.y>0 ? Direction.Forward : Direction.Backward, out var nextLocation))
                {
                    currentLocation = dictLocation[nextLocation];
                    startPos = nextPos;
                    return true;
                }
            }
            else
            {
                if(currentLocation.TryGetNextLocation(dir.x>0 ? Direction.Right : Direction.Left, out var nextLocation))
                {
                    currentLocation = dictLocation[nextLocation];
                    startPos = nextPos;
                    return true;
                }
            }
            return false;
        }
    }
    public Location GetLocation() => currentLocation;
}
