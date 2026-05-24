using SimpleAudioSystem;
using UnityEngine;

public class LoopingNotify : MonoBehaviour
{
    [Header("Notify config")]
    [SerializeField] private float notifyStartDelay;
    [SerializeField] private float notifyIntersection;
    [SerializeField] private float notifyDuration;

    [Header("Display Config")]
    [SerializeField] private SpriteRenderer notifyRender;

    private float timer;
    private bool isNotifying;

    void Start()
    {
        timer = -notifyStartDelay;
        if(timer > 0)
        {
            timer = 0;
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
    }
}