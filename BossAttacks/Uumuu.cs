using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;

namespace GhostPlando.BossAttacks {
    internal class Uumuu {
        internal static GameObject jellyPrefab;
        internal static GameObject zapPoolObject;
        private static string fsmName = "Zap Control";
        private static string zapStoreName = "Uumuu Zap";

        public static void Hook(PlayMakerFSM nosk) {
            zapPoolObject = jellyPrefab.LocateMyFSM("Mega Jellyfish").GetState("Gen").GetFirstActionOfType<SpawnObjectFromGlobalPool>().gameObject.Value;

            PlayMakerFSM zapFsm = nosk.gameObject.AddComponent<PlayMakerFSM>();
            zapFsm.FsmName = fsmName;
            FsmState idleState = zapFsm.AddState("Idle");
            FsmState slowState = zapFsm.AddState("Slow Zap");
            FsmState offState = zapFsm.AddState("Off");
            zapFsm.Fsm.Events = [new FsmEvent("TRIGGER"), new FsmEvent("DISABLE"), new FsmEvent("FINISHED")];
            offState.AddTransition("TRIGGER", "Slow Zap");
            slowState.AddTransition("FINISHED", "Idle");
            idleState.AddTransition("FINISHED", "Slow Zap");
            idleState.AddTransition("DISABLE", "Off");

            zapFsm.AddFsmGameObject(zapStoreName, null);
            zapFsm.AddFsmGameObject("Self", nosk.gameObject);
            zapFsm.AddFsmBool("Running", false);
            zapFsm.SetState("Off");

            idleState.Actions = [
                new Lambda(() => zapFsm.SendEvent(zapFsm.FsmVariables.GetFsmBool("Running").Value ? "FINISHED" : "DISABLE"))
            ];
            slowState.Actions = [
                new SpawnObjectFromGlobalPool {
                    gameObject = new FsmGameObject { Value = zapPoolObject },
                    spawnPoint = zapFsm.FsmVariables.GetFsmGameObject("Self"),
                    position = Vector3.zero,
                    rotation = Vector3.zero,
                    storeObject = zapFsm.FsmVariables.GetFsmGameObject(zapStoreName)
                },
                new SetFsmFloat {
                    gameObject = zapFsm.FsmGOToOwner(zapStoreName),
                    fsmName = "Control",
                    variableName = "Antic Time",
                    setValue = 0.65f,
                    everyFrame = false
                },
                new Wait {
                    time = 0.3f,
                    finishEvent = FsmEvent.GetFsmEvent("FINISHED"),
                    realTime = false
                }
            ];
        }

        public static FsmStateAction[] RunSlowZap(PlayMakerFSM self) {
            return [
                new Lambda(() => {
                    FsmVariables fv = self.FsmVariables;
                    int hp = fv.GetFsmInt("HP").Value;
                    if(!fv.GetFsmBool("Will Spit").Value && hp <= fv.GetFsmInt("Phase 2-3 HP").Value && hp > fv.GetFsmInt("Phase 3-4 HP").Value) {
                        PlayMakerFSM uumuuFsm = self.gameObject.LocateMyFSM(fsmName);
                        uumuuFsm.FsmVariables.GetFsmBool("Running").Value = true;
                        uumuuFsm.SendEvent("TRIGGER");
                    }
                })
            ];
        }

        public static FsmStateAction[] StopSlowZap(PlayMakerFSM self) {
            return [
                new Lambda(() => self.gameObject.LocateMyFSM(fsmName).FsmVariables.GetFsmBool("Running").Value = false)
            ];
        }
    }
}
