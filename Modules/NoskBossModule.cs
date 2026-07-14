using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;
using ItemChanger.Modules;

namespace GhostPlando.Modules {
    internal class NoskBossModule: Module {
        internal static GameObject titlePrefab;
        internal static GameObject titleFaderPrefab;
        private const string titleName = "Nosk Title";
        private const string titleFaderName = "Nosk Title Fader";

        public override void Initialize() {
            On.Language.Language.Get_string_string += LanguageGet;
            On.GameManager.OnNextLevelReady += ResetEncounter;
            On.PlayMakerFSM.OnEnable += EditNosk;
            On.GameManager.OnNextLevelReady += EditArena;
        }

        public override void Unload() {
            On.Language.Language.Get_string_string -= LanguageGet;
            On.GameManager.OnNextLevelReady -= ResetEncounter;
            On.PlayMakerFSM.OnEnable -= EditNosk;
            On.GameManager.OnNextLevelReady -= EditArena;
        }

        private string LanguageGet(On.Language.Language.orig_Get_string_string orig, string key, string sheetTitle) {
            if(sheetTitle == "Titles") {
                if(key == "NIGHTMARE_GRIMM_MAIN")
                    return "Nosk";
                if(key == "NIGHTMARE_GRIMM_SUPER")
                    return "Mimic Paragon";
            }
            return orig(key, sheetTitle);
        }

        private void ResetEncounter(On.GameManager.orig_OnNextLevelReady orig, GameManager self) {
            orig(self);
            PlayerData.instance.SetBool(nameof(PlayerData.encounteredMimicSpider), false);
        }

        private void EditNosk(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self) {
            orig(self);
            if(self.gameObject.scene.name != "Deepnest_32" || self.gameObject.name != "Mimic Spider" || self.FsmName != "Mimic Spider")
                return;

            //title
            {
                GameObject title = GameObject.Instantiate(titlePrefab);
                GameObject fader = GameObject.Instantiate(titleFaderPrefab);
                foreach(TextMeshPro tmp in title.transform.GetComponentsInChildren<TextMeshPro>()) {
                    FsmVariables fv = tmp.GetComponent<PlayMakerFSM>().FsmVariables;
                    fv.GetFsmColor("Up Colour").Value = new Color(1, 0.4f, 0, 1);
                    fv.GetFsmColor("Down Colour").Value = new Color(1, 0.4f, 0, 0);
                    fv.GetFsmColor("Start Colour").Value = new Color(1, 0.4f, 0, 0);
                }
                self.AddFsmGameObject(titleName, title);
                self.AddFsmGameObject(titleFaderName, fader);
                FsmState roarEnd = self.GetState("Roar End");
                FsmState trans1 = self.GetState("Trans 1");
                new List<FsmStateAction> {
                    new Wait {
                        time = 2.5f,
                        finishEvent = new FsmEvent("FINISHED"),
                        realTime = false
                    },
                    new Lambda(() => {
                        Vector3 titlePosition = GameCameras.instance.mainCamera.transform.position + new Vector3(-0.4479f, 4.8225f, 0);
                        title.transform.position = new Vector3(titlePosition.x, titlePosition.y, -1.695f);
                        fader.transform.position = title.transform.position + new Vector3(0.5f, -2.67f, -8.93f);
                        fader.transform.localScale = new Vector3(37.6446f, 26.62126f, 1);
                    }),
                    new ActivateGameObject {
                        gameObject = self.FsmGOToOwner(titleFaderName),
                        activate = true,
                        recursive = false,
                        resetOnExit = false,
                        everyFrame = false
                    },
                    new ActivateGameObject {
                        gameObject = self.FsmGOToOwner(titleName),
                        activate = true,
                        recursive = false,
                        resetOnExit = false,
                        everyFrame = false
                    },
                    new TransitionToAudioSnapshot {
                        snapshot = trans1.GetFirstActionOfType<TransitionToAudioSnapshot>().snapshot,
                        transitionTime = 0
                    },
                    new ApplyMusicCue {
                        musicCue = trans1.GetFirstActionOfType<ApplyMusicCue>().musicCue,
                        delayTime = 0,
                        transitionTime = 0
                    }
                }.ForEach(action => roarEnd.AddLastAction(action));
                trans1.GetFirstActionOfType<TransitionToAudioSnapshot>().Enabled = false;
                trans1.GetFirstActionOfType<ApplyMusicCue>().Enabled = false;
                FsmState roarInit = self.GetState("Roar Init");
                roarInit.AddFirstAction(new SendEventByName {
                    eventTarget = new FsmEventTarget {
                        target = FsmEventTarget.EventTarget.GameObject,
                        gameObject = self.FsmGOToOwner(titleName),
                        sendToChildren = true
                    },
                    sendEvent = "DOWN",
                    delay = 0,
                    everyFrame = false
                });
                roarInit.InsertAction(new iTweenFadeTo {
                    gameObject = self.FsmGOToOwner(titleFaderName),
                    alpha = 0,
                    includeChildren = true,
                    namedValueColor = "_Color",
                    time = 0.1f,
                    delay = 0,
                    easeType = iTween.EaseType.linear,
                    loopType = iTween.LoopType.none,
                    finishEvent = new FsmEvent("FINISHED"),
                    realTime = false,
                    stopOnExit = true,
                    loopDontFinish = false
                }, 1);
            }

            self.GetState("Spit Antic").GetFirstActionOfType<SetPosition>().y.Value = 4.84f;

            BossAttacks.FalseKnight.Hook(self);
            BossAttacks.HornetP.Hook(self);
            BossAttacks.SoulMaster.Hook(self);
            BossAttacks.Gorb.Hook(self);
            BossAttacks.Uumuu.Hook(self);
            BossAttacks.Sheo.Hook(self);
            BossAttacks.Collector.Hook(self);

            //testing falling barrels
            //self.GetState("Roof Drop").AddFirstAction(BossAttacks.FalseKnight.Run(self));

            //testing threadstorm
            //self.GetState("Rising").AddFirstActions(BossAttacks.HornetP.RunThreadstorm(self));

            //testing needle
            self.GetState("Land 2").AddFirstActions(BossAttacks.HornetP.RunNeedle(self));

            //testing soulmaster shockwave
            self.AddFsmBool("Falling from Ceiling", false);
            self.GetState("To Fall").AddFirstAction(new SetBoolValue {
                boolVariable = self.FsmVariables.GetFsmBool("Falling from Ceiling"),
                boolValue = true,
                everyFrame = false
            });
            self.GetState("Land 2").AddLastActions(BossAttacks.SoulMaster.RunQuake(self));

            //testing gorb spears
            self.GetState("Spit 1").AddFirstActions(BossAttacks.Gorb.RunSpears(self));

            //testing uumuu slow electricity
            self.GetState("Charge Start").AddFirstActions(BossAttacks.Uumuu.RunSlowZap(self));
            self.GetState("Wait").AddFirstActions(BossAttacks.Uumuu.StopSlowZap(self));

            //testing sheo blue paint
            self.GetState("Launch").AddFirstActions(BossAttacks.Sheo.RunBluePaint(self));

            //testing collector jars
            FsmState jarDrop = self.AddState("Roof Drop Jars");
            jarDrop.AddFirstAction(new Wait { time = 4.5f, finishEvent = new FsmEvent("FINISHED"), realTime = false });
            jarDrop.AddFirstActions(BossAttacks.Collector.Run(self));
            self.GetState("Roof Antic").RemoveTransitionsOn("FINISHED");
            self.GetState("Roof Antic").AddTransition("FINISHED", "Roof Drop Jars");
            jarDrop.AddTransition("FINISHED", "Fall Antic");
            self.GetState("Fall Antic").AddFirstActions(BossAttacks.Collector.Stop(self));
        }

        private void EditArena(On.GameManager.orig_OnNextLevelReady orig, GameManager self) {
            orig(self);
            if(self.sceneName != "Deepnest_32")
                return;
            
            GameObject chunk2 = GameObject.Find("Chunk 0 2");
            chunk2.GetComponents<EdgeCollider2D>()[0].points = GhostFsmUtil.AssignFromTuples2((0, 1), (0, 0), (32, 0), (32, 3), (0, 3), (0, 1));
            Mesh mesh2 = new Mesh {
                vertices = GhostFsmUtil.AssignFromTuples3((0, 0), (32, 0), (0, 3), (32, 3), (0, 7), (7, 7), (0, 26), (7, 26)),
                triangles = [0, 2, 1, 2, 3, 1, 4, 6, 5, 6, 7, 5],
                colors = Enumerable.Repeat(Color.black, 8).ToArray(),
                uv = GhostFsmUtil.AssignFromTuples2((0, 0), (1, 0), (0, 1), (1, 1))
            };
            mesh2.RecalculateNormals();
            mesh2.RecalculateBounds();
            chunk2.GetComponent<MeshFilter>().mesh = mesh2;
            
            GameObject chunk3 = GameObject.Find("Chunk 0 3");
            chunk3.GetComponents<EdgeCollider2D>()[0].points = GhostFsmUtil.AssignFromTuples2((0, 1), (0, 0), (32, 0), (32, 4), (20.8f, 4), (20.8f, 3), (0, 3), (0, 1));
            Mesh mesh3 = new Mesh {
                vertices = GhostFsmUtil.AssignFromTuples3((0, 0), (32, 0), (0, 3), (32, 3), (22, 8), (32, 8), (22, 26), (32, 36)),
                triangles = [0, 2, 1, 2, 3, 1, 4, 6, 5, 6, 7, 5],
                colors = Enumerable.Repeat(Color.black, 8).ToArray(),
                uv = GhostFsmUtil.AssignFromTuples2((0, 0), (1, 0), (0, 1), (1, 1))
            };
            mesh3.RecalculateNormals();
            mesh3.RecalculateBounds();
            chunk3.GetComponent<MeshFilter>().mesh = mesh3;

            foreach(string name in new string[] { "deepnest_corner3 (2)", "deep_floor_tilable2 (35)", "deep_floor_tilable2 (34)", "deepnest_corner3 (1)", "deep_egg_set_10 (5)" })
                GameObject.Find(name).transform.position += new Vector3(0, -3f, 0);
            foreach(string name in new string[] { "bone_deep_0103_b_sil (11)",
                "d_break_0043_deep_cocoon_break1 (9)", "d_break_0043_deep_cocoon_break1 (10)", "d_break_0043_deep_cocoon_break1 (11)", "d_break_0043_deep_cocoon_break1 (19)",
                "deep_egg_set_09 (1)", "deep_egg_set_09 (2)", "deep_egg_set_09 (3)",
                "deep_fg_small (77)", "deep_fg_small (78)", "deep_fg_small (79)" })
                GameObject.Find(name).transform.position += new Vector3(0, -2f, 0);
            foreach(string name in new string[] { "deep_floor_tilable2 (20)", "deep_floor_tilable2 (21)", "deep_floor_tilable2 (23)",
                "deepnest_stalactites (39)", "deepnest_stalactites (51)", "deepnest_stalactites (52)" })
                GameObject.Find(name).transform.position += new Vector3(0, -1f, 0);
            foreach((string name, Vector3 position) in new (string, Vector3)[] {
                ("bone_deep_0103_b_sil (10)", new Vector3(95.9f, 2.7f, -2.9f)),
                ("bone_deep_0103_b_sil (13)", new Vector3(90.16f, 2.08f, -2.99f)),
                ("d_break_0043_deep_cocoon_break1 (6)", new Vector3(104.57f, 3.14f, -6.88f))
            })
                GameObject.Find(name).transform.position = position;
        }
    }
}
