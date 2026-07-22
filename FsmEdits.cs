using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.FsmStateActions;

namespace GhostPlando {
    internal static class FsmEdits {
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
                    greaterThan = FsmEvent.GetFsmEvent("FINISHED"),
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
                finishEvent = FsmEvent.GetFsmEvent("FINISHED"),
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
                    finishEvent = FsmEvent.GetFsmEvent("FINISHED"),
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
