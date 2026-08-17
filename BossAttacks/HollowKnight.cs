using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;
using GhostPlando.Modules;

namespace GhostPlando.BossAttacks {
    internal class HollowKnight {
        internal static GameObject thkPrefab;
        internal static GameObject pillarObject;
        internal static GameObject sprayObject;
        internal static Object pillarAudio;
        internal static Object sprayAudio;

        public static void Hook(PlayMakerFSM nosk) {
            PlayMakerFSM thkfsm = thkPrefab.LocateMyFSM("Control");
            pillarObject = thkfsm.GetState("Plume Gen").GetFirstActionOfType<SpawnObjectFromGlobalPool>().gameObject.Value;
            pillarAudio = thkfsm.GetState("Plume Up").GetFirstActionOfType<AudioPlayerOneShotSingle>().audioClip.Value;
            sprayObject = thkfsm.GetState("Chest Shot").GetFirstActionOfType<FlingObjectsFromGlobalPoolVel>().gameObject.Value;
            sprayAudio = thkfsm.GetState("Burst Effect").GetFirstActionOfType<AudioPlayerOneShotSingle>().audioClip.Value;
            nosk.AddFsmGameObject("THK Pillar", null);
            nosk.AddFsmFloat("THK PillarL X", 0);
            nosk.AddFsmFloat("THK PillarR X", 0);
            nosk.AddFsmInt("THK Pillar Count", 0);
        }

        public static FsmStateAction[] SetupPillars(PlayMakerFSM self) {
            return [
                new SetFloatValue {
                    floatVariable = self.FsmVariables.GetFsmFloat("THK PillarL X"),
                    floatValue = 92.5f,
                    everyFrame = false
                },
                new SetFloatValue {
                    floatVariable = self.FsmVariables.GetFsmFloat("THK PillarR X"),
                    floatValue = 97.5f,
                    everyFrame = false
                },
                new SetIntValue {
                    intVariable = self.FsmVariables.GetFsmInt("THK Pillar Count"),
                    intValue = 10,
                    everyFrame = false
                }
            ];
        }

        public static FsmStateAction[] GenPillars(PlayMakerFSM self) {
            return [
                new SpawnObjectFromGlobalPool {
                    gameObject = pillarObject,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Self"),
                    position = Vector3.zero,
                    rotation = Vector3.zero,
                    storeObject = self.FsmVariables.GetFsmGameObject("THK Pillar")
                },
                new SetPosition {
                    gameObject = self.FsmGOToOwner("THK Pillar"),
                    vector = Vector3.zero,
                    x = self.FsmVariables.GetFsmFloat("THK PillarL X"),
                    y = 3f,
                    z = 0,
                    space = Space.World,
                    everyFrame = false,
                    lateUpdate = false
                },
                new Lambda(() => {
                    FsmState anticState = self.FsmVariables.GetFsmGameObject("THK Pillar").Value.GetComponent<PlayMakerFSM>().GetState("Antic");
                    anticState.RemoveTransitionsTo("Scale Up");
                    anticState.AddTransition("THK PLUMES UP", "Scale Up");
                }),
                new SpawnObjectFromGlobalPool {
                    gameObject = pillarObject,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Self"),
                    position = Vector3.zero,
                    rotation = Vector3.zero,
                    storeObject = self.FsmVariables.GetFsmGameObject("THK Pillar")
                },
                new SetPosition {
                    gameObject = self.FsmGOToOwner("THK Pillar"),
                    vector = Vector3.zero,
                    x = self.FsmVariables.GetFsmFloat("THK PillarR X"),
                    y = 3f,
                    z = 0,
                    space = Space.World,
                    everyFrame = false,
                    lateUpdate = false
                },
                new Lambda(() => {
                    FsmState anticState = self.FsmVariables.GetFsmGameObject("THK Pillar").Value.GetComponent<PlayMakerFSM>().GetState("Antic");
                    anticState.RemoveTransitionsTo("Scale Up");
                    anticState.AddTransition("THK PLUMES UP", "Scale Up");
                    FsmInt count = self.FsmVariables.GetFsmInt("THK Pillar Count");
                    count.Value--;
                    if(count.Value <= 0) {
                        self.SendEvent("END PILLAR GEN");
                    }
                    else {
                        self.FsmVariables.GetFsmFloat("THK PillarR X").Value += Random.Range(4.5f, 5);
                        self.FsmVariables.GetFsmFloat("THK PillarL X").Value -= Random.Range(4.5f, 5);
                    }
                })
            ];
        }

        public static FsmStateAction[] TriggerPillars(PlayMakerFSM self) {
            return [
                new AudioPlayerOneShotSingle {
                    audioPlayer = NoskBossModule.audioPlayer,
                    spawnPoint = HeroController.instance.gameObject,
                    audioClip = pillarAudio,
                    pitchMin = 1,
                    pitchMax = 1,
                    volume = 1,
                    delay = 0,
                    storePlayer = self.FsmVariables.GetFsmGameObject("Empty Store")
                },
                new SendEventByName {
                    eventTarget = new FsmEventTarget { target = FsmEventTarget.EventTarget.BroadcastAll },
                    sendEvent = "THK PLUMES UP",
                    delay = 0f,
                    everyFrame = false
                }
            ];
        }

        public static FsmStateAction[] PrepSpray(PlayMakerFSM self, NoskBossModule module) {
            return [
                new Lambda(() => {
                    module.EncounteredWingedNosk = true;
                }),
                new SetFloatValue {
                    floatVariable = self.FsmVariables.GetFsmFloat("THK Chest Shot Time"),
                    floatValue = 6,
                    everyFrame = false
                },
                new SetVelocity2d {
                    gameObject = self.FsmGOToOwner("Self"),
                    vector = Vector2.zero,
                    everyFrame = false
                },
                new AudioPlayerOneShotSingle {
                    audioPlayer = NoskBossModule.audioPlayer,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Self"),
                    audioClip = sprayAudio,
                    pitchMin = 1,
                    pitchMax = 1,
                    volume = 1,
                    delay = 0,
                    storePlayer = self.FsmVariables.GetFsmGameObject("Empty Store")
                },
                new Tk2dPlayAnimation {
                    gameObject = self.FsmGOToOwner("Self"),
                    animLibName = "",
                    clipName = "Roar"
                }
            ];
        }

        public static FsmStateAction[] RunSpray(PlayMakerFSM self) {
            return [
                new FlingObjectsFromGlobalPoolVel {
                    gameObject = sprayObject,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Self"),
                    position = new Vector3(0, 0, -0.004f),
                    spawnMin = 1,
                    spawnMax = 2,
                    speedMinX = -1,
                    speedMaxX = -20,
                    speedMinY = 17,
                    speedMaxY = 22,
                    originVariationX = 0.5f,
                    originVariationY = 0.5f
                },
                new Wait {
                    time = 0.15f,
                    finishEvent = FsmEvent.GetFsmEvent("LOOP"),
                    realTime = false
                },
                new FlingObjectsFromGlobalPoolVel {
                    gameObject = sprayObject,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Self"),
                    position = new Vector3(0, 0, -0.004f),
                    spawnMin = 1,
                    spawnMax = 2,
                    speedMinX = 1,
                    speedMaxX = 20,
                    speedMinY = 15,
                    speedMaxY = 20,
                    originVariationX = 0.5f,
                    originVariationY = 0.5f
                },
                new FlingObjectsFromGlobalPool {
                    gameObject = sprayObject,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Self"),
                    position = new Vector3(0, 0, -0.004f),
                    spawnMin = 1,
                    spawnMax = 3,
                    speedMin = 15,
                    speedMax = 20,
                    angleMin = 235,
                    angleMax = 325,
                    originVariationX = 0.5f,
                    originVariationY = 0.5f
                },
                new FloatAdd {
                    floatVariable = self.FsmVariables.GetFsmFloat("THK Chest Shot Time"),
                    add = -1,
                    everyFrame = true,
                    perSecond = true
                },
                new FloatCompare {
                    float1 = self.FsmVariables.GetFsmFloat("THK Chest Shot Time"),
                    float2 = 0,
                    tolerance = 0,
                    equal = FsmEvent.GetFsmEvent("END"),
                    lessThan = FsmEvent.GetFsmEvent("END"),
                    everyFrame = true
                }
            ];
        }
    }
}
