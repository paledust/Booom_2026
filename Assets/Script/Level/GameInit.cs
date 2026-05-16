using SimpleAudioSystem;
using UnityEngine;

public class GameInit : MonoBehaviour
{
    [SerializeField] private string initAmb;
    void Start()
    {
        AudioManager.Instance.PlayAmbience(initAmb, true, .5f, 1);        
    }
}
