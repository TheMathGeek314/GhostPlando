using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;

namespace GhostPlando.BossAttacks {
    internal class Galien {
        internal static GameObject hammerPrefab;
        private static GameObject woskRef;

        public static void Hook(PlayMakerFSM wosk) {
            woskRef = wosk.gameObject;
            wosk.AddFsmGameObject("Galien Hammer", null);
            On.PlayMakerFSM.OnEnable += EditHammer;
        }

        internal static void EditHammer(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self) {
            orig(self);
            if(GameManager.instance.sceneName != "Deepnest_32" || !self.gameObject.name.StartsWith("Galien Hammer"))
                return;
            if(self.FsmName == "Control") {
                FsmState init = self.GetState("Init");
                init.AddTransition("FINISHED", "Emerge");
                init.GetFirstActionOfType<SetVector3XYZ>().Enabled = false;
                GetPosition gp = init.GetFirstActionOfType<GetPosition>();
                gp.gameObject = woskRef.GameObjectToOwner();
                gp.vector = self.FsmVariables.GetFsmVector3("Float Vector");
                FsmState emerge = self.GetState("Emerge");
                emerge.GetFirstActionOfType<iTweenMoveTo>().time = 0.5f;
                emerge.GetFirstActionOfType<iTweenRotateTo>().time = 0.5f;
            }
            else if(self.FsmName == "Attack") {
                self.GetState("Antic").GetFirstActionOfType<FindGameObject>().Enabled = false;
                FsmVariables fv = self.FsmVariables;
                fv.GetFsmFloat("Floor Y").Value = 4.88f;
                fv.GetFsmFloat("Wall L X").Value = 70;
                fv.GetFsmFloat("Wall R X").Value = 120;
                FsmState chase = self.GetState("Chase");
                foreach(string fsmEvent in new string[] { "WALL L", "WALL R" }) {
                    chase.RemoveTransitionsOn(fsmEvent);
                    chase.AddTransition(fsmEvent, "Floor Bounce");
                }
                chase.GetFirstActionOfType<ChaseObjectGround>().acceleration.Value = 1f;
                chase.GetFirstActionOfType<AccelerateVelocity>().yAccel.Value = -0.35f;
                self.GetState("Floor Bounce").AddLastAction(new DestroySelf());
            }
        }

        public static FsmStateAction[] RaiseHammer(PlayMakerFSM self) {
            return [
                new CreateObject {
                    gameObject = hammerPrefab,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Self"),
                    position = Vector3.zero,
                    rotation = Vector3.zero,
                    storeObject = self.FsmVariables.GetFsmGameObject("Galien Hammer"),
                    networkInstantiate = false,
                    networkGroup = 0
                },
                new ActivateGameObject {
                    gameObject = self.FsmGOToOwner("Galien Hammer"),
                    activate = true,
                    recursive = false,
                    resetOnExit = false,
                    everyFrame = false
                },
                new SetIntValue {
                    intVariable = self.FsmVariables.GetFsmInt("Summon Quantity"),
                    intValue = 3,
                    everyFrame = false
                }
            ];
        }

        public static FsmStateAction[] ThrowHammer(PlayMakerFSM self) {
            return [
                new SendEventByName {
                    eventTarget = self.FsmGOToEvent("Galien Hammer"),
                    sendEvent = "HAMMER ATTACK",
                    delay = 0,
                    everyFrame = false
                },
                new Wait {
                    time = 1.5f,
                    finishEvent = FsmEvent.Finished,
                    realTime = false
                }
            ];
        }
    }
}
