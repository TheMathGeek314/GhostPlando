using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;

namespace GhostPlando.BossAttacks {
    internal class Markoth {
        internal static GameObject markothPrefab;
        internal static GameObject nailObject;

        public static void Hook(PlayMakerFSM wosk) {
            nailObject = markothPrefab.LocateMyFSM("Attacking").GetState("Nail").GetFirstActionOfType<SpawnObjectFromGlobalPool>().gameObject.Value;
            wosk.AddFsmVector3("Markoth Vector", Vector3.zero);
            wosk.AddFsmGameObject("Markoth Shot", null);
            for(int i = 0; i < 3; i++)
                nailObject.Spawn().Recycle();
        }

        public static FsmStateAction[] RunNails(PlayMakerFSM self) {
            FsmVariables fv = self.FsmVariables;
            return [
                new Lambda(() => {
                    Vector2 cgVector = fv.GetFsmVector2("CG Ray Vector").Value.normalized * 2;
                    fv.GetFsmVector3("Markoth Vector").Value = new Vector3(-cgVector.y, cgVector.x);
                }),
                new SpawnObjectFromGlobalPool {
                    gameObject = nailObject,
                    spawnPoint = fv.GetFsmGameObject("Self"),
                    position = fv.GetFsmVector3("Markoth Vector"),
                    rotation = Vector3.zero,
                    storeObject = fv.GetFsmGameObject("Markoth Shot")
                },
                new EditShotFsm(),
                new Vector3Multiply {
                    vector3Variable = fv.GetFsmVector3("Markoth Vector"),
                    multiplyBy = -1,
                    everyFrame = false
                },
                new SpawnObjectFromGlobalPool {
                    gameObject = nailObject,
                    spawnPoint = fv.GetFsmGameObject("Self"),
                    position = fv.GetFsmVector3("Markoth Vector"),
                    rotation = Vector3.zero,
                    storeObject = fv.GetFsmGameObject("Markoth Shot")
                },
                new EditShotFsm()
            ];
        }
    }

    internal class EditShotFsm: FsmStateAction {
        public override void OnEnter() {
            PlayMakerFSM self = Fsm.FsmComponent.FsmVariables.GetFsmGameObject("Markoth Shot").Value.GetComponent<PlayMakerFSM>();
            FsmState setPos = self.GetState("Set Pos");
            setPos.RemoveActionsOfType<RandomFloat>();
            setPos.AddFirstAction(new Lambda(() => {
                self.FsmVariables.GetFsmFloat("Tele X").Value = self.transform.position.x;
                self.FsmVariables.GetFsmFloat("Tele Y").Value = self.transform.position.y;
            }));
            FsmState checkDist = self.GetState("Check Distance");
            for(int i = 0; i < 3; i++)
                checkDist.Actions[i].Enabled = false;
            Finish();
        }
    }
}
