using System.Collections.Generic;
using UnityEngine;
using ItemChanger.Modules;

namespace GhostPlando.Modules {
    internal class IntangibleKnightModule: Module {
        public override void Initialize() {
            On.HeroController.Update10 += HeroUpdate10;
            On.GameManager.OnNextLevelReady += ConditionalNailDamage;
        }

        public override void Unload() {
            On.HeroController.Update10 -= HeroUpdate10;
            On.GameManager.OnNextLevelReady -= ConditionalNailDamage;
        }

        private void HeroUpdate10(On.HeroController.orig_Update10 orig, HeroController self) {
            orig(self);
            self.GetComponent<tk2dSprite>().color = new Color(1, 1, 1, 0.3f);
        }

        private void ConditionalNailDamage(On.GameManager.orig_OnNextLevelReady orig, GameManager self) {
            orig(self);
            PlayerData.instance.SetInt(nameof(PlayerData.nailDamage), self.sceneName == "Deepnest_32" ? 5 : 0);
            //new List<string> { "Slash", "AltSlash" }.ForEach(s => HeroController.instance.transform.Find($"Attacks/{s}").GetComponent<PlayMakerFSM>().FsmVariables.GetFsmInt("damageDealt").Value = PlayerData.instance.GetInt(nameof(PlayerData.nailDamage)));
            HeroController.instance.transform.Find("Attacks").gameObject.LocateMyFSM("Set Slash Damage").SendEvent("UPDATE NAIL DAMAGE");
        }
    }
}
