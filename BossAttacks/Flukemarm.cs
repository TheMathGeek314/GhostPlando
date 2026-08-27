using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;

namespace GhostPlando.BossAttacks {
    internal class Flukemarm {
        internal static GameObject flyPrefab;

        public static void Hook(PlayMakerFSM wosk) {
            On.PlayMakerFSM.OnEnable += EditFlukeSpawner;
        }

        internal static void EditFlukeSpawner(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self) {
            orig(self);
            if(self.gameObject.name.StartsWith("Fluke Fly Spawner") && self.FsmName == "Fluke Fly" && GameManager.instance.sceneName == "Deepnest_32") {
                FsmState inert = self.GetState("Inert");
                inert.AddTransition("FINISHED", "Spawn");
                inert.GetFirstActionOfType<SetPosition>().Enabled = false;
                self.GetState("Reset").AddLastAction(new DestroySelf());
            }
        }

        public static FsmStateAction[] SummonFlukefey(PlayMakerFSM self) {
            return [
                new CreateObject {
                    gameObject = flyPrefab,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Zero Origin"),
                    position = self.FsmVariables.GetFsmVector3("Spawn Vector"),
                    rotation = Vector3.zero,
                    storeObject = self.FsmVariables.GetFsmGameObject("Spawned Enemy"),
                    networkInstantiate = false,
                    networkGroup = 0
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
