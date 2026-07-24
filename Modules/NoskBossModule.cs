using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;
using ItemChanger.Extensions;
using ItemChanger.FsmStateActions;
using ItemChanger.Modules;
using System;

namespace GhostPlando.Modules {
    internal class NoskBossModule: Module {
        internal static GameObject titlePrefab;
        internal static GameObject titleFaderPrefab;
        /*private const string titleName = "Nosk Title";
        private const string titleFaderName = "Nosk Title Fader";*/
        internal static GameObject audioPlayer;
        private int[] phaseHpHits = [25, 50, 45, 45, 45, 45];

        public override void Initialize() {
            On.Language.Language.Get_string_string += LanguageGet;
            On.GameManager.OnNextLevelReady += ResetEncounter;
            On.PlayMakerFSM.OnEnable += EditNosk;
            On.GameManager.OnNextLevelReady += EditArena;
            On.PlayMakerFSM.OnEnable += RemoveBossTitle;
        }

        public override void Unload() {
            On.Language.Language.Get_string_string -= LanguageGet;
            On.GameManager.OnNextLevelReady -= ResetEncounter;
            On.PlayMakerFSM.OnEnable -= EditNosk;
            On.GameManager.OnNextLevelReady -= EditArena;
            On.PlayMakerFSM.OnEnable -= RemoveBossTitle;
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
            if(self.sceneName != "Deepnest_32") {
                BossAttacks.BrokenVessel.JarHook?.Dispose();
            }
        }

        private void EditNosk(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self) {
            orig(self);
            if(self.gameObject.scene.name != "Deepnest_32" || self.gameObject.name != "Mimic Spider" || self.FsmName != "Mimic Spider")
                return;

            //full title card
            {
                GameObject title = GameObject.Instantiate(titlePrefab);
                GameObject fader = GameObject.Instantiate(titleFaderPrefab);
                foreach(TextMeshPro tmp in title.transform.GetComponentsInChildren<TextMeshPro>()) {
                    FsmVariables fv = tmp.GetComponent<PlayMakerFSM>().FsmVariables;
                    fv.GetFsmColor("Up Colour").Value = new Color(1, 0.4f, 0, 1);
                    fv.GetFsmColor("Down Colour").Value = new Color(1, 0.4f, 0, 0);
                    fv.GetFsmColor("Start Colour").Value = new Color(1, 0.4f, 0, 0);
                }
                self.AddFsmGameObject("Nosk Title", title);
                self.AddFsmGameObject("Nosk Title Fader", fader);
                FsmState roarEnd = self.GetState("Roar End");
                FsmState trans1 = self.GetState("Trans 1");
                roarEnd.AddLastActions(FsmEdits.RoarEndActions(self, title, fader, trans1));
                /*new List<FsmStateAction> {
                    new Wait {
                        time = 2.5f,
                        finishEvent = FsmEvent.Finished,
                        realTime = false
                    },
                    new Lambda(() => {
                        Vector3 titlePosition = GameCameras.instance.mainCamera.transform.position + new Vector3(-0.4479f, 4.8225f, 0);
                        title.transform.position = new Vector3(titlePosition.x, titlePosition.y, -1.695f);
                        fader.transform.position = title.transform.position + new Vector3(0.5f, -2.67f, -8.93f);
                        fader.transform.localScale = new Vector3(37.6446f, 26.62126f, 1);
                    }),
                    new ActivateGameObject {
                        gameObject = self.FsmGOToOwner("Nosk Title Fader"),
                        activate = true,
                        recursive = false,
                        resetOnExit = false,
                        everyFrame = false
                    },
                    new ActivateGameObject {
                        gameObject = self.FsmGOToOwner("Nosk Title"),
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
                }.ForEach(action => roarEnd.AddLastAction(action));*/
                trans1.GetFirstActionOfType<TransitionToAudioSnapshot>().Enabled = false;
                trans1.GetFirstActionOfType<ApplyMusicCue>().Enabled = false;
                self.GetState("Roar Init").AddFirstActions(FsmEdits.RoarInitActions(self));
                /*roarInit.AddFirstAction(new SendEventByName {
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
                    finishEvent = FsmEvent.Finished,
                    realTime = false,
                    stopOnExit = true,
                    loopDontFinish = false
                }, 1);*/
            }

            //int hp = phaseHpHits[5] * 5;
            //self.AddFsmInt("Phase 5-6 HP", hp); This should be on the Winged Nosk object, not here
            //hp += phaseHpHits[4] * 5;
            //start Winged Nosk with {hp} health
            string debugPhases;
            int hp = phaseHpHits[3] * 5;
            debugPhases = $"phase 4 starts at {hp}";
            self.AddFsmInt("Phase 3-4 HP", hp);
            hp += phaseHpHits[2] * 5;
            debugPhases = $"phase 3 starts at {hp}, {debugPhases}";
            self.AddFsmInt("Phase 2-3 HP", hp);
            hp += phaseHpHits[1] * 5;
            debugPhases = $"phase 2 starts at {hp}, {debugPhases}";
            self.AddFsmInt("Phase 1-2 HP", hp);
            hp += phaseHpHits[0] * 5;
            Modding.Logger.Log($"Nosk health starts at {hp}, {debugPhases}");
            self.gameObject.GetComponent<HealthManager>().hp = hp;

            audioPlayer = self.GetState("Trans 1").GetFirstActionOfType<AudioPlayerOneShotSingle>().audioPlayer.Value;

            BossAttacks.FalseKnight.Hook(self);
            BossAttacks.HornetP.Hook(self);
            BossAttacks.SoulMaster.Hook(self);
            BossAttacks.Gorb.Hook(self);
            BossAttacks.Uumuu.Hook(self);
            BossAttacks.Sheo.Hook(self);
            BossAttacks.Collector.Hook(self);
            BossAttacks.BrokenVessel.Hook(self);
            BossAttacks.WDefender.Hook(self);
            BossAttacks.MantisLords.Hook(self);
            BossAttacks.ElderHu.Hook(self);
            BossAttacks.Grimm.Hook(self);
            BossAttacks.PureVessel.Hook(self);
            BossAttacks.CGuardian.Hook(self);
            BossAttacks.HollowKnight.Hook(self);
            BossAttacks.TraitorLord.Hook(self);

            //mostly irrelevant tweaks to vanilla behaviour
            self.GetState("Spit Antic").GetFirstActionOfType<SetPosition>().y.Value = 4.84f;
            self.GetState("Roof Jump?").GetFirstActionOfType<IntCompare>().integer2 = self.FsmVariables.GetFsmInt("Phase 1-2 HP");
            self.GetState("Rising").GetFirstActionOfType<GetVelocity2d>().Enabled = true;
            self.GetState("Idle").AddFirstAction(new GetHP { target = self.FsmGOToOwner("Self"), storeValue = self.FsmVariables.GetFsmInt("HP") });

            //nonspecific supplemental edits
            GameObject zeroOrigin = new();
            zeroOrigin.transform.position = Vector3.zero;
            self.AddFsmGameObject("Zero Origin", zeroOrigin);
            self.AddFsmGameObject("Empty Store", null);
            self.AddFsmFloat("Wait After Falling", 0);

            //phase decision for ceiling crawling
            FsmState roofDecisionState = self.AddState("Roof Decision");
            FsmState roofAnticState = self.GetState("Roof Antic");
            FsmState roof2 = self.AddState("Roof Phase 2");
            FsmState roof3 = self.AddState("Roof Phase 3");
            FsmState roof4a = self.AddState("Roof Phase 4a");
            FsmState roof4b = self.AddState("Roof Phase 4b");
            FsmState roof4c = self.AddState("Roof Phase 4c");
            FsmState roof4d = self.AddState("Roof Phase 4d");
            FsmState roof4e = self.AddState("Roof Phase 4e");
            roofAnticState.RemoveTransitionsTo("Roof Drop");
            roofAnticState.AddTransition("FINISHED", roofDecisionState);
            roofDecisionState.AddTransition("ONE", "Roof Drop");
            roofDecisionState.AddTransition("TWO", roof2);
            roof2.AddTransition("FINISHED", "Roof Drop");
            roofDecisionState.AddTransition("THREE", roof3);
            roof3.AddTransition("FINISHED", "Fall Antic");
            roofDecisionState.AddTransition("FOUR", roof4a);
            roof4a.AddTransition("FINISHED", roof4b);
            roof4b.AddTransition("FINISHED", roof4c);
            roof4c.AddTransition("FINISHED", roof4c);
            roof4c.AddTransition("END PILLAR GEN", roof4d);
            roof4d.AddTransition("FINISHED", roof4e);
            roof4e.AddTransition("FINISHED", "Fall Antic");

            roofDecisionState.Actions = FsmEdits.PhaseDecisionActions(self);
            roof2.AddFirstActions(BossAttacks.FalseKnight.RunBarrels(self));
            roof3.AddFirstActions(BossAttacks.Collector.Run(self));
            self.GetState("Fall Antic").AddFirstActions(BossAttacks.Collector.Stop(self));
            roof4a.AddFirstActions(BossAttacks.Grimm.RunSpikes(self));
            roof4b.AddFirstActions(BossAttacks.PureVessel.RunSpikes(self));
            roof4b.AddLastActions(BossAttacks.HollowKnight.SetupPillars(self));
            roof4c.AddFirstActions(BossAttacks.HollowKnight.GenPillars(self));
            roof4d.AddFirstAction(new Wait { time = 0.6f, finishEvent = FsmEvent.Finished, realTime = false });
            roof4e.AddFirstActions(BossAttacks.HollowKnight.TriggerPillars(self));
            roof4e.AddLastActions(BossAttacks.CGuardian.RunCeilingLasers(self));
            roof4e.AddLastActions(BossAttacks.PureVessel.RunFocus(self));

            //action selection when falling
            self.AddFsmBool("Falling from Ceiling", false);
            self.GetState("To Fall").AddFirstAction(new SetBoolValue {
                boolVariable = self.FsmVariables.GetFsmBool("Falling from Ceiling"),
                boolValue = true,
                everyFrame = false
            });
            FsmState fallingState = self.GetState("Falling");
            FsmState fallenDecisionState = self.AddState("Fallen Decision");
            FsmState fallen2 = self.AddState("Fallen Phase 2");
            FsmState fallen2Hornet = self.AddState("Fallen Phase 2h");
            FsmState fallen2Master = self.AddState("Fallen Phase 2sm");
            FsmState fallen3 = self.AddState("Fallen Phase 3");
            FsmState fallen4 = self.AddState("Fallen Phase 4");
            FsmState land2State = self.GetState("Land 2");
            FsmState land3State = self.AddState("Land 3");
            fallingState.RemoveTransitionsTo("Land 2");
            fallingState.AddTransition("LAND", fallenDecisionState);
            fallenDecisionState.AddTransition("TWO", fallen2);
            fallen2.AddTransition("NEEDLE", fallen2Hornet);
            fallen2.AddTransition("QUAKE", fallen2Master);
            fallen2Hornet.AddTransition("FINISHED", land2State);
            fallen2Master.AddTransition("FINISHED", land2State);
            fallenDecisionState.AddTransition("THREE", fallen3);
            fallen3.AddTransition("FINISHED", land2State);
            fallenDecisionState.AddTransition("FOUR", fallen4);
            fallen4.AddTransition("FINISHED", land2State);
            fallenDecisionState.AddTransition("ONE", land2State);
            land2State.RemoveTransitionsTo("Roof Jump?");
            land2State.AddTransition("FINISHED", land3State);
            land3State.AddTransition("FINISHED", "Roof Jump?");

            fallenDecisionState.Actions = FsmEdits.PhaseDecisionActions(self);
            fallen2.AddFirstAction(new BoolTest {
                boolVariable = self.FsmVariables.GetFsmBool("Falling from Ceiling"),
                isTrue = FsmEvent.GetFsmEvent("QUAKE"),
                isFalse = FsmEvent.GetFsmEvent("NEEDLE"),
                everyFrame = false
            });
            fallen2Hornet.Actions = BossAttacks.HornetP.RunNeedle(self);
            fallen2Master.Actions = BossAttacks.SoulMaster.RunQuake(self);
            fallen3.Actions = BossAttacks.WDefender.RunShockwave(self);
            fallen4.Actions = BossAttacks.TraitorLord.RunShockwave(self);
            fallen4.AddLastActions(BossAttacks.WDefender.RunFountain(self));
            land2State.AddFirstActions(FsmEdits.RevertCeilingFall(self));
            land3State.Actions = FsmEdits.AfterFallActions(self);
            self.GetState("Roof Jump?").AddFirstActions(FsmEdits.ResetFallWait(self));

            //charging effects
            self.GetState("Charge Start").AddFirstActions(BossAttacks.Uumuu.RunSlowZap(self));
            self.GetState("Wait").AddFirstActions(BossAttacks.Uumuu.StopSlowZap(self));
            self.GetState("Charge R").AddFirstActions(BossAttacks.ElderHu.RunRight(self));
            self.GetState("Charge L").AddFirstActions(BossAttacks.ElderHu.RunLeft(self));

            //jump effects
            FsmState launchState = self.GetState("Launch");
            FsmState preLaunchState = self.AddState("PreLaunch");
            FsmState jumpAnticState = self.GetState("Jump Antic");
            FsmState risingState = self.GetState("Rising");
            FsmState hoverState = self.AddState("Hovering");
            jumpAnticState.RemoveTransitionsTo("Launch");
            jumpAnticState.AddTransition("FINISHED", preLaunchState);
            preLaunchState.AddTransition("FINISHED", launchState);
            risingState.RemoveTransitionsTo("Falling");
            risingState.AddTransition("FINISHED", hoverState);
            hoverState.AddTransition("FINISHED", fallingState);

            preLaunchState.Actions = BossAttacks.Sheo.RunBluePaint(self);
            launchState.AddFirstActions(FsmEdits.ReduceJumpForce(self));
            hoverState.Actions = FsmEdits.HoveringActions(self);
            fallingState.AddFirstAction(new SetGravity2dScale { gameObject = self.FsmGOToOwner("Self"), gravityScale = 2 });

            //decision when spitting
            FsmState spitAnticState = self.GetState("Spit Antic");
            FsmState spit1State = self.GetState("Spit 1");
            FsmState spitDecisionState = self.AddState("Spit Decision");
            FsmState gorbState = self.AddState("Gorb Spit");
            FsmState mantisState = self.AddState("Mantis Spit");
            FsmState firebat1 = self.AddState("Firebat 1 ");
            spitAnticState.RemoveTransitionsTo("Spit 1");
            spitAnticState.AddTransition("FINISHED", spitDecisionState);
            spitDecisionState.AddTransition("ONE", spit1State);
            spitDecisionState.AddTransition("TWO", gorbState);
            spitDecisionState.AddTransition("THREE", mantisState);
            spitDecisionState.AddTransition("FOUR", firebat1);
            gorbState.AddTransition("FINISHED", spit1State);
            mantisState.AddTransition("FINISHED", spit1State);
            spitDecisionState.Actions = FsmEdits.PhaseDecisionActions(self);
            gorbState.Actions = BossAttacks.Gorb.RunSpears(self);
            mantisState.Actions = BossAttacks.MantisLords.Run(self);
            firebat1.Actions = BossAttacks.Grimm.RunFirebats(self, 0.3f, 1, -2, false);
            firebat1.ChainFsmStates([
                ("Firebat 2", BossAttacks.Grimm.RunFirebats(self, 0.3f, 1.1f, 0, false)),
                ("Firebat 3", BossAttacks.Grimm.RunFirebats(self, 0.3f, 1.2f, -2, true).Concat(BossAttacks.PureVessel.RunTendrilAntic(self)).ToArray()),
                ("PV Tendril Burst", BossAttacks.PureVessel.RunTendrilBurst(self)),
                ("PV Tendril Start", BossAttacks.PureVessel.RunTendrilStart(self)),
                ("PV Tendril Hit", BossAttacks.PureVessel.RunTendrilHit(self)),
                ("PV Tendril T2", BossAttacks.PureVessel.RunTendrilT2(self)),
                ("PV Tendril T3", BossAttacks.PureVessel.RunTendrilT3(self)),
                ("PV Tendril Hit End", BossAttacks.PureVessel.RunTendrilHitEnd(self)),
                ("PV Tendril Recover", BossAttacks.PureVessel.RunTendrilRecover(self))
            ]).AddTransition("FINISHED", "Spit Recover");
            //wait fuck it isn't that simple, phase 2 and 3 attach to Spit 1 but phase 4 deviates from Spit Antic



            //testing gorb spears
            //self.GetState("Spit 1").AddFirstActions(BossAttacks.Gorb.RunSpears(self));

            //testing mantis lords discs
            //self.GetState("Spit 1").AddFirstActions(BossAttacks.MantisLords.Run(self));

            //testing grimm firebats and pure vessel void tendrils
            //FsmState firebat1 = self.AddState("Firebat 1");
            /*FsmState spitAnticState = self.GetState("Spit Antic");
            spitAnticState.RemoveTransitionsTo("Spit 1");
            //spitAnticState.AddTransition("FINISHED", firebat1);
            //firebat1.Actions = BossAttacks.Grimm.RunFirebats(self, 0.3f, 1, -2, false);
            //firebat2.Actions = BossAttacks.Grimm.RunFirebats(self, 0.3f, 1.1f, false);
            //firebat3.Actions = BossAttacks.Grimm.RunFirebats(self, 0.6f, 1.2f, true);
            //firebat3.AddLastActions(BossAttacks.PureVessel.RunTendrilAntic(self));

            spitAnticState.ChainFsmStates([
                ("Firebat 1", BossAttacks.Grimm.RunFirebats(self, 0.3f, 1, -2, false)),
                ("Firebat 2", BossAttacks.Grimm.RunFirebats(self, 0.3f, 1.1f, 0, false)),
                ("Firebat 3", BossAttacks.Grimm.RunFirebats(self, 0.3f, 1.2f, -2, true).Concat(BossAttacks.PureVessel.RunTendrilAntic(self)).ToArray()),
                ("PV Tendril Burst", BossAttacks.PureVessel.RunTendrilBurst(self)),
                ("PV Tendril Start", BossAttacks.PureVessel.RunTendrilStart(self)),
                ("PV Tendril Hit", BossAttacks.PureVessel.RunTendrilHit(self)),
                ("PV Tendril T2", BossAttacks.PureVessel.RunTendrilT2(self)),
                ("PV Tendril T3", BossAttacks.PureVessel.RunTendrilT3(self)),
                ("PV Tendril Hit End", BossAttacks.PureVessel.RunTendrilHitEnd(self)),
                ("PV Tendril Recover", BossAttacks.PureVessel.RunTendrilRecover(self))
            ]).AddTransition("FINISHED", "Spit Recover");*/
        }

        private void EditArena(On.GameManager.orig_OnNextLevelReady orig, GameManager self) {
            orig(self);
            if(self.sceneName != "Deepnest_32")
                return;
            
            GameObject chunk2 = GameObject.Find("Chunk 0 2");
            chunk2.GetComponents<EdgeCollider2D>()[0].points = GhostFsmUtil.AssignFromTuples2((0, 1), (0, 0), (33, 0), (33, 3), (0, 3), (0, 1));
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
            chunk3.GetComponents<EdgeCollider2D>()[0].points = GhostFsmUtil.AssignFromTuples2((-1, 1), (-1, 0), (32, 0), (32, 4), (20.8f, 4), (20.8f, 3), (-1, 3), (-1, 1));
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

        private void RemoveBossTitle(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self) {
            orig(self);
            if(self.FsmName == "Area Title Control") {
                FsmState visitedCheck = self.GetState("Visited Check");
                visitedCheck.AddTransition("SKIP", "Done");
                visitedCheck.AddFirstAction(new Lambda(() => {
                    if(GameManager.instance.sceneName == "Deepnest_32") {
                        self.SendEvent("SKIP");
                    }
                }));
            }
        }
    }
}
