using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;

namespace GhostPlando {
    internal static class FsmEdits {
        public static FsmStateAction[] RoarEndActions(PlayMakerFSM self, GameObject title, GameObject fader, FsmState trans1) {
            return [
                new Wait {
                    time = 2.5f,
                    finishEvent = FsmEvent.Finished,
                    realTime = false
                },
                new Lambda(() => {
                    Vector3 titlePosition = GameCameras.instance.mainCamera.transform.position + new Vector3(-0.4479f, 4.8225f, 0);
                    title.transform.position = new Vector3(titlePosition.x, titlePosition.y, -1.695f);
                    fader.transform.position = title.transform.position + new Vector3(0.5f, -2.67f, -8.93f);
                    fader.transform.localScale = new Vector3(37.6446f, 26.62126f, 1);
                    GameCameras.instance.hudCanvas.LocateMyFSM("Slide Out").SendEvent("OUT INSTANT");
                }),
                new ActivateGameObject {
                    gameObject = self.FsmGOToOwner("Nosk Title Fader"),
                    activate = true,
                    recursive = false,
                    resetOnExit = false,
                    everyFrame = false
                },
                new ActivateGameObject {
                    gameObject = self.FsmGOToOwner("Nosk Title"),
                    activate = true,
                    recursive = false,
                    resetOnExit = false,
                    everyFrame = false
                },
                new TransitionToAudioSnapshot {
                    snapshot = trans1.GetFirstActionOfType<TransitionToAudioSnapshot>().snapshot,
                    transitionTime = 0
                },
                new ApplyMusicCue {
                    musicCue = trans1.GetFirstActionOfType<ApplyMusicCue>().musicCue,
                    delayTime = 0,
                    transitionTime = 0
                }
            ];
        }

        public static FsmStateAction[] RoarInitActions(PlayMakerFSM self) {
            return [
                new SendEventByName {
                    eventTarget = new FsmEventTarget {
                        target = FsmEventTarget.EventTarget.GameObject,
                        gameObject = self.FsmGOToOwner("Nosk Title"),
                        sendToChildren = true
                    },
                    sendEvent = "DOWN",
                    delay = 0,
                    everyFrame = false
                },
                new iTweenFadeTo {
                    gameObject = self.FsmGOToOwner("Nosk Title Fader"),
                    alpha = 0,
                    includeChildren = true,
                    namedValueColor = "_Color",
                    time = 0.1f,
                    delay = 0,
                    easeType = iTween.EaseType.linear,
                    loopType = iTween.LoopType.none,
                    finishEvent = FsmEvent.Finished,
                    realTime = false,
                    stopOnExit = true,
                    loopDontFinish = false
                },
                new Lambda(() => {
                    GameCameras.instance.hudCanvas.LocateMyFSM("Slide Out").SendEvent("IN");
                })
            ];
        }

        public static FsmStateAction[] PhaseDecisionActions(PlayMakerFSM self) {
            return [
                new IntCompare {
                    integer1 = self.FsmVariables.GetFsmInt("HP"),
                    integer2 = self.FsmVariables.GetFsmInt("Phase 1-2 HP"),
                    greaterThan = FsmEvent.GetFsmEvent("ONE"),
                    everyFrame = false
                },
                new IntCompare {
                    integer1 = self.FsmVariables.GetFsmInt("HP"),
                    integer2 = self.FsmVariables.GetFsmInt("Phase 2-3 HP"),
                    greaterThan = FsmEvent.GetFsmEvent("TWO"),
                    everyFrame = false
                },
                new IntCompare {
                    integer1 = self.FsmVariables.GetFsmInt("HP"),
                    integer2 = self.FsmVariables.GetFsmInt("Phase 3-4 HP"),
                    equal = FsmEvent.GetFsmEvent("FOUR"),
                    lessThan = FsmEvent.GetFsmEvent("FOUR"),
                    greaterThan = FsmEvent.GetFsmEvent("THREE"),
                    everyFrame = false
                }
            ];
        }

        public static FsmStateAction[] HoveringActions(PlayMakerFSM self) {
            List<FsmStateAction> actions = [
                new IntCompare {
                    integer1 = self.FsmVariables.GetFsmInt("HP"),
                    integer2 = self.FsmVariables.GetFsmInt("Phase 3-4 HP"),
                    greaterThan = FsmEvent.Finished,
                    everyFrame = false
                },
                new SetGravity2dScale {
                    gameObject = self.FsmGOToOwner("Self"),
                    gravityScale = 0
                },
                new SetVelocity2d {
                    gameObject = self.FsmGOToOwner("Self"),
                    vector = Vector2.zero,
                    x = 0,
                    y = 0,
                    everyFrame = false
                }
            ];
            actions.AddRange(BossAttacks.HornetP.RunThreadstorm(self));
            actions.AddRange(BossAttacks.Grimm.RunFlames(self));
            actions.Add(new Wait {
                time = 1f,
                finishEvent = FsmEvent.Finished,
                realTime = false
            });
            return actions.ToArray();
        }

        public static FsmStateAction[] RevertCeilingFall(PlayMakerFSM self) {
            return [
                new SetBoolValue {
                    boolVariable = self.FsmVariables.GetFsmBool("Falling from Ceiling"),
                    boolValue = false,
                    everyFrame = false
                }
            ];
        }

        public static FsmStateAction[] AfterFallActions(PlayMakerFSM self) {
            return [
                new Wait {
                    time = self.FsmVariables.GetFsmFloat("Wait After Falling"),
                    finishEvent = FsmEvent.Finished,
                    realTime = false
                }
            ];
        }

        public static FsmStateAction[] ResetFallWait(PlayMakerFSM self) {
            return [
                new SetFloatValue {
                    floatVariable = self.FsmVariables.GetFsmFloat("Wait After Falling"),
                    floatValue = 0,
                    everyFrame = false
                }
            ];
        }

        public static FsmStateAction[] ReduceJumpForce(PlayMakerFSM self) {
            return [
                new Lambda(() => {
                    FsmVariables fv = self.FsmVariables;
                    if(fv.GetFsmInt("HP").Value <= fv.GetFsmInt("Phase 3-4 HP").Value)
                        fv.GetFsmFloat("Jump Force").Value = 41;
                })
            ];
        }
    }
}
