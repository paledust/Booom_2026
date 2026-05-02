using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleAudioSystem{
    [CreateAssetMenu(fileName = "AudioInfo_SO", menuName = "DevelopBasic/AudioSystem/AudioInfo_SO")]
    public class AudioInfo_SO : ScriptableObject
    {
        public List<AudioInfo> bgm_info_list;
        public List<AudioInfo> amb_info_list;
        public List<AudioInfo> sfx_info_list;
        public List<AudioGroupInfo> sfx_group_info_list;
        public AudioClip GetBGMClipByName(string audio_name){
            return bgm_info_list.Find(x=>x.audio_name == audio_name).audio_clip;
        }
        public AudioClip GetAMBClipByName(string audio_name){
            return amb_info_list.Find(x=>x.audio_name == audio_name).audio_clip;
        }
        public AudioClip GetSFXClipByName(string audio_name){
            if(audio_name.Contains("group"))
                return GetSFXClipFromGroupByName(audio_name);
            else
                return sfx_info_list.Find(x=>x.audio_name == audio_name).audio_clip;
        }
        AudioClip GetSFXClipFromGroupByName(string audio_group_name){
            var clipGroup = sfx_group_info_list.Find(x=>x.audio_group_name == audio_group_name);
            if(clipGroup!=null) return clipGroup.GetAudioClip();
            else return null;
        }
    }
    [Serializable]
    public class AudioInfo{
        public string audio_name;
        public AudioClip audio_clip;
    }
    [Serializable]
    public class AudioGroupInfo{
        public string audio_group_name;
        public AudioClip[] audioClips;
        [NonSerialized] private int audioIndex = 0;
        public AudioClip GetAudioClip(){
            var clip = audioClips[audioIndex];
            audioIndex ++;
            if(audioIndex >= audioClips.Length){
                audioIndex = 0;
                Service.Shuffle(ref audioClips);
            }

            return clip;
        }
    }
}