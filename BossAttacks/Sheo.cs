using System.Collections.Generic;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;

namespace GhostPlando.BossAttacks {
    internal class Sheo {
        internal static GameObject sheoPrefab;
        internal static GameObject bluePaintObject;

        public static void Hook(PlayMakerFSM nosk) {
            bluePaintObject = sheoPrefab.LocateMyFSM("nailmaster_sheo").GetState("S 3").GetFirstActionOfType<SpawnObjectFromGlobalPool>().gameObject.Value;
            nosk.AddFsmGameObject("Paint Shot", null);
            nosk.AddFsmFloat("Paint Speed", 8);
        }

        public static FsmStateAction[] RunBluePaint(PlayMakerFSM self) {
            List<FsmStateAction> actions = [
                new IntCompare {
                    integer1 = self.FsmVariables.GetFsmInt("HP"),
                    integer2 = self.FsmVariables.GetFsmInt("Phase 2-3 HP"),
                    greaterThan = FsmEvent.GetFsmEvent("FINISHED"),
                    everyFrame = false
                },
                new IntCompare {
                    integer1 = self.FsmVariables.GetFsmInt("HP"),
                    integer2 = self.FsmVariables.GetFsmInt("Phase 3-4 HP"),
                    equal = FsmEvent.GetFsmEvent("FINISHED"),
                    lessThan = FsmEvent.GetFsmEvent("FINISHED"),
                    everyFrame = false
                },
                new Lambda(() => {
                    float scale = self.gameObject.transform.localScale.x;
                    self.FsmVariables.GetFsmFloat("Paint Speed").Value = -10 * scale;
                })
            ];
            int[] ySpeeds = [17, 24, 30];
            /*actions.Add(new Lambda(() => {
                float scale = self.gameObject.transform.localScale.x;
                self.FsmVariables.GetFsmFloat("Paint Speed").Value = -10 * scale;
            }));*/
            for(int i = 0; i < 3; i++) {
                actions.Add(new SpawnObjectFromGlobalPool {
                    gameObject = bluePaintObject,
                    spawnPoint = self.FsmVariables.GetFsmGameObject("Self"),
                    position = new Vector3(0, 1, 0),
                    rotation = Vector3.zero,
                    storeObject = self.FsmVariables.GetFsmGameObject("Paint Shot")
                });
                actions.Add(new SetVelocity2d {
                    gameObject = self.FsmGOToOwner("Paint Shot"),
                    vector = Vector2.zero,
                    x = self.FsmVariables.GetFsmFloat("Paint Speed"),
                    y = ySpeeds[i],
                    everyFrame = false
                });
                actions.Add(new FloatMultiply {
                    floatVariable = self.FsmVariables.GetFsmFloat("Paint Speed"),
                    multiplyBy = 1.35f,
                    everyFrame = false
                });
            }
            return actions.ToArray();
        }
    }
}
