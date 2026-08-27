using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;

namespace GhostPlando.BossAttacks {
    internal class HiveKnight {
        internal static GameObject battleScenePrefab;
        internal static GameObject globPrefab;
        public static void Hook(PlayMakerFSM wosk) {
            globPrefab = battleScenePrefab.FindChild(["Globs", "Hive Knight Glob"]);
            On.PlayMakerFSM.OnEnable += EditBeeDroppers;
            foreach(string name in new string[] { "Droppers", "Swarm Audio" })
                wosk.AddFsmGameObject($"Hive Knight {name}", GameObject.Instantiate(battleScenePrefab.FindChild(name)));
            wosk.FsmVariables.GetFsmGameObject("Hive Knight Swarm Audio").Value.SetActive(false);

            PlayMakerFSM globFsm = wosk.gameObject.AddComponent<PlayMakerFSM>();
            globFsm.FsmName = "Hivenet Glob Barb Control";
            globFsm.Fsm.Events = [new FsmEvent("START"), new FsmEvent("GLOB"), new FsmEvent("BARB"), new FsmEvent("END"), new FsmEvent("FINISHED")];
            globFsm.AddFsmGameObject("Wosk", wosk.gameObject);
            FsmState startState = globFsm.GetState(globFsm.ActiveStateName);
            FsmState decisionState = globFsm.AddState("Glob or Barb");
            FsmState globState = globFsm.AddState("Chose Glob");
            FsmState barbState = globFsm.AddState("Chose Barb");
            FsmState glob2State = globFsm.AddState("Chose Glob 2");
            FsmState barb2State = globFsm.AddState("Chose Barb 2");
            FsmState globEndState = globFsm.AddState("Glob End Early");
            FsmState barbEndState = globFsm.AddState("Barb End Early");
            startState.AddTransition("START", decisionState);
            decisionState.AddTransition("GLOB", globState);
            decisionState.AddTransition("BARB", barbState);
            globState.AddTransition("FINISHED", glob2State);
            barbState.AddTransition("FINISHED", barb2State);
            glob2State.AddTransition("FINISHED", globState);
            barb2State.AddTransition("FINISHED", barbState);
            globState.AddTransition("END", globEndState);
            glob2State.AddTransition("END", startState);
            globEndState.AddTransition("FINISHED", startState);
            barbState.AddTransition("END", barbEndState);
            barb2State.AddTransition("END", startState);
            barbEndState.AddTransition("FINISHED", startState);

            FsmFloat wx = globFsm.AddFsmFloat("wx", 0);
            FsmFloat wy = globFsm.AddFsmFloat("wy", 0);
            FsmFloat sx = globFsm.AddFsmFloat("sx", 0);
            FsmGameObject globarbInstance = globFsm.AddFsmGameObject("Globarb Instance", null);

            decisionState.Actions = [
                new SendEventByScale {
                    gameObject = wosk.FsmGOToOwner("Self"),
                    eventTarget = wosk.FsmGOToEvent("Self"),
                    xScale = true,
                    positiveEvent = FsmEvent.GetFsmEvent("BARB"),
                    negativeEvent = FsmEvent.GetFsmEvent("GLOB"),
                    space = Space.World
                }
            ];
            globState.Actions = [
                new GetPosition {
                    gameObject = globFsm.FsmGOToOwner("Wosk"),
                    vector = Vector3.zero,
                    x = wx,
                    y = wy,
                    z = 0,
                    space = Space.World,
                    everyFrame = false
                },
                new Lambda(() => {
                    GameObject glob = GameObject.Instantiate(globPrefab, new Vector3(wx.Value, wy.Value, 0), Quaternion.identity, null);
                    PlayMakerFSM gf = glob.GetComponent<PlayMakerFSM>();
                    FsmState setInertState = gf.GetState("Set Inert");
                    setInertState.GetFirstActionOfType<SetParent>().Enabled = false;
                    setInertState.GetFirstActionOfType<SetPosition>().y = 0;
                    gf.GetState("End").AddLastAction(new DestroySelf());
                    gf.GetState("Tween Up").GetFirstActionOfType<Translate>().y = Random.Range(0f, 4.5f);
                    glob.SetActive(true);
                    globarbInstance.Value = glob;
                }),
                new NextFrameEvent {
                    sendEvent = FsmEvent.Finished
                }
            ];
            glob2State.Actions = [
                new SendEventByName {
                    eventTarget = globFsm.FsmGOToEvent("Globarb Instance"),
                    sendEvent = "FIRE",
                    delay = 0,
                    everyFrame = false
                },
                new Wait {
                    time = 0.5f,
                    finishEvent = FsmEvent.Finished,
                    realTime = false
                }
            ];
            globEndState.Actions = [
                new SendEventByName {
                    eventTarget = globFsm.FsmGOToEvent("Globarb Instance"),
                    sendEvent = "FIRE",
                    delay = 0,
                    everyFrame = false
                }
            ];
            barbState.Actions = BossAttacks.HornetS.SpawnBarb(globFsm);
            barb2State.Actions = BossAttacks.HornetS.TriggerBarb(globFsm);
            barbEndState.Actions = BossAttacks.HornetS.TriggerBarbEarly(globFsm);
        }

        public static FsmStateAction[] RunGlobarbs(PlayMakerFSM self) {
            return [
                new SendEventByName {
                    eventTarget = new FsmEventTarget {
                        gameObject = self.FsmGOToOwner("Self"),
                        target = FsmEventTarget.EventTarget.GameObjectFSM,
                        fsmName = "Hivenet Glob Barb Control"
                    },
                    sendEvent = "START",
                    delay = 0,
                    everyFrame = false
                }
            ];
        }

        public static FsmStateAction[] EndGlobarbs(PlayMakerFSM self) {
            return [
                new SendEventByName {
                    eventTarget = new FsmEventTarget {
                        gameObject = self.FsmGOToOwner("Self"),
                        target = FsmEventTarget.EventTarget.GameObjectFSM,
                        fsmName = "Hivenet Glob Barb Control"
                    },
                    sendEvent = "END",
                    delay = 0,
                    everyFrame = false
                }
            ];
        }

        public static FsmStateAction[] RunSwarm(PlayMakerFSM self) {
            return [
                new ActivateGameObject {
                    gameObject = self.FsmGOToOwner("Hive Knight Swarm Audio"),
                    activate = true,
                    recursive = false,
                    resetOnExit = false,
                    everyFrame = false
                },
                new SendEventByName {
                    eventTarget = self.FsmGOToEvent("Hive Knight Droppers", true),
                    sendEvent = "SWARM",
                    delay = 0,
                    everyFrame = false
                },
                new Wait {
                    time = 1.5f,
                    finishEvent = FsmEvent.Finished,
                    realTime = false
                }
            ];
        }

        public static void EditBeeDroppers(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self) {
            orig(self);
            if(GameManager.instance.sceneName == "Deepnest_32" && self.gameObject.name.StartsWith("Bee Dropper") && self.FsmName == "Control") {
                FsmVariables fv = self.FsmVariables;
                fv.GetFsmFloat("X Left").Value = 72;
                fv.GetFsmFloat("X Right").Value = 118;
                fv.GetFsmFloat("Start Y").Value = 18;
                self.GetState("Swarm").GetFirstActionOfType<FloatCompare>().float2.Value = 0;
            }
        }
    }
}
