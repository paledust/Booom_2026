using SimpleAudioSystem;
using UnityEngine;

public class GameInit : MonoBehaviour
{
    [SerializeField] private string initAmb;
    [SerializeField] private string startLoc;
    [SerializeField] private LocationController locationController;
    void Start()
    {
        AudioManager.Instance.PlayAmbience(initAmb, true, .5f, 1);     
        locationController.Init(startLoc);
    }
}
