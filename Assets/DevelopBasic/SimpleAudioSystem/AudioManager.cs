using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace SimpleAudioSystem{
    public class AudioManager : Singleton<AudioManager>
    {
        private enum AudioType{BGM, AMB, SFX}
        [SerializeField] private AudioInfo_SO audioInfo;
    [Header("Audio source")]
        [SerializeField] private AudioSource ambience_loop;
        [SerializeField] private AudioSource music_loop;
    [Header("Audio mixer")]
        [SerializeField] private AudioMixer mainMixer;
        [SerializeField] private AudioMixerSnapshot[] mixerSnapShots;
        private bool ambience_crossfading = false;
        private bool music_crossfading = false;

        public string current_music_name{get; private set;} = string.Empty;
        public string current_ambience_name{get; private set;} = string.Empty;

        private const string masterVolumeName = "MasterVolume";
        private CoroutineExcuter ambFader;
        private CoroutineExcuter musicFader;

        protected override void Awake()
        {
            base.Awake();
        }
        #region Sound Play
        public void PlayMusic(string audio_name, float volume = 1){
            current_music_name = audio_name;
            if(audio_name == string.Empty) music_loop.Stop();

            music_loop.clip = audioInfo.GetBGMClipByName(audio_name);
            if(music_loop.clip!=null){
                music_loop.volume = volume;
                music_loop.Play();
            }
        }
        public void PlayMusic(string audio_name, bool startOver, float transitionTime, float volume = 1, bool forcePlay = false){
        //If no audio name, fade out the ambience
            if(audio_name == string.Empty){
                FadeAudio(music_loop, 0, transitionTime, true);
                current_music_name = string.Empty;
            }
        //If the audio name is the same, only fade the volume to the target value
            if(current_music_name==audio_name){
                FadeAudio(music_loop, volume, transitionTime);
            }
            else{
                if(current_music_name == string.Empty || !music_loop.isPlaying){
                    music_loop.clip = audioInfo.GetAMBClipByName(audio_name);
                    if(music_loop.clip==null){
                        Debug.LogWarning("No clip found, nothing will be done for ambient");
                        return;
                    }
                    music_loop.volume = volume;
                    music_loop.Play();
                }
                else{
                    if(music_loop.clip==null){
                        Debug.LogWarning("No clip found, nothing will be done for ambient");
                        return;
                    }
                    CorssFadeMusic(audio_name, volume, startOver, transitionTime, forcePlay);
                }
                current_music_name = audio_name;
            }   
        }
        public void PlayAmbience(string audio_name, bool startOver, float transitionTime, float volume = 1, bool forcePlay = false){
        //If no audio name, fade out the ambience
            if(audio_name == string.Empty){
                FadeAudio(ambience_loop, 0, transitionTime, true);
                current_ambience_name = string.Empty;
            }
        //If the audio name is the same, only fade the volume to the target value
            if(current_ambience_name==audio_name){
                FadeAudio(ambience_loop, volume, transitionTime);
            }
            else{
                if(current_ambience_name == string.Empty || !ambience_loop.isPlaying){
                    ambience_loop.clip = audioInfo.GetAMBClipByName(audio_name);
                    if(ambience_loop.clip==null){
                        Debug.LogWarning("No clip found, nothing will be done for ambient");
                        return;
                    }
                    ambience_loop.volume = volume;
                    ambience_loop.Play();
                }
                else{
                    if(ambience_loop.clip==null){
                        Debug.LogWarning("No clip found, nothing will be done for ambient");
                        return;
                    }
                    CrossFadeAmbience(audio_name, volume, startOver, transitionTime, forcePlay);
                }
                current_ambience_name = audio_name;
            }            
        }
        public void PlayAmbience(string audio_name, bool startOver, float volume=1, bool forcePlay = false){
        //If no audio name, fade out the ambience
            if(audio_name == string.Empty){
                FadeAudio(ambience_loop, 0, 0.5f, true);
                current_ambience_name = string.Empty;
            }
        //If the audio name is the same, only fade the volume to the target value
            if(current_ambience_name==audio_name){
                FadeAudio(ambience_loop, volume, 0.5f);
            }
            else{
                if(current_ambience_name == string.Empty || !ambience_loop.isPlaying){
                    ambience_loop.clip = audioInfo.GetAMBClipByName(audio_name);
                    if(ambience_loop.clip==null){
                        Debug.LogWarning("No clip found, nothing will be done for ambient");
                        return;
                    }
                    ambience_loop.volume = volume;
                    ambience_loop.Play();
                }
                else{
                    if(ambience_loop.clip==null){
                        Debug.LogWarning("No clip found, nothing will be done for ambient");
                        return;
                    }
                    CrossFadeAmbience(audio_name, volume, startOver, 0.5f, forcePlay);
                }
                current_ambience_name = audio_name;
            }
        }
        public AudioClip PlaySoundEffect(AudioSource targetSource, string clip_name, float volumeScale){
            AudioClip clip = audioInfo.GetSFXClipByName(clip_name);
            if(clip!=null)
                targetSource.PlayOneShot(clip, volumeScale);
            else
                Debug.LogAssertion($"No Clip found:{clip_name}");
            return clip;
        }
        public AudioClip PlaySoundEffectLoop(AudioSource targetSource, string clip_name, float volumeScale, float transition = 1f){
            AudioClip clip = audioInfo.GetSFXClipByName(clip_name);
            targetSource.clip = clip;
            targetSource.loop = true;
            targetSource.Play();
            FadeAudio(targetSource, volumeScale, transition);

            return clip;
        }
        public void PlaySoundEffect(AudioSource targetSource, string clip, float volumeScale, float delay, Action completeCallback=null)=>
            StartCoroutine(coroutineDelaySFX(targetSource, clip, volumeScale, delay, completeCallback));
        public void PlaySoundEffect_WithFinishCallback(AudioSource targetSource, string clip, float volumScale, Action finishCallback=null)=>
            StartCoroutine(coroutineSFX_WithFinishAction(targetSource, clip, volumScale, finishCallback));
        public void FadeInAndOutSoundEffect(AudioSource targetSource, string clip, float maxVolume, float duration, float fadeIn, float fadeOut)=>
            StartCoroutine(coroutineFadeInAndOutSFX(targetSource, clip, maxVolume, duration, fadeIn, fadeOut));
    #endregion

    #region Helper function
        public static void SwitchAudioListener(AudioListener from, AudioListener to){
            from.enabled = false;
            to.enabled = true;
        }
        public void FadeAmbience(float targetVolume, float transitionTime, bool StopOnFadeOut = false)=>FadeAudio(ambience_loop, targetVolume, transitionTime, StopOnFadeOut);
        public void FadeMusic(float targetVolume, float transitionTime, bool StopOnFadeOut = false)=>FadeAudio(music_loop, targetVolume, transitionTime, StopOnFadeOut);
        public void FadeAudio(AudioSource m_audio, float targetVolume, float transitionTime, bool StopOnFadeOut = false){
            StartCoroutine(coroutineFadeAudio(m_audio, targetVolume, transitionTime, StopOnFadeOut));
        }
        public void ChangeMasterVolume(float targetVolume){
            mainMixer.SetFloat(masterVolumeName, targetVolume);
        }
        void CrossFadeAmbience(string audio_name, float targetVolume, bool startOver, float transitionTime, bool forceCrossFade = false){
            if(!forceCrossFade && ambience_crossfading) return;
            if(ambFader==null) ambFader = new CoroutineExcuter(this);
            ambFader.Excute(coroutineCrossFadeAmbience(current_ambience_name, audio_name, targetVolume, startOver, transitionTime));
        }
        void CorssFadeMusic(string audio_name, float targetVolume, bool startOver, float transitionTime, bool forceCrossFade = false){
            if(!forceCrossFade && music_crossfading) return;
            if(musicFader==null) musicFader = new CoroutineExcuter(this);
            musicFader.Excute(coroutineCrossFadeMusic(current_music_name, audio_name, targetVolume, startOver, transitionTime));           
        }
    #endregion
        IEnumerator coroutineFadeInAndOutSFX(AudioSource m_audio, string clip, float maxVolume, float duration, float fadeIn, float fadeOut){
            AudioSource tempAudio = Instantiate(m_audio);
            tempAudio.name = "_TempSFX";
            tempAudio.volume = 0;
            tempAudio.loop   = true;
            tempAudio.clip   = audioInfo.GetSFXClipByName(clip);
            tempAudio.Play();

            yield return new WaitForLoop(fadeIn, (t)=>tempAudio.volume = Mathf.Lerp(0, maxVolume, t));
            yield return new WaitForSeconds(duration);
            yield return new WaitForLoop(fadeOut, (t)=>tempAudio.volume = Mathf.Lerp(maxVolume, 0, t));

            Destroy(tempAudio.gameObject);
        }
        IEnumerator coroutineDelaySFX(AudioSource m_audio, string clip, float volumeScale, float delay, Action completeCallback){
            yield return new WaitForSeconds(delay);
            PlaySoundEffect(m_audio, clip, volumeScale);
            completeCallback?.Invoke();
        }
        IEnumerator coroutineSFX_WithFinishAction(AudioSource m_audio, string clip, float volumeScale, Action finishCallback){
            var playedClip = PlaySoundEffect(m_audio, clip, volumeScale);
            yield return new WaitForSeconds(playedClip==null?0:playedClip.length);
            finishCallback?.Invoke();
        }
        IEnumerator coroutineCrossFadeAmbience(string from_clip, string to_clip, float targetVolume, bool startOver, float transitionTime){
            ambience_crossfading = true;

            yield return coroutineCrossFadeAudio(ambience_loop, from_clip, to_clip, targetVolume, startOver, transitionTime, AudioType.AMB);
            current_ambience_name = to_clip;

            ambience_crossfading = false;
        }
        IEnumerator coroutineCrossFadeMusic(string from_clip, string to_clip, float targetVolume, bool startOver, float transitionTime){
            music_crossfading = true;

            yield return coroutineCrossFadeAudio(music_loop, from_clip, to_clip, targetVolume, startOver, transitionTime, AudioType.BGM);
            current_music_name = to_clip;

            music_crossfading = false;
        }
        IEnumerator coroutineCrossFadeAudio(AudioSource targetSource, string from_clip, string to_clip, float targetVolume, bool startOver, float transitionTime, AudioType audioType){
            if(from_clip!=string.Empty){
                StartCoroutine(coroutineFadeAudio(targetSource, 0, transitionTime));
            }

            AudioSource tempAudio = new GameObject($"[_Temp_{targetSource.name}]").AddComponent<AudioSource>();
            Destroy(tempAudio.gameObject, transitionTime); //Schedule an auto Destruction;

            tempAudio.volume = 0;
            tempAudio.loop   = true;
        //Get Audio clip based on type
            AudioClip targetClip;
            switch(audioType){
                case AudioType.SFX:
                    targetClip = audioInfo.GetSFXClipByName(to_clip);
                    break;
                case AudioType.AMB:
                    targetClip = audioInfo.GetAMBClipByName(to_clip);
                    break;
                case AudioType.BGM:
                    targetClip = audioInfo.GetBGMClipByName(to_clip);
                    break;
                default:
                    targetClip = audioInfo.GetAMBClipByName(to_clip);
                    break;
            }

        //Fade In Clip
            tempAudio.clip   = targetClip;
            if(!startOver) tempAudio.time   = targetSource.time;
            tempAudio.outputAudioMixerGroup = targetSource.outputAudioMixerGroup;
            tempAudio.Play();
            yield return coroutineFadeAudio(tempAudio, targetVolume, transitionTime);

        //Swap audio
            targetSource.clip = tempAudio.clip;
            targetSource.time = tempAudio.time;
            targetSource.volume = tempAudio.volume;
            targetSource.Play();
        }
        IEnumerator coroutineFadeAudio(AudioSource source, float targetVolume, float transition, bool StopOnFadeOut = false){
            float initVolume = source.volume;
            yield return new WaitForLoop(transition, (t)=>{
                source.volume = Mathf.Lerp(initVolume, targetVolume, t);
            });
            yield return null;

            if(StopOnFadeOut && source.volume == 0) source.Stop();
        }
    }
}
