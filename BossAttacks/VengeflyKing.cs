using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;

namespace GhostPlando.BossAttacks {
    internal class VengeflyKing {
        internal static GameObject vfkPrefab;
        internal static GameObject vengeflyObject;

        public static void Hook(PlayMakerFSM wosk) {
            vengeflyObject = vfkPrefab.LocateMyFSM("Big Buzzer").GetState("Summon").GetFirstActionOfType<CreateObject>().gameObject.Value;
        }

        public static FsmStateAction[] SummonVengefly(PlayMakerFSM self) {
            return [
                new Vector3Add {
                    vector3Variable = self.FsmVariables.GetFsmVector3("Spawn Vector"),
                    addVector = new Vector3(0, 7, 17),
                    perSecond = false,
                    everyFrame = false
                },
                new CreateObject {
                    gameObject = vengeflyObject,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Zero Origin"),
                    position = self.FsmVariables.GetFsmVector3("Spawn Vector"),
                    rotation = Vector3.zero,
                    storeObject = self.FsmVariables.GetFsmGameObject("Spawned Enemy"),
                    networkInstantiate = false,
                    networkGroup = 0
                }
            ];
        }
    }
}
