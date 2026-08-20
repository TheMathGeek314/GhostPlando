using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;

namespace GhostPlando.BossAttacks {
    internal class NightmareKing {
        internal static GameObject nkgPrefab;
        internal static GameObject flameTrailObject;

        public static void Hook(PlayMakerFSM wosk) {
            flameTrailObject = nkgPrefab.LocateMyFSM("Control").GetState("AD Fire").GetFirstActionOfType<SpawnObjectFromGlobalPoolOverTime>().gameObject.Value;
        }

        public static FsmStateAction[] RunFlameTrail(PlayMakerFSM self) {
            return [
                new SpawnObjectFromGlobalPoolOverTime {
                    gameObject = flameTrailObject,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Self"),
                    position = new Vector3(0, -0.5f, 0.001f),
                    rotation = Vector3.zero,
                    frequency = 0.1f
                }
            ];
        }
    }
}
