using SimpleAudioSystem;
using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class SFXPlayer
{
    [Header("Audio Config")]
    [SerializeField] private string sfxKey;
    [SerializeField, Range(0, 1)] private float sfxVolume;
    [SerializeField] private bool AutoPlay;
    [SerializeField] private bool playOnStart;
    [SerializeField, ShowIf("AutoPlay")] private float playIntersection;
    private float timer;

    public void BeginSFX()
    {
        timer = 0;
        if(playOnStart)
            PlayOnce();
    }
    public void ManualPlay() => PlayOnce();
    public void Update()
    {
        if(!AutoPlay) return;
        
        timer += Time.deltaTime;
        if(timer > playIntersection)
        {
            PlayOnce();
            timer = 0;
        }
    }
    void PlayOnce()
    {
        AudioManager.Instance.PlaySFX(sfxKey, sfxVolume);
    }
}
