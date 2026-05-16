using SimpleAudioSystem;
using UnityEngine;

public class LoopingNotify : MonoBehaviour
{
    [Header("Notify config")]
    [SerializeField] private bool notifyOnStart;
    [SerializeField] private float notifyIntersection;
    [SerializeField] private float notifyDuration;

    [Header("Display Config")]
    [SerializeField] private SpriteRenderer notifyRender;

    [Header("Audio Config")]
    [SerializeField] private string sfxOnNotify;
    [SerializeField, Range(0, 1)] private float sfxVolume;

    private float timer;
    private bool isNotifying;

    void Start()
    {
        timer = 0;
        if(notifyOnStart)
        {
            BeginNotify();
        }
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(isNotifying)
        {
            if(timer > notifyDuration)
            {
                timer = 0;
                isNotifying = false;
                notifyRender.enabled = false;
            }
        }
        else
        {
            if(timer > notifyIntersection)
            {
                timer = 0;
                BeginNotify();
            }
        }
    }
    void BeginNotify()
    {
        isNotifying = true;
        notifyRender.enabled = true;
        AudioManager.Instance.PlaySFX(sfxOnNotify, sfxVolume);
    }
}