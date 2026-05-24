using System;
using UnityEngine;

public class PhotoAudioPlay : PhotoComponent
{
    [SerializeField] private SFXPlayer sfxLooper;
    void Start()
    {
        sfxLooper.BeginSFX();
    }

    // Update is called once per frame
    void Update()
    {
        sfxLooper.Update();
    }
}
