using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;

namespace GhostPlando.BossAttacks {
    internal class MantisLords {
        internal static GameObject lordPrefab;
        internal static GameObject discObject;
        internal static Object discAudio;

        public static void Hook(PlayMakerFSM nosk) {
            FsmState prefabState = lordPrefab.LocateMyFSM("Mantis Lord").GetState("Throw 2");
            discObject = prefabState.GetFirstActionOfType<SpawnObjectFromGlobalPool>().gameObject.Value;
            discAudio = prefabState.GetFirstActionOfType<AudioPlaySimple>().oneShotClip.Value;
            nosk.AddFsmGameObject("Mantis Projectile", null);
            discObject.Spawn().Recycle();
        }

        public static FsmStateAction[] Run(PlayMakerFSM self) {
            return [
                new AudioPlayerOneShotSingle {
                    audioPlayer = Modules.NoskBossModule.audioPlayer,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Self"),
                    audioClip = discAudio,
                    pitchMin = 1,
                    pitchMax = 1,
                    volume = 1,
                    delay = 0,
                    storePlayer = self.FsmVariables.GetFsmGameObject("Empty Store")
                },
                new SpawnObjectFromGlobalPool {
                    gameObject = discObject,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Self"),
                    position = new Vector3(0, 0, 0),
                    rotation = Vector3.zero,
                    storeObject = self.FsmVariables.GetFsmGameObject("Mantis Projectile")
                },
                new Lambda(() => {
                    foreach(KeepWorldPosition kwp in self.FsmVariables.GetFsmGameObject("Mantis Projectile").Value.GetComponentsInChildren<KeepWorldPosition>())
                        kwp.yPosition = 2.89f;
                }),
                new SendEventByName {
                    eventTarget = self.FsmGOToEvent("Mantis Projectile"),
                    sendEvent = "WIDE L",
                    delay = 0f,
                    everyFrame = false
                },
                new SpawnObjectFromGlobalPool {
                    gameObject = discObject,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Self"),
                    position = new Vector3(0, 0, 0),
                    rotation = Vector3.zero,
                    storeObject = self.FsmVariables.GetFsmGameObject("Mantis Projectile")
                },
                new Lambda(() => {
                    foreach(KeepWorldPosition kwp in self.FsmVariables.GetFsmGameObject("Mantis Projectile").Value.GetComponentsInChildren<KeepWorldPosition>())
                        kwp.yPosition = 2.89f;
                }),
                new SendEventByName {
                    eventTarget = self.FsmGOToEvent("Mantis Projectile"),
                    sendEvent = "WIDE R",
                    delay = 0f,
                    everyFrame = false
                }
            ];
        }
    }
}
