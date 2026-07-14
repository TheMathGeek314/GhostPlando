using Modding;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using WavLib;
using MCI = MusicCue.MusicChannelInfo;

namespace GhostPlando.Modules {
    internal class MimicMusicModule: ItemChanger.Modules.Module {
        private Dictionary<string, AudioClip> _audioClipCache = new Dictionary<string, AudioClip>();
        private const string filename = "S49B-23 MIMIC SPIDER";

        public override void Initialize() {
            CacheAudioFiles();
            On.AudioManager.BeginApplyMusicCue += ApplyMusic;
        }

        public override void Unload() {
            On.AudioManager.BeginApplyMusicCue -= ApplyMusic;
        }

        private void CacheAudioFiles() {
            _audioClipCache.Clear();
            using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream($"GhostPlando.Resources.Audio.{filename}.wav");
            WavData wavData = new();
            wavData.Parse(stream);

            float[] wavSoundData = wavData.GetSamples();
            AudioClip audioClip = AudioClip.Create(filename, wavSoundData.Length / wavData.FormatChunk.NumChannels, wavData.FormatChunk.NumChannels, (int)wavData.FormatChunk.SampleRate, false);
            audioClip.SetData(wavSoundData, 0);
            GameObject.DontDestroyOnLoad(audioClip);
            _audioClipCache.Add(filename, audioClip);
        }

        private IEnumerator ApplyMusic(On.AudioManager.orig_BeginApplyMusicCue orig, AudioManager self, MusicCue musicCue, float delayTime, float transitionTime, bool applySnapshot) {
            bool changed = false;
            MCI[] infos = ReflectionHelper.GetField<MusicCue, MCI[]>(musicCue, "channelInfos");
            foreach(MCI info in infos) {
                AudioClip origAudio = ReflectionHelper.GetField<MCI, AudioClip>(info, "clip");
                if(origAudio != null) {
                    AudioClip possibleReplace = GetAudioClip(origAudio.name);
                    if(possibleReplace != null) {
                        ReflectionHelper.SetField<MCI, AudioClip>(info, "clip", possibleReplace);
                        changed = true;
                    }
                }
            }
            if(changed)
                ReflectionHelper.SetField<MusicCue, MCI[]>(musicCue, "channelInfos", infos);
            yield return orig(self, musicCue, delayTime, transitionTime, applySnapshot);
        }

        private AudioClip GetAudioClip(string origName) {
            if(_audioClipCache.TryGetValue(origName.ToUpper(), out AudioClip ac))
                return ac;
            return null;
        }
    }
}