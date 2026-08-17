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
        internal static GameObject purplePaintObject;

        public static void Hook(PlayMakerFSM nosk) {
            PlayMakerFSM sheoFsm = sheoPrefab.LocateMyFSM("nailmaster_sheo");
            bluePaintObject = sheoFsm.GetState("S 3").GetFirstActionOfType<SpawnObjectFromGlobalPool>().gameObject.Value;
            purplePaintObject = sheoFsm.GetState("Drops").GetFirstActionOfType<FlingObjectsFromGlobalPool>().gameObject.Value;
            nosk.AddFsmGameObject("Paint Shot", null);
            nosk.AddFsmFloat("Paint Speed", 8);
        }

        public static FsmStateAction[] RunBluePaint(PlayMakerFSM self) {
            List<FsmStateAction> actions = [
                new IntCompare {
                    integer1 = self.FsmVariables.GetFsmInt("HP"),
                    integer2 = self.FsmVariables.GetFsmInt("Phase 2-3 HP"),
                    greaterThan = FsmEvent.Finished,
                    everyFrame = false
                },
                new IntCompare {
                    integer1 = self.FsmVariables.GetFsmInt("HP"),
                    integer2 = self.FsmVariables.GetFsmInt("Phase 3-4 HP"),
                    equal = FsmEvent.Finished,
                    lessThan = FsmEvent.Finished,
                    everyFrame = false
                },
                new Lambda(() => {
                    float scale = self.gameObject.transform.localScale.x;
                    self.FsmVariables.GetFsmFloat("Paint Speed").Value = -10 * scale;
                })
            ];
            int[] ySpeeds = [17, 24, 30];
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

        public static FsmStateAction[] RunPurplePaint(PlayMakerFSM self) {
            List<FsmStateAction> actions = new();
            int interval = 4;//this was originally 5
            for(int i = interval - 1; i < 30; i += interval) {
                foreach(int ii in new int[] { -i, i }) {
                    actions.Add(new FlingObjectsFromGlobalPool {
                        gameObject = purplePaintObject,
                        spawnPoint = self.FsmVariables.GetFsmGameObject("Self"),
                        position = new Vector3(ii, (i % (2 * interval) == (interval - 1) ? 10 : 0), 0),
                        spawnMin = 1,
                        spawnMax = 1,
                        speedMin = 0,
                        speedMax = 0,
                        angleMin = 270,
                        angleMax = 270,
                        originVariationX = 1,
                        originVariationY = 4.5f
                    });
                }
            }
            return actions.ToArray();
        }
    }
}
