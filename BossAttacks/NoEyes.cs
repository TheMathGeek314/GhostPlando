using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;

namespace GhostPlando.BossAttacks {
    internal class NoEyes {
        internal static GameObject noEyesPrefab;
        internal static GameObject headObject;

        public static void Hook(PlayMakerFSM wosk) {
            headObject = noEyesPrefab.LocateMyFSM("Shot Spawn").GetState("Spawn L").GetFirstActionOfType<SpawnObjectFromGlobalPool>().gameObject.Value;

            PlayMakerFSM ghostFsm = wosk.gameObject.AddComponent<PlayMakerFSM>();
            ghostFsm.FsmName = "No Eyes Ghosts";
            ghostFsm.Fsm.Events = [new FsmEvent("GO"), new FsmEvent("STOP"), new FsmEvent("FINISHED")];
            ghostFsm.AddFsmFloat("Random Y", 0);
            ghostFsm.AddFsmGameObject("Shot", null);

            FsmState startState = ghostFsm.GetState(ghostFsm.ActiveStateName);
            FsmState beginState = ghostFsm.AddState("Begin");
            FsmState loopState = ghostFsm.AddState("Loop");
            startState.AddTransition("GO", beginState);
            beginState.AddTransition("FINISHED", loopState);
            loopState.AddTransition("FINISHED", loopState);
            loopState.AddTransition("STOP", startState);

            //beginState.Actions = audio???
            loopState.Actions = [
                new SpawnObjectFromGlobalPool {
                    gameObject = headObject,
                    spawnPoint = wosk.FsmVariables.GetFsmGameObject("Zero Origin"),
                    position = Vector3.zero,
                    rotation = Vector3.zero,
                    storeObject = ghostFsm.FsmVariables.GetFsmGameObject("Shot")
                },
                new RandomFloat {
                    min = 2,
                    max = 10,
                    storeResult = ghostFsm.FsmVariables.GetFsmFloat("Random Y")
                },
                new SetPosition {
                    gameObject = ghostFsm.FsmGOToOwner("Shot"),
                    vector = Vector3.zero,
                    x = 65,
                    y = ghostFsm.FsmVariables.GetFsmFloat("Random Y"),
                    z = 0,
                    space = Space.World,
                    everyFrame = false,
                    lateUpdate = false
                },
                new SetFsmFloat {
                    gameObject = ghostFsm.FsmGOToOwner("Shot"),
                    fsmName = "Control",
                    variableName = "Speed",
                    setValue = 9,
                    everyFrame = false
                },
                new Wait {
                    time = 0.7f,
                    finishEvent = FsmEvent.Finished,
                    realTime = false
                }
            ];
        }

        public static FsmStateAction[] RunGhosts(PlayMakerFSM self, bool start) {
            return [
                new SendEventByName {
                    eventTarget = new() { target = FsmEventTarget.EventTarget.GameObjectFSM, gameObject = new() { OwnerOption = OwnerDefaultOption.UseOwner }, fsmName = "No Eyes Ghosts" },
                    sendEvent = start ? "GO" : "STOP",
                    delay = 0,
                    everyFrame = false
                }
            ];
        }
    }
}
