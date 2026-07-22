using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;

namespace GhostPlando.BossAttacks {
    internal class PureVessel {
        internal static GameObject pvPrefab;
        internal static GameObject focusPrefab;
        internal static GameObject swordObject;
        internal static Object swordAppearAudio;

        public static void Hook(PlayMakerFSM nosk) {
            PlayMakerFSM pvFsm = pvPrefab.LocateMyFSM("Control");
            swordObject = pvFsm.GetState("Plume Gen").GetFirstActionOfType<SpawnObjectFromGlobalPool>().gameObject.Value;
            swordAppearAudio = pvFsm.GetState("Plume Up").GetFirstActionOfType<AudioPlayerOneShotSingle>().audioClip.Value;
            nosk.AddFsmGameObject("PV Sword", null);
            GameObject pvFocusParent = new();
            for(int i = 70; i < 120; i += 7) {
                GameObject blast = GameObject.Instantiate(focusPrefab, new Vector3(i, 12.1483f, -0.9688f), Quaternion.identity, pvFocusParent.transform);
                PlayMakerFSM blastControl = blast.GetComponent<PlayMakerFSM>();
                RandomFloat lowState = blastControl.GetState("Pos Low").GetFirstActionOfType<RandomFloat>();
                RandomFloat highState = blastControl.GetState("Pos High").GetFirstActionOfType<RandomFloat>();
                lowState.min = 5.88f;
                lowState.max = 8.08f;
                highState.min = 9.88f;
                highState.max = 12.08f;
                blast.SetActive(true);
            }
            nosk.AddFsmGameObject("PV Focus", pvFocusParent);
        }

        public static FsmStateAction[] RunSpikes(PlayMakerFSM self) {
            List<FsmStateAction> actions = new();
            for(float x = 72; x < 117; x += 3.7f) {
                Vector3 vec = new(x, 2.2f, -0.05f);
                actions.Add(new SpawnObjectFromGlobalPool {
                    gameObject = swordObject,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Zero Origin"),
                    position = vec,
                    rotation = Vector3.zero,
                    storeObject = self.FsmVariables.GetFsmGameObject("PV Sword")
                });
            }
            actions.Add(new AudioPlayerOneShotSingle {
                audioPlayer = Modules.NoskBossModule.audioPlayer,
                spawnPoint = HeroController.instance.gameObject,
                audioClip = swordAppearAudio,
                pitchMin = 1,
                pitchMax = 1,
                volume = 1,
                delay = 1,
                storePlayer = self.FsmVariables.GetFsmGameObject("Empty Store")
            });
            return actions.ToArray();
        }

        public static FsmStateAction[] RunFocus(PlayMakerFSM self) {
            return [
                new SendEventByName {
                    eventTarget = new FsmEventTarget {
                        gameObject = self.FsmGOToOwner("PV Focus"),
                        target = FsmEventTarget.EventTarget.GameObject,
                        sendToChildren = true
                    },
                    sendEvent = "BLAST",
                    delay = 0,
                    everyFrame = false
                },
                new Wait {
                    time = 1,
                    finishEvent = FsmEvent.GetFsmEvent("FINISHED"),
                    realTime = false
                }
            ];
        }
    }
}
