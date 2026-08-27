using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

namespace GhostPlando.BossAttacks {
    internal class SoulWarrior {
        internal static GameObject spawnerPrefab;

        public static void Hook(PlayMakerFSM wosk) {
            //no hook needed but retained for consistency
        }

        public static FsmStateAction[] SummonFolly(PlayMakerFSM self) {
            return [
                new CreateObject {
                    gameObject = spawnerPrefab,
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
