using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using UnityEngine;
using HutongGames.PlayMaker.Actions;
using ItemChanger;
using ItemChanger.Extensions;
using ItemChanger.Internal;
using ItemChanger.Util;
using LoreCore.Items;

namespace GhostPlando.Modules {
    internal class VanishedBossesModule: ItemChanger.Modules.Module {
        private string[] intBosses = [
            nameof(pd.noEyesDefeated),
            nameof(pd.galienDefeated),
            nameof(pd.markothDefeated),
            nameof(pd.xeroDefeated),
            nameof(pd.aladarSlugDefeated),
            nameof(pd.elderHuDefeated)
        ];
        private string[] boolBosses = [
            nameof(pd.zoteRescuedBuzzer),
            nameof(pd.falseKnightDefeated),
            nameof(pd.falseKnightDreamDefeated),
            nameof(pd.hornet1Defeated),
            nameof(pd.hornet_f19),
            nameof(pd.hornetFountainEncounter),
            nameof(pd.hornetOutskirtsDefeated),
            nameof(pd.hornetDenEncounter),
            nameof(pd.hornetAbyssEncounter),
            nameof(pd.megaMossChargerDefeated),
            nameof(pd.flukeMotherDefeated),
            nameof(pd.defeatedMantisLords),
            nameof(pd.killedInfectedKnight),
            nameof(pd.infectedKnightDreamDefeated),
            nameof(pd.collectorDefeated),
            nameof(pd.killedMegaBeamMiner),
            nameof(pd.defeatedMegaBeamMiner),
            nameof(pd.defeatedMegaJelly),
            nameof(pd.defeatedDungDefender),
            nameof(pd.killedGrimm)
        ];
        private (string, string)[] sceneDataBosses = [
            (SceneNames.Crossroads_04, "Battle Scene"),
            (SceneNames.Crossroads_09, "Mawlek Body"),
            (SceneNames.Crossroads_09, "Battle Scene"),
            (SceneNames.Mines_32, "Zombie Beam Miner Rematch"),
            (SceneNames.Mines_32, "Battle Scene"),
            (SceneNames.Ruins1_23, "Battle Scene v2"),
            (SceneNames.Ruins1_31, "Battle Scene v2"),
            (SceneNames.Ruins2_03, "Battle Control"),
            (SceneNames.Fungus1_19, "Hornet_Activate"),
            (SceneNames.Deepnest_East_12, "Hornet Encounter Outskirts")
        ];
        private Dictionary<string, List<string>> objectsToRemove = new() {
            { SceneNames.Fungus2_15, [ "Mantis Lord Throne 1", "Mantis Lord Throne 2", "Mantis Lord Throne 3" ] },
            { SceneNames.Deepnest_03, [ "Breakable Wall" ] },
            { SceneNames.Deepnest_31, [ "Breakable Wall", "Breakable Wall (1)" ] }
        };

        private PlayerData pd => PlayerData.instance;

        private ILHook listenHook;
        private MethodInfo _listenItem = typeof(PowerLoreItem).Assembly.GetType("LoreCore.Items.ListenItem").GetNestedType("<>c__DisplayClass9_0", BindingFlags.NonPublic).GetMethod("<PlayMakerFSM_OnEnable>b__0", BindingFlags.Instance | BindingFlags.NonPublic);
        
        public override void Initialize() {
            SetBossVars();
            On.PlayMakerFSM.OnEnable += EditFsm;
            On.GameManager.OnNextLevelReady += LateLoad;
            GameManager.instance.StartCoroutine(HookMods());
        }

        public override void Unload() {
            On.PlayMakerFSM.OnEnable -= EditFsm;
            On.GameManager.OnNextLevelReady -= LateLoad;
            DisposeModHooks();
        }

        private void SetBossVars() {
            foreach(string intBoss in intBosses)
                pd.SetInt(intBoss, 1);
            foreach(string boolBoss in boolBosses)
                pd.SetBool(boolBoss, true);
            foreach((string scene, string id) in  sceneDataBosses)
                SceneDataUtil.Save(scene, id, true);
            pd.hornetGreenpath = 3;
        }

        private void EditFsm(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self) {
            orig(self);
            if(self.gameObject.name == "Dung Defender NPC" && self.FsmName == "Emerge And Turn") {
                self.GetState("Sleep").RemoveTransitionsOn("WAKE");
            }
            if(self.gameObject.name.StartsWith("Mimic Spider Fake") && self.FsmName == "Control") {
                self.FsmVariables.GetFsmBool("Activated").Value = true;
            }
            if(self.FsmName == "Mantis") {
                PlayerDataBoolTest pdbt = self.GetState("Lords Defeated?").GetFirstActionOfType<PlayerDataBoolTest>();
                pdbt.isTrue = pdbt.isFalse;
            }
            if(self.FsmName == "Mantis Flyer") {
                self.GetState("Rest").GetFirstActionOfType<PlayerDataBoolTest>().Enabled = false;
                self.GetState("Lord Def?").GetFirstActionOfType<PlayerDataBoolTest>().Enabled = false;
            }
            if(self.gameObject.name == "Lords Undefeated Group" && self.FsmName == "FSM") {
                self.GetState("Check").RemoveTransitionsOn("DESTROY");
            }
            if(self.gameObject.name == "Lords Defeated Group" && self.FsmName == "FSM") {
                 PlayerDataBoolTest pdbt = self.GetState("Check").GetFirstActionOfType<PlayerDataBoolTest>();
                pdbt.isTrue = pdbt.isFalse;
            }
        }

        private void LateLoad(On.GameManager.orig_OnNextLevelReady orig, GameManager self) {
            orig(self);
            if(objectsToRemove.TryGetValue(self.sceneName, out List<string> objects)) {
                foreach(string obj in objects)
                    GameObject.Find(obj).SetActive(false);
            }
            if(self.sceneName == "Fungus2_15") {
                Container c = Container.GetContainer(Container.Shiny);
                GameObject shiny = c.GetNewContainer(new(c.Name, Ref.Settings.Placements["Mantis_Respect"], FlingType.DirectDeposit));
                shiny.transform.position = new(30.2f, 7.4f);
                shiny.SetActive(true);
            }
        }

        private IEnumerator HookMods() {
            yield return null;
            
            listenHook = new ILHook(_listenItem, OnListenEnable);
        }

        private void DisposeModHooks() {
            listenHook.Dispose();
        }

        private void OnListenEnable(ILContext il) {
            ILCursor cursor = new ILCursor(il).Goto(0);
            cursor.GotoNext(i => i.MatchLdstr("You can't understand them."));
            cursor.Index++;
            cursor.EmitDelegate<Func<string, string>>(old => "They can't understand you.");
        }
    }
}