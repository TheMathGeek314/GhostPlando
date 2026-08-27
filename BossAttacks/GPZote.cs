using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;

namespace GhostPlando.BossAttacks {
    internal class GPZote {
        internal static GameObject zotelingPrefab;

        public static void Hook(PlayMakerFSM wosk) {
            wosk.AddFsmFloat("Zoteling Random X", 0);
            wosk.AddFsmFloat("Zoteling Random Y", 0);
            On.PlayMakerFSM.OnEnable += EditZotelings;
        }

        internal static void EditZotelings(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self) {
            orig(self);
            if(self.gameObject.name.StartsWith("Zoteling") && self.FsmName == "Control" && GameManager.instance.sceneName == "Deepnest_32") {
                self.GetState("Dormant").AddTransition("FINISHED", "Ball");
                self.GetState("Reset").AddLastAction(new DestroySelf());
                self.GetState("Choice").InsertAction(new SendEventByName {
                    eventTarget = self.gameObject.GameObjectToEventTarget(),
                    sendEvent = "BUZZER",
                    delay = 0,
                    everyFrame = false
                }, 3);
            }
        }

        public static FsmStateAction[] SummonZoteling(PlayMakerFSM self) {
            return [
                new CreateObject {
                    gameObject = zotelingPrefab,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Zero Origin"),
                    position = self.FsmVariables.GetFsmVector3("Spawn Vector"),
                    rotation = Vector3.zero,
                    storeObject = self.FsmVariables.GetFsmGameObject("Spawned Enemy"),
                    networkInstantiate = false,
                    networkGroup = 0
                },
                new RandomFloat {
                    min = 20,
                    max = 5,//Thank you Team Cherry <3
                    storeResult = self.FsmVariables.GetFsmFloat("Zoteling Random X")
                },
                new RandomFloat {
                    min = 20,
                    max = 30,
                    storeResult = self.FsmVariables.GetFsmFloat("Zoteling Random Y")
                },
                new SetVelocity2d {
                    gameObject = self.FsmGOToOwner("Spawned Enemy"),
                    vector = Vector2.zero,
                    x = self.FsmVariables.GetFsmFloat("Zoteling Random X"),
                    y = self.FsmVariables.GetFsmFloat("Zoteling Random Y"),
                    everyFrame = false
                },
                new ActivateGameObject {
                    gameObject = self.FsmGOToOwner("Spawned Enemy"),
                    activate = true,
                    recursive = false,
                    resetOnExit = false,
                    everyFrame = false
                }
            ];
        }
    }
}
