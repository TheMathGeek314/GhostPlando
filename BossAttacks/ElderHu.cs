using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;

namespace GhostPlando.BossAttacks {
    internal class ElderHu {
        internal static GameObject huPrefab;
        internal static GameObject ringHolderPrefab;
        internal static Object appearAudio;
        internal static Object impactAudio;
        private static float[] ringPositions = [0, 74.4f, 76.85f, 79.3f, 81.75f, 84.2f, 86.65f, 89.1f, 91.55f, 94, 96.45f, 98.9f, 101.35f, 103.8f, 106.25f, 108.7f, 111.15f, 113.6f];

        public static void Hook(PlayMakerFSM nosk) {
            GameObject ringHolder = GameObject.Instantiate(ringHolderPrefab, new Vector3(94, 1, 0), Quaternion.identity);
            AudioPlayerOneShotSingle[] audioObjects = huPrefab.LocateMyFSM("Attacking").GetState("M 1").GetActionsOfType<AudioPlayerOneShotSingle>();
            appearAudio = audioObjects[0].audioClip.Value;
            impactAudio = audioObjects[1].audioClip.Value;

            PlayMakerFSM fsm = nosk.gameObject.AddComponent<PlayMakerFSM>();
            fsm.FsmName = "Ring Control";
            fsm.Fsm.Events = [new FsmEvent("FINISHED"), new FsmEvent("RUNLEFT"), new FsmEvent("RUNRIGHT")];

            foreach(PlayMakerFSM childFsm in ringHolder.GetComponentsInChildren<PlayMakerFSM>(true)) {
                childFsm.GetState("Reset").GetFirstActionOfType<SetPosition>().y.Value = 14;
                FloatCompare[] compares = childFsm.GetState("Check Pos").GetActionsOfType<FloatCompare>();
                compares[0].float2.Value = 70;
                compares[1].float2.Value = 119;
                childFsm.GetState("Init").AddFirstAction(new AudioPlayerOneShotSingle {
                    audioPlayer = Modules.NoskBossModule.audioPlayer,
                    spawnPoint = childFsm.gameObject,
                    audioClip = appearAudio,
                    pitchMin = 1,
                    pitchMax = 1,
                    volume = 1,
                    delay = 0,
                    storePlayer = nosk.FsmVariables.GetFsmGameObject("Empty Store")
                });
                FsmState landState = childFsm.GetState("Land");
                landState.AddFirstAction(new AudioPlayerOneShotSingle {
                    audioPlayer = Modules.NoskBossModule.audioPlayer,
                    spawnPoint = childFsm.gameObject,
                    audioClip = impactAudio,
                    pitchMin = 1,
                    pitchMax = 1,
                    volume = 1,
                    delay = 0,
                    storePlayer = nosk.FsmVariables.GetFsmGameObject("Empty Store")
                });
                landState.GetFirstActionOfType<SetPosition>().y.Value = 6.72f;
                fsm.AddFsmGameObject($"Ring {childFsm.gameObject.name}", childFsm.gameObject);
            }
            ringHolder.SetActive(true);

            FsmState firstState = fsm.GetState(fsm.ActiveStateName);
            for(int i = 0; i < ringPositions.Length; i++) {
                FsmState leftState = fsm.AddState($"Left {i}");
                FsmState rightState = fsm.AddState($"Right {i}");
                if(i == 0) {
                    firstState.AddTransition("RUNLEFT", leftState);
                    firstState.AddTransition("RUNRIGHT", rightState);
                }
                else {
                    fsm.GetState($"Left {i - 1}").AddTransition("FINISHED", leftState);
                    fsm.GetState($"Right {i - 1}").AddTransition("FINISHED", rightState);
                }
                if(i < 17) {
                    leftState.AddLastAction(new FloatCompare {
                        float1 = nosk.FsmVariables.GetFsmFloat("Self X"),
                        float2 = ringPositions[17 - i],
                        tolerance = 0,
                        equal = FsmEvent.GetFsmEvent("FINISHED"),
                        lessThan = FsmEvent.GetFsmEvent("FINISHED"),
                        everyFrame = false
                    });
                    rightState.AddLastAction(new FloatCompare {
                        float1 = nosk.FsmVariables.GetFsmFloat("Self X"),
                        float2 = ringPositions[i],
                        tolerance = 0,
                        equal = FsmEvent.GetFsmEvent("FINISHED"),
                        greaterThan = FsmEvent.GetFsmEvent("FINISHED"),
                        everyFrame = false
                    });
                }
                if(i > 0) {
                    string ringNumSubtract = $"Ring {18 - i}";
                    string ringNumRaw = $"Ring {i}";
                    leftState.AddLastAction(new Lambda(() => fsm.FsmVariables.GetFsmGameObject(ringNumSubtract).Value.SetActive(true)));
                    rightState.AddLastAction(new Lambda(() => fsm.FsmVariables.GetFsmGameObject(ringNumRaw).Value.SetActive(true)));
                }
                if(i < 17) {
                    leftState.AddLastAction(new FloatCompare {
                        float1 = nosk.FsmVariables.GetFsmFloat("Self X"),
                        float2 = ringPositions[17 - i],
                        tolerance = 0,
                        equal = FsmEvent.GetFsmEvent("FINISHED"),
                        lessThan = FsmEvent.GetFsmEvent("FINISHED"),
                        everyFrame = true
                    });
                    rightState.AddLastAction(new FloatCompare {
                        float1 = nosk.FsmVariables.GetFsmFloat("Self X"),
                        float2 = ringPositions[i],
                        tolerance = 0,
                        equal = FsmEvent.GetFsmEvent("FINISHED"),
                        greaterThan = FsmEvent.GetFsmEvent("FINISHED"),
                        everyFrame = true
                    });
                }
            }
            fsm.GetState("Left 17").AddTransition("FINISHED", fsm.ActiveStateName);
            fsm.GetState("Right 17").AddTransition("FINISHED", fsm.ActiveStateName);
        }

        public static FsmStateAction[] RunLeft(PlayMakerFSM self) {
            return [
                new Lambda(() => {
                    FsmVariables fv = self.FsmVariables;
                    int hp = fv.GetFsmInt("HP").Value;
                    if(!fv.GetFsmBool("Will Spit").Value && hp <= fv.GetFsmInt("Phase 3-4 HP").Value) {
                        self.gameObject.LocateMyFSM("Ring Control").SendEvent("RUNLEFT");
                    }
                })
            ];
        }

        public static FsmStateAction[] RunRight(PlayMakerFSM self) {
            return [
                new Lambda(() => {
                    FsmVariables fv = self.FsmVariables;
                    int hp = fv.GetFsmInt("HP").Value;
                    if(!fv.GetFsmBool("Will Spit").Value && hp <= fv.GetFsmInt("Phase 3-4 HP").Value) {
                        self.gameObject.LocateMyFSM("Ring Control").SendEvent("RUNRIGHT");
                    }
                })
            ];
        }
    }
}