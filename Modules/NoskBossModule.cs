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
        public bool EncounteredWingedNosk;
        private bool FirstEncounter = true;
        private int strengthMultiplier => PlayerData.instance.GetBool(nameof(PlayerData.equippedCharm_25)) ? 6 : 5;

        internal static GameObject titlePrefab;
        internal static GameObject titleFaderPrefab;
        internal static GameObject audioPlayer;
        internal static GameObject wingedNoskPrefab;
        private int[] phaseHpHits = [25, 50, 45, 45, 35, 35];

        public override void Initialize() {
            On.Language.Language.Get_string_string += LanguageGet;
            On.GameManager.OnNextLevelReady += ResetEncounter;
            On.PlayMakerFSM.OnEnable += EditNosk;
            On.GameManager.OnNextLevelReady += EditArena;
            On.PlayMakerFSM.OnEnable += RemoveBossTitle;
            On.PlayMakerFSM.OnEnable += EditWingedNosk;
            On.PlayMakerFSM.OnEnable += EditWoskCorpse;
        }

        public override void Unload() {
            On.Language.Language.Get_string_string -= LanguageGet;
            On.GameManager.OnNextLevelReady -= ResetEncounter;
            On.PlayMakerFSM.OnEnable -= EditNosk;
            On.GameManager.OnNextLevelReady -= EditArena;
            On.PlayMakerFSM.OnEnable -= RemoveBossTitle;
            On.PlayMakerFSM.OnEnable -= EditWingedNosk;
            On.PlayMakerFSM.OnEnable -= EditWoskCorpse;
            DisableInternalHooks();
        }

        private void DisableInternalHooks() {
            BossAttacks.BrokenVessel.JarHook?.Dispose();
            On.PlayMakerFSM.OnEnable -= BossAttacks.SoulMaster.ReduceSpinnerRange;
            On.PlayMakerFSM.OnEnable -= BossAttacks.HiveKnight.EditBeeDroppers;
            On.PlayMakerFSM.OnEnable -= BossAttacks.Flukemarm.EditFlukeSpawner;
            On.PlayMakerFSM.OnEnable -= BossAttacks.GPZote.EditZotelings;
            On.PlayMakerFSM.OnEnable -= BossAttacks.Galien.EditHammer;
        }

        private string LanguageGet(On.Language.Language.orig_Get_string_string orig, string key, string sheetTitle) {
            if(GameManager.instance.sceneName == "Deepnest_32") {
                if(sheetTitle == "Titles") {
                    if(key == "NIGHTMARE_GRIMM_MAIN")
                        return "Nosk";
                    if(key == "NIGHTMARE_GRIMM_SUPER")
                        return "Mimic Paragon";
                    if(key == "NIGHTMARE_GRIMM_SUB")
                        return "";
                }
            }
            return orig(key, sheetTitle);
        }

        private void ResetEncounter(On.GameManager.orig_OnNextLevelReady orig, GameManager self) {
            orig(self);
            PlayerData.instance.SetBool(nameof(PlayerData.encounteredMimicSpider), false);
            HeroController.instance.gameObject.LocateMyFSM("Spell Control").FsmVariables.GetFsmBool("Dream Focus").Value = false;
            if(self.sceneName != "Deepnest_32") {
                DisableInternalHooks();
            }
        }

        private void EditNosk(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self) {
            orig(self);
            if(self.gameObject.scene.name != "Deepnest_32" || self.gameObject.name != "Mimic Spider" || self.FsmName != "Mimic Spider")
                return;

            //full title card
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
            trans1.GetFirstActionOfType<TransitionToAudioSnapshot>().Enabled = false;
            trans1.GetFirstActionOfType<ApplyMusicCue>().Enabled = false;
            self.GetState("Roar Init").AddFirstActions(FsmEdits.RoarInitActions(self));

            int hp = (phaseHpHits[5] + phaseHpHits[4]) * strengthMultiplier;
            self.AddFsmInt("Phase 4-5 HP", hp);
            hp += phaseHpHits[3] * strengthMultiplier;
            self.AddFsmInt("Phase 3-4 HP", hp);
            hp += phaseHpHits[2] * strengthMultiplier;
            self.AddFsmInt("Phase 2-3 HP", hp);
            hp += phaseHpHits[1] * strengthMultiplier;
            self.AddFsmInt("Phase 1-2 HP", hp);
            hp += phaseHpHits[0] * strengthMultiplier;
            self.gameObject.GetComponent<HealthManager>().hp = hp;
            CheckPhaseChange.SetupThresholds(phaseHpHits, strengthMultiplier);
            self.AddFsmInt("Current Boss Phase", 1);

            if(EncounteredWingedNosk)
                self.gameObject.GetComponent<HealthManager>().hp = self.FsmVariables.GetFsmInt("Phase 4-5 HP").Value;

            audioPlayer = self.GetState("Trans 1").GetFirstActionOfType<AudioPlayerOneShotSingle>().audioPlayer.Value;

            GameObject wosk = GameObject.Instantiate(wingedNoskPrefab, new Vector3(100, 25, wingedNoskPrefab.transform.position.z), Quaternion.identity);
            self.AddFsmGameObject("Winged Nosk", wosk);

            BossAttacks.FalseKnight.Hook(self);
            BossAttacks.HornetP.Hook(self);
            BossAttacks.SoulMaster.HookNosk(self);
            BossAttacks.Gorb.Hook(self);
            BossAttacks.Uumuu.HookNosk(self);
            BossAttacks.Sheo.Hook(self);
            BossAttacks.Collector.Hook(self);
            BossAttacks.BrokenVessel.Hook(self);
            BossAttacks.WDefender.Hook(self);
            BossAttacks.MantisLords.Hook(self);
            BossAttacks.ElderHu.Hook(self);
            BossAttacks.Grimm.HookNosk(self);
            BossAttacks.PureVessel.Hook(self);
            BossAttacks.CGuardian.Hook(self);
            BossAttacks.HollowKnight.Hook(self);
            BossAttacks.TraitorLord.HookNosk(self);

            //force first attack after phase threshold
            self.GetState("Idle").AddTransition("PHASE ROOF", "RS Jump Antic");

            //mostly irrelevant tweaks to vanilla behaviour
            self.GetState("Spit Antic").GetFirstActionOfType<SetPosition>().y.Value = 4.84f;
            self.GetState("Roof Jump?").GetFirstActionOfType<IntCompare>().integer2 = self.FsmVariables.GetFsmInt("Phase 1-2 HP");
            self.GetState("Rising").GetFirstActionOfType<GetVelocity2d>().Enabled = true;
            self.GetState("Idle").AddFirstActions(FsmEdits.IdleActions(self));

            //speed up returning intro
            if(!FirstEncounter) {
                for(int i = 1; i < 5; i++) {
                    self.GetState($"Trans {i}").GetFirstActionOfType<Wait>().time = 1f;
                }
            }
            roarEnd.AddFirstAction(new Lambda(() => FirstEncounter = false));

            //nonspecific supplemental edits
            GameObject zeroOrigin = new("Zero Origin");
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
            fallen4.Actions = [.. BossAttacks.TraitorLord.RunShockwave(self), .. BossAttacks.WDefender.RunFountain(self)];
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
            firebat1.Actions = BossAttacks.Grimm.RunFirebats(self, 1, -2, false);
            firebat1.ChainFsmStates("FINISHED",
                ("Firebat 2", BossAttacks.Grimm.RunFirebats(self, 1.1f, 1, false)),
                ("Firebat 3", BossAttacks.Grimm.RunFirebats(self, 1.2f, -2, true).Concat(BossAttacks.PureVessel.RunTendrilAntic(self)).ToArray()),
                ("PV Tendril Burst", BossAttacks.PureVessel.RunTendrilBurst(self)),
                ("PV Tendril Start", BossAttacks.PureVessel.RunTendrilStart(self)),
                ("PV Tendril Hit", BossAttacks.PureVessel.RunTendrilHit(self)),
                ("PV Tendril T2", BossAttacks.PureVessel.RunTendrilT2(self)),
                ("PV Tendril T3", BossAttacks.PureVessel.RunTendrilT3(self)),
                ("PV Tendril Hit End", BossAttacks.PureVessel.RunTendrilHitEnd(self)),
                ("PV Tendril Recover", BossAttacks.PureVessel.RunTendrilRecover(self))
            ).AddTransition("FINISHED", "Spit Recover");

            //transition to winged form
            FsmState mutateAntic = self.AddState("Mutate Antic");
            FsmState mutateJump = self.AddState("Mutate Jump");
            FsmState mutateWait = self.AddState("Mutate Wait");
            FsmState mutateSwitch = self.AddState("Mutate Switch");
            self.GetState("Idle").AddTransition("WINGIFY", mutateAntic);
            mutateAntic.AddTransition("FINISHED", mutateJump);
            mutateJump.AddTransition("LAND", mutateWait);
            mutateWait.AddTransition("FINISHED", mutateSwitch);

            mutateAntic.Actions = FsmEdits.MutateAnticActions(self);
            mutateJump.Actions = FsmEdits.MutateJumpActions(self);
            mutateWait.Actions = FsmEdits.MutateWaitActions(self);
            mutateSwitch.Actions = FsmEdits.MutateSwitchActions(self, self.FsmVariables.GetFsmInt("Phase 4-5 HP").Value);
        }

        private void EditArena(On.GameManager.orig_OnNextLevelReady orig, GameManager self) {
            orig(self);
            if(self.sceneName != "Deepnest_32")
                return;
            
            GameObject chunk2 = GameObject.Find("Chunk 0 2");
            chunk2.GetComponents<EdgeCollider2D>()[0].points = GhostFsmUtil.AssignFromTuples2((0, 1), (0, 0), (33, 0), (33, 3), (0, 3), (0, 1));
            chunk2.GetComponents<EdgeCollider2D>()[1].points = GhostFsmUtil.AssignFromTuples2((0, 8), (0, 7), (7, 7), (7, 26), (0, 26), (0, 8));
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
            chunk3.GetComponents<EdgeCollider2D>()[1].points = GhostFsmUtil.AssignFromTuples2((22, 26), (22, 8), (32, 8), (32, 26), (22, 26));
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

        private void EditWingedNosk(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self) {
            orig(self);
            if(self.gameObject.scene.name != "Deepnest_32" || !self.gameObject.name.StartsWith("Hornet Nosk") || self.FsmName != "Hornet Nosk")
                return;

            BossAttacks.EGuardian.Hook(self);
            BossAttacks.Markoth.Hook(self);
            BossAttacks.SoulMaster.HookWosk(self);
            BossAttacks.HornetS.Hook(self);
            BossAttacks.HiveKnight.Hook(self);
            BossAttacks.Uumuu.HookWosk(self);
            BossAttacks.NightmareKing.Hook(self);
            BossAttacks.TraitorLord.HookWosk(self);
            BossAttacks.Grimm.HookWosk(self);
            BossAttacks.NoEyes.Hook(self);
            BossAttacks.VengeflyKing.Hook(self);
            BossAttacks.Flukemarm.Hook(self);
            BossAttacks.SoulWarrior.Hook(self);
            BossAttacks.GPZote.Hook(self);
            BossAttacks.Galien.Hook(self);

            self.AddFsmGameObject("Empty Store", null);
            self.AddFsmInt("Phase 5-6 HP", phaseHpHits[5] * strengthMultiplier);
            self.AddFsmInt("Current Boss Phase", 5);

            //generic/boundary edits
            self.GetState("Init").GetFirstActionOfType<GetParent>().Enabled = false;
            FsmState setPosState = self.GetState("Set Pos");
            SetPosition sp = setPosState.GetFirstActionOfType<SetPosition>();
            sp.x.Value = 100;
            sp.y.Value = 22.5f;
            setPosState.GetFirstActionOfType<SetFsmGameObject>().Enabled = false;
            FsmState hpState = self.GetState("HP");
            hpState.RemoveTransitionsOn("FINISHED");
            hpState.AddTransition("FINISHED", "Set Pos");
            FsmVariables fv = self.FsmVariables;
            fv.GetFsmFloat("Swoop Height").Value = 5.9f;
            fv.GetFsmFloat("X Min").Value = 80f;
            fv.GetFsmFloat("X Max").Value = 110f;
            fv.GetFsmFloat("Y Min").Value = 7.65f;
            fv.GetFsmFloat("Y Max").Value = 10.65f;
            self.GetState("Swoop L").GetFirstActionOfType<FloatCompare>().float2.Value = 85f;
            self.GetState("Swoop R").GetFirstActionOfType<FloatCompare>().float2.Value = 106f;
            self.GetState("Shift Down?").GetFirstActionOfType<FloatCompare>().float2.Value = 10.42f;
            FsmState roofImpactState = self.GetState("Roof Impact");
            roofImpactState.GetFirstActionOfType<SetPosition>().y.Value = 25f;
            roofImpactState.AddFirstAction(FsmEdits.ZeroVelocity(self));
            self.GetState("Roof Return").GetFirstActionOfType<SetPosition>().y.Value = 24f;
            FsmState chooseAttackState = self.GetState("Choose Attack");
            chooseAttackState.AddTransition("ROOF", "Roof Antic");
            chooseAttackState.GetFirstActionOfType<IntCompare>().integer2 = fv.GetFsmInt("Phase 5-6 HP");
            SendRandomEventV3 randomEvent = chooseAttackState.GetFirstActionOfType<SendRandomEventV3>();
            randomEvent.events = new List<string> { "SWOOP", "ACID ROAR", "SUMMON", "ROOF" }.Select(s => FsmEvent.GetFsmEvent(s)).ToArray();
            randomEvent.weights = [1, 1, 1, 0.7f];
            randomEvent.trackingInts = new List<string> { "Ct Swoop", "Ct Acid", "Ct Summon", "Ct Roof" }.Select(s => fv.GetFsmInt(s)).ToArray();
            randomEvent.eventMax = [2, 1, 1, 1];
            randomEvent.trackingIntsMissed = new List<string> { "Ms Swoop", "Ms Acid", "Ms Summon", "Ms Roof" }.Select(s => fv.GetFsmInt(s)).ToArray();
            randomEvent.missedMax = [3, 4, 4, 5];
            SendRandomEventV3 chooseAttack2 = self.GetState("Choose Attack 2").GetFirstActionOfType<SendRandomEventV3>();
            chooseAttack2.weights[3].Value = 0.7f;
            chooseAttack2.missedMax[3].Value = 5;
            self.GetState("Summon Recover").GetFirstActionOfType<AudioPlayerOneShotSingle>().Enabled = false;

            //Phase 5-6 updates
            FsmState toChoose2State = self.AddState("To Choose 2");
            chooseAttackState.RemoveTransitionsOn("HALF");
            chooseAttackState.AddTransition("HALF", toChoose2State);
            toChoose2State.AddTransition("FINISHED", "Choose Attack 2");
            self.AddFsmInt("Summon Quantity", 2);
            toChoose2State.Actions = FsmEdits.UpdateSpinners(self);
            self.GetState("Choose Attack 2").AddFirstAction(new CheckPhaseChange { ChangeEvent = FsmEvent.GetFsmEvent("ROOF") });

            //entrance and intermediate phase
            FsmState initVelState = self.GetState("Init Velocity");
            initVelState.GetFirstActionOfType<SetVelocity2d>().vector.Value = new Vector2(0, -32);
            initVelState.GetFirstActionOfType<Wait>().time.Value = 2;
            initVelState.GetFirstActionOfType<DecelerateXY>().decelerationY.Value = 0.95f;

            FsmState thkSprayState = self.AddState("THK Spray");
            FsmState thkLoopState = self.AddState("THK Spray Loop");
            FsmState thkPrepState = self.AddState("THK Spray Prep");
            FsmState smSpinnerState = self.AddState("Start SM Spinner");
            initVelState.RemoveTransitionsOn("FINISHED");
            initVelState.AddTransition("FINISHED", thkPrepState);
            thkPrepState.AddTransition("FINISHED", thkSprayState);
            thkSprayState.AddTransition("LOOP", thkLoopState);
            thkLoopState.AddTransition("FINISHED", thkSprayState);
            thkSprayState.AddTransition("END", smSpinnerState);
            smSpinnerState.AddTransition("FINISHED", "Idle");

            self.AddFsmFloat("THK Chest Shot Time", 0);
            thkPrepState.Actions = BossAttacks.HollowKnight.PrepSpray(self, this);
            thkSprayState.Actions = BossAttacks.HollowKnight.RunSpray(self);
            smSpinnerState.Actions = BossAttacks.SoulMaster.RunSpinner(self);

            //swoop attacks
            FsmState swoopAnticState = self.GetState("Swoop Antic");
            FsmState swoopStartState = self.AddState("Swoop Start");
            swoopAnticState.RemoveTransitionsTo("Swoop Direction");
            swoopAnticState.AddTransition("FINISHED", swoopStartState);
            swoopStartState.AddTransition("FINISHED", "Swoop Direction");
            swoopStartState.Actions = FsmEdits.SwoopStartActions(self);

            FsmState swoopFireChoiceL = self.AddState("Swoop L Fire?");
            FsmState swoopFireChoiceR = self.AddState("Swoop R Fire?");
            FsmState swoopFireL = self.AddState("Swoop Fire L");
            FsmState swoopFireR = self.AddState("Swoop Fire R");
            FsmState swoopDirectionState = self.GetState("Swoop Direction");
            swoopDirectionState.RemoveTransitionsOn("LEFT");
            swoopDirectionState.RemoveTransitionsOn("RIGHT");
            swoopDirectionState.AddTransition("LEFT", swoopFireChoiceL);
            swoopDirectionState.AddTransition("RIGHT", swoopFireChoiceR);
            swoopFireChoiceL.AddTransition("WITH", swoopFireL);
            swoopFireChoiceL.AddTransition("WITHOUT", "Swoop L");
            swoopFireChoiceR.AddTransition("WITH", swoopFireR);
            swoopFireChoiceR.AddTransition("WITHOUT", "Swoop R");
            swoopFireL.AddTransition("FINISHED", "Swoop Rise");
            swoopFireR.AddTransition("FINISHED", "Swoop Rise");
            swoopFireChoiceL.Actions = FsmEdits.DoFireSwoop(self);
            swoopFireChoiceR.Actions = FsmEdits.DoFireSwoop(self);
            swoopFireL.Actions = [.. BossAttacks.NightmareKing.RunFlameTrail(self), .. self.GetState("Swoop L").Actions];
            swoopFireR.Actions = [.. BossAttacks.NightmareKing.RunFlameTrail(self), .. self.GetState("Swoop R").Actions];
            self.GetState("Swoop Rise").AddFirstActions(BossAttacks.HiveKnight.EndGlobarbs(self));

            //spit attacks
            FsmState acidRoarAnticState = self.GetState("Acid Roar Antic");
            FsmState spitChooseState = self.AddState("Acid Roar Choice");
            FsmState spitZapState = self.AddState("Acid Roar Zap");
            FsmState sickleOrbStartState = self.AddState("Sickle Orb Start");
            FsmState sickleOrbFireState = self.AddState("Sickle Orb Fire");
            FsmState sickleOrbLoopState = self.AddState("Sickle Orb Loop");
            FsmState sickleOrbDeclineState = self.AddState("Sickle Orb Decline");
            acidRoarAnticState.RemoveTransitionsTo("Spit 1");
            acidRoarAnticState.AddTransition("FINISHED", spitChooseState);
            spitChooseState.AddTransition("FIVE", spitZapState);
            spitChooseState.AddTransition("SIX", sickleOrbStartState);
            spitZapState.AddTransition("FINISHED", "Spit 1");
            sickleOrbStartState.AddTransition("FINISHED", sickleOrbFireState);
            sickleOrbFireState.AddTransition("LOOP", sickleOrbLoopState);
            sickleOrbFireState.AddTransition("FINISHED", sickleOrbDeclineState);
            sickleOrbDeclineState.AddTransition("FINISHED", "Acid Roar End");
            sickleOrbLoopState.AddTransition("FINISHED", sickleOrbFireState);

            spitChooseState.Actions = [FsmEdits.PhaseDecisionActions2(self)];
            spitZapState.Actions = [.. BossAttacks.Uumuu.RunFastZap(self), new Wait { time = 0.3f, finishEvent = FsmEvent.Finished, realTime = false }];
            sickleOrbStartState.Actions = [.. BossAttacks.TraitorLord.RunSickles(self), .. BossAttacks.SoulMaster.PrepOrb(self)];
            sickleOrbFireState.Actions = BossAttacks.SoulMaster.FireOrb(self);
            sickleOrbLoopState.Actions = BossAttacks.SoulMaster.LoopOrb(self);
            sickleOrbDeclineState.Actions = [new Wait { time = 0.7f, finishEvent = FsmEvent.Finished, realTime = false }];

            //roof attacks
            FsmState globState = self.GetState("Globs");
            FsmState globs2State = self.AddState("Globs 2");
            FsmState ghostfireState = self.AddState("Ghosts and Fire");
            FsmState ghostfire2State = self.AddState("Ghosts and Fire 2");
            globState.RemoveTransitionsOn("FINISHED");
            globState.AddTransition("FINISHED", globs2State);
            globState.AddTransition("SIX", ghostfireState);
            globs2State.AddTransition("FINISHED", "Roof Return");
            ghostfireState.AddTransition("FINISHED", ghostfire2State);
            ghostfire2State.AddTransition("FINISHED", "Roof Return");
            globState.Actions = FsmEdits.GlobsActions(self);
            globs2State.Actions = BossAttacks.Sheo.RunPurplePaint2(self);
            ghostfireState.Actions = [.. BossAttacks.NoEyes.RunGhosts(self, true), .. BossAttacks.Grimm.RunFireballsMulti(self, true)];
            ghostfire2State.Actions = [.. BossAttacks.NoEyes.RunGhosts(self, false), .. BossAttacks.Grimm.RunFireballsMulti(self, false)];

            //summon attacks
            FsmState summonRoarState = self.GetState("Summon Roar");
            summonRoarState.GetFirstActionOfType<SetIntValue>().intValue = fv.GetFsmInt("Summon Quantity");
            FsmState summonState = self.GetState("Summon");
            GameObject infectedBalloon = summonState.GetFirstActionOfType<SpawnObjectFromGlobalPool>().gameObject.Value;
            infectedBalloon.Spawn().Recycle();
            summonState.GetFirstActionOfType<SpawnObjectFromGlobalPool>().Enabled = false;
            summonState.GetFirstActionOfType<EnemyPusherIgnore>().Enabled = false;
            FsmState summonBalloon = self.AddState("Summon Infected Balloon");
            FsmState summonVengefly = self.AddState("Summon Vengefly");
            FsmState summonFlukefey = self.AddState("Summon Flukefey");
            FsmState summonFolly = self.AddState("Summon Folly");
            FsmState summonZoteling = self.AddState("Summon Zoteling");
            summonState.RemoveTransitionsTo("Resummon");
            foreach((string transition, FsmState sstate) in new (string, FsmState)[] { ("Balloon", summonBalloon),
                    ("Vengefly", summonVengefly), ("Fluke", summonFlukefey), ("Folly", summonFolly), ("Zote", summonZoteling)}) {
                summonState.AddTransition(transition, sstate);
                sstate.AddTransition("FINISHED", "Resummon");
            }
            summonBalloon.Actions = FsmEdits.SummonBalloonActions(self, infectedBalloon);
            summonVengefly.Actions = BossAttacks.VengeflyKing.SummonVengefly(self);
            summonFlukefey.Actions = BossAttacks.Flukemarm.SummonFlukefey(self);
            summonFolly.Actions = BossAttacks.SoulWarrior.SummonFolly(self);
            summonZoteling.Actions = BossAttacks.GPZote.SummonZoteling(self);
            summonState.AddLastActions(FsmEdits.SummonChoice(self));

            self.GetState("Init").AddLastActions(BossAttacks.EGuardian.InitDirectLaser(self));
            summonRoarState.RemoveTransitionsTo("Summon");
            FsmState summonChooseState = self.AddState("Summon Choose");
            summonRoarState.AddTransition("FINISHED", summonChooseState);
            summonChooseState.Actions = [FsmEdits.PhaseDecisionActions2(self)];
            summonChooseState.ChainFsmStates("FIVE",
                ("CG Laser Prep", [.. BossAttacks.EGuardian.PrepDirectLaser(self), .. BossAttacks.Markoth.RunNails(self)]),
                ("CG Laser Fire", BossAttacks.EGuardian.FireDirectLaser(self)),
                ("CG Laser End", BossAttacks.EGuardian.EndDirectLaser(self))
            ).AddTransition("FINISHED", "Summon");
            summonChooseState.ChainFsmStates("SIX",
                ("Galien Hammer", BossAttacks.Galien.RaiseHammer(self))
            ).AddTransition("FINISHED", "Summon");
            FsmState recoverState = self.GetState("Summon Recover");
            recoverState.RemoveTransitionsOn("FINISHED");
            FsmState recoverChooseState = self.AddState("Summon Recover Choose");
            recoverState.AddTransition("FINISHED", recoverChooseState);
            recoverChooseState.Actions = [FsmEdits.PhaseDecisionActions2(self)];
            recoverChooseState.AddTransition("FIVE", "Idle");
            recoverChooseState.ChainFsmStates("SIX",
                ("Recover Pre Hammer", [new Wait { time = 1.5f, finishEvent = FsmEvent.Finished, realTime = false }]),
                ("Recover Hammer", BossAttacks.Galien.ThrowHammer(self))
            ).AddTransition("FINISHED", "Idle");
        }

        private void EditWoskCorpse(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self) {
            orig(self);
            if(GameManager.instance.sceneName == "Deepnest_32" && self.gameObject.name.StartsWith("Corpse Hornet Nosk")) {
                if(self.FsmName == "Corpse Control") {
                    self.GetState("Init").RemoveTransitionsTo("Blow");
                }
                else if(self.FsmName == "corpse") {
                    if(self.gameObject.TryGetComponent<ParagonDeathEffects>(out _))
                        return;
                    self.gameObject.AddComponent<ParagonDeathEffects>();
                    FsmState steam = self.GetState("Steam");
                    steam.RemoveTransitionsTo("Ready");
                    FsmState finalState = self.AddState("Final State");
                    steam.AddTransition("FINISHED", finalState);
                    finalState.Actions = FsmEdits.BossDeathActions(self);
                }
            }
        }
    }
}
