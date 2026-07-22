using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;

namespace GhostPlando.BossAttacks {
    internal class Gorb {
        internal static GameObject slugPrefab;
        internal static GameObject spearObject;
        internal static Object projectileAudio;

        public static void Hook(PlayMakerFSM nosk) {
            FsmState attackState = slugPrefab.LocateMyFSM("Attacking").GetState("Attack");
            projectileAudio = attackState.GetFirstActionOfType<AudioPlayerOneShotSingle>().audioClip.Value;
            spearObject = attackState.GetFirstActionOfType<SpawnObjectFromGlobalPool>().gameObject.Value;
            nosk.AddFsmGameObject("Gorb Shot", null);
        }

        public static FsmStateAction[] RunSpears(PlayMakerFSM self) {
            List<FsmStateAction> actions = [
                new AudioPlayerOneShotSingle {
                    audioPlayer = Modules.NoskBossModule.audioPlayer,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Self"),
                    audioClip = projectileAudio,
                    pitchMin = 1,
                    pitchMax = 1,
                    volume = 1,
                    delay = 0,
                    storePlayer = self.FsmVariables.GetFsmGameObject("Empty Store")
                }
            ];
            float random = Random.Range(0, 360);
            for(int i = 0; i < 8; i++) {
                actions.Add(new SpawnObjectFromGlobalPool {
                    gameObject = spearObject,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Self"),
                    position = new Vector3(0, 2, 0),
                    rotation = new Vector3(0, 0, i * 45f + random),
                    storeObject = self.FsmVariables.GetFsmGameObject("Gorb Shot")
                });
            }
            return actions.ToArray();
        }
    }
}
