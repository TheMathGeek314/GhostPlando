using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;

namespace GhostPlando.BossAttacks {
    internal class FalseKnight {
        internal static GameObject summonPrefab;
        internal const string barrelName = "False Knight Barrels";

        public static void Hook(PlayMakerFSM nosk) {
            GameObject barrelSummon = GameObject.Instantiate(summonPrefab, new Vector3(95, 20, 0), Quaternion.identity);
            PlayMakerFSM fsm = barrelSummon.GetComponent<PlayMakerFSM>();
            FsmVariables fvars = fsm.FsmVariables;
            fvars.GetFsmFloat("Summon Max").Value = 116f;
            fvars.GetFsmFloat("Summon Min").Value = 75f;
            RandomInt action = fsm.GetState("Determine Spawns").GetFirstActionOfType<RandomInt>();
            action.Enabled = true;
            action.min = 20;
            action.max = 20;
            barrelSummon.SetActive(true);
            nosk.AddFsmGameObject(barrelName, barrelSummon);
        }

        public static FsmStateAction[] RunBarrels(PlayMakerFSM self) {
            return [
                new SendEventByName {
                    eventTarget = self.FsmGOToEvent(barrelName),
                    sendEvent = "SUMMON",
                    delay = 0f,
                    everyFrame = false
                }
            ];
        }
    }
}