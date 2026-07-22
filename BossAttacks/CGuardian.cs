using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

namespace GhostPlando.BossAttacks {
    internal class CGuardian {
        internal static GameObject turretPrefab;

        public static void Hook(PlayMakerFSM nosk) {
            for(float f = 78f; f < 115; f += 4f) {
                GameObject.Instantiate(turretPrefab, new Vector3(f, 19.5f, 0.006f), Quaternion.identity).SetActive(true);
            }
        }

        public static FsmStateAction[] RunCeilingLasers(PlayMakerFSM self) {
            return [
                new SendEventByName {
                    eventTarget = new FsmEventTarget { target = FsmEventTarget.EventTarget.BroadcastAll },
                    sendEvent = "LASER SHOOT",
                    delay = 0f,
                    everyFrame = false
                }
            ];
        }
    }
}
