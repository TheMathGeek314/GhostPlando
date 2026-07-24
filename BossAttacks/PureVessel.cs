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
        internal static Object tendrilAnticAudio;
        internal static Object tendrilBurstAudio;
        internal static Object tendrilStartAudio;

        public static void Hook(PlayMakerFSM nosk) {
            GameObject pvTemp = GameObject.Instantiate(pvPrefab);
            PlayMakerFSM pvFsm = pvPrefab.LocateMyFSM("Control");
            swordObject = pvFsm.GetState("Plume Gen").GetFirstActionOfType<SpawnObjectFromGlobalPool>().gameObject.Value;
            swordAppearAudio = pvFsm.GetState("Plume Up").GetFirstActionOfType<AudioPlayerOneShotSingle>().audioClip.Value;
            tendrilAnticAudio = pvFsm.GetState("Tendril Antic").GetFirstActionOfType<AudioPlayerOneShotSingle>().audioClip.Value;
            tendrilBurstAudio = pvFsm.GetState("Tendril Burst").GetFirstActionOfType<AudioPlayerOneShotSingle>().audioClip.Value;
            tendrilStartAudio = pvFsm.GetState("Tendril Start").GetFirstActionOfType<AudioPlayerOneShotSingle>().audioClip.Value;
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

            GameObject tendrils = GameObject.Instantiate(pvTemp.FindChild("Tendrils"), nosk.transform.position + new Vector3(-2f, 1.6f, 0), Quaternion.identity, nosk.transform);
            tendrils.transform.SetScaleX(tendrils.transform.GetScaleX() * -1);
            foreach(string varName in new string[] { "T1", "T2", "T3", "ST1", "ST2", "T Hit" }) {
                nosk.AddFsmGameObject($"Tendril {varName}", tendrils.FindChild(varName));
            }
            tendrils.SetActive(true);
            nosk.AddFsmGameObject("Tendril Parent", tendrils);
            GameObject bss = GameObject.Instantiate(pvTemp.FindChild("Burst SmallShot"), nosk.transform.position + new Vector3(-2.05f, 0.96f, 0.001f), Quaternion.identity, nosk.transform);
            bss.transform.SetScaleX(bss.transform.GetScaleX() * -1);
            nosk.AddFsmGameObject("Tendril Burst SmallShot", bss);

            GameObject.Destroy(pvTemp);
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
                    finishEvent = FsmEvent.Finished,
                    realTime = false
                }
            ];
        }

        public static FsmStateAction[] RunTendrilAntic(PlayMakerFSM self) {
            return [
                new AudioPlayerOneShotSingle {
                    audioPlayer = Modules.NoskBossModule.audioPlayer,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Self"),
                    audioClip = tendrilAnticAudio,
                    pitchMin = 1,
                    pitchMax = 1,
                    volume = 1,
                    delay = 0,
                    storePlayer = self.FsmVariables.GetFsmGameObject("Empty Store")
                }
            ];
        }

        public static FsmStateAction[] RunTendrilBurst(PlayMakerFSM self) {
            return [
                new ActivateGameObject {
                    gameObject = self.FsmGOToOwner("Tendril Burst SmallShot"),
                    activate = true,
                    recursive = false,
                    resetOnExit = false,
                    everyFrame = false
                },
                new ActivateGameObject {
                    gameObject = self.FsmGOToOwner("Tendril ST1"),
                    activate = true,
                    recursive = false,
                    resetOnExit = false,
                    everyFrame = false
                },
                new ActivateGameObject {
                    gameObject = self.FsmGOToOwner("Tendril ST2"),
                    activate = true,
                    recursive = false,
                    resetOnExit = false,
                    everyFrame = false
                },
                new AudioPlayerOneShotSingle {
                    audioPlayer = Modules.NoskBossModule.audioPlayer,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Self"),
                    audioClip = tendrilBurstAudio,
                    pitchMin = 1,
                    pitchMax = 1,
                    volume = 1,
                    delay = 0,
                    storePlayer = self.FsmVariables.GetFsmGameObject("Empty Store")
                },
                new Wait {
                    time = 0.75f,
                    finishEvent = FsmEvent.Finished,
                    realTime = false
                }
            ];
        }

        public static FsmStateAction[] RunTendrilStart(PlayMakerFSM self) {
            return [
                new AudioPlayerOneShotSingle {
                    audioPlayer = Modules.NoskBossModule.audioPlayer,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Self"),
                    audioClip = tendrilStartAudio,
                    pitchMin = 1,
                    pitchMax = 1,
                    volume = 1,
                    delay = 0,
                    storePlayer = self.FsmVariables.GetFsmGameObject("Empty Store")
                },
                new ActivateGameObject {
                    gameObject = self.FsmGOToOwner("Tendril T1"),
                    activate = true,
                    recursive = false,
                    resetOnExit = false,
                    everyFrame = false
                },
                new Wait {
                    time = 0.1f,
                    finishEvent = FsmEvent.Finished,
                    realTime = false
                }
            ];
        }

        public static FsmStateAction[] RunTendrilHit(PlayMakerFSM self) {
            return [
                new ActivateGameObject {
                    gameObject = self.FsmGOToOwner("Tendril T Hit"),
                    activate = true,
                    recursive = false,
                    resetOnExit = false,
                    everyFrame = false
                },
                new Wait {
                    time = 0.05f,
                    finishEvent = FsmEvent.Finished,
                    realTime = false
                }
            ];
        }

        public static FsmStateAction[] RunTendrilT2(PlayMakerFSM self) {
            return [
                new ActivateGameObject {
                    gameObject = self.FsmGOToOwner("Tendril T2"),
                    activate = true,
                    recursive = false,
                    resetOnExit = false,
                    everyFrame = false
                },
                new Wait {
                    time = 0.15f,
                    finishEvent = FsmEvent.Finished,
                    realTime = false
                }
            ];
        }

        public static FsmStateAction[] RunTendrilT3(PlayMakerFSM self) {
            return [
                new ActivateGameObject {
                    gameObject = self.FsmGOToOwner("Tendril T3"),
                    activate = true,
                    recursive = false,
                    resetOnExit = false,
                    everyFrame = false
                },
                new Wait {
                    time = 0.2f,
                    finishEvent = FsmEvent.Finished,
                    realTime = false
                }
            ];
        }

        public static FsmStateAction[] RunTendrilHitEnd(PlayMakerFSM self) {
            return [
                new ActivateGameObject {
                    gameObject = self.FsmGOToOwner("Tendril T Hit"),
                    activate = false,
                    recursive = false,
                    resetOnExit = false,
                    everyFrame = false
                },
                new Wait {
                    time = 0.25f,
                    finishEvent = FsmEvent.Finished,
                    realTime = false
                }
            ];
        }

        public static FsmStateAction[] RunTendrilRecover(PlayMakerFSM self) {
            return [
                new ActivateGameObject {
                    gameObject = self.FsmGOToOwner("Tendril Burst SmallShot"),
                    activate = false,
                    recursive = false,
                    resetOnExit = false,
                    everyFrame = false
                }
            ];
        }
    }
}
