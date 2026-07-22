using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;

namespace GhostPlando.BossAttacks {
    internal class Collector {
        internal static GameObject collectorPrefab;
        internal static GameObject jarObject;
        private static string fsmName = "Jar Spawner";
        private static string jarStoreName = "Collector Spawn";

        public static void Hook(PlayMakerFSM nosk) {
            jarObject = collectorPrefab.LocateMyFSM("Control").GetState("Spawn").GetFirstActionOfType<SpawnObjectFromGlobalPool>().gameObject.Value;

            PlayMakerFSM jarFsm = nosk.gameObject.AddComponent<PlayMakerFSM>();
            jarFsm.FsmName = fsmName;
            FsmState idleState = jarFsm.AddState("Idle");
            FsmState spawnState = jarFsm.AddState("Spawn");
            FsmState offState = jarFsm.AddState("Off");
            jarFsm.Fsm.Events = [new FsmEvent("TRIGGER"), new FsmEvent("DISABLE"), new FsmEvent("FINISHED")];
            offState.AddTransition("TRIGGER", "Spawn");
            spawnState.AddTransition("FINISHED", "Idle");
            idleState.AddTransition("FINISHED", "Spawn");
            idleState.AddTransition("DISABLE", "Off");

            jarFsm.AddFsmGameObject(jarStoreName, null);
            jarFsm.AddFsmGameObject("Self", nosk.gameObject);
            jarFsm.AddFsmBool("Spawning", false);
            jarFsm.AddFsmFloat("Jar Random", 95);
            jarFsm.SetState("Off");

            idleState.Actions = [
                new Lambda(() => jarFsm.SendEvent(jarFsm.FsmVariables.GetFsmBool("Spawning").Value ? "FINISHED" : "DISABLE"))
            ];
            spawnState.Actions = [
                new RandomFloat {
                    min = 80,
                    max = 111,
                    storeResult = jarFsm.FsmVariables.GetFsmFloat("Jar Random")
                },
                new SpawnObjectFromGlobalPool {
                    gameObject = jarObject,
                    spawnPoint = jarFsm.FsmVariables.GetFsmGameObject("Self"),
                    position = Vector3.zero,
                    rotation = Vector3.zero,
                    storeObject = jarFsm.FsmVariables.GetFsmGameObject(jarStoreName)
                },
                new SetPosition {
                    gameObject = jarFsm.FsmGOToOwner(jarStoreName),
                    vector = Vector3.zero,
                    x = jarFsm.FsmVariables.GetFsmFloat("Jar Random"),
                    y = 17,
                    z = 0,
                    space = Space.World
                },
                new Lambda(() => {
                    SpawnJarControl sjc = jarFsm.FsmVariables.GetFsmGameObject(jarStoreName).Value.GetComponent<SpawnJarControl>();
                    sjc.spawnY = 17;
                    sjc.breakY = 4;
                }),
                new Wait {
                    time = 0.52f,
                    finishEvent = FsmEvent.GetFsmEvent("FINISHED"),
                    realTime = false
                }
            ];
        }

        public static FsmStateAction[] Run(PlayMakerFSM self) {
            return [
                new Lambda(() => {
                    PlayMakerFSM collectorFsm = self.gameObject.LocateMyFSM(fsmName);
                    collectorFsm.FsmVariables.GetFsmBool("Spawning").Value = true;
                    collectorFsm.SendEvent("TRIGGER");
                }),
                new Wait {
                    time = 4.5f,
                    finishEvent = FsmEvent.GetFsmEvent("FINISHED"),
                    realTime = false
                }
            ];
        }

        public static FsmStateAction[] Stop(PlayMakerFSM self) {
            return [
                new Lambda(() => self.gameObject.LocateMyFSM(fsmName).FsmVariables.GetFsmBool("Spawning").Value = false)
            ];
        }
    }
}
