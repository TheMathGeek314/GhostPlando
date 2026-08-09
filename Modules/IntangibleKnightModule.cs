using UnityEngine;
using ItemChanger.Modules;

namespace GhostPlando.Modules {
    internal class IntangibleKnightModule: Module {
        private bool inNoskRoom = false;

        public override void Initialize() {
            On.HeroController.Update10 += HeroUpdate10;
            On.GameManager.OnNextLevelReady += ConditionalNailDamage;
            On.GameManager.OnNextLevelReady += CheckNoskRoom;
        }

        public override void Unload() {
            On.HeroController.Update10 -= HeroUpdate10;
            On.GameManager.OnNextLevelReady -= ConditionalNailDamage;
        }

        private void HeroUpdate10(On.HeroController.orig_Update10 orig, HeroController self) {
            orig(self);
            if(!inNoskRoom)
                self.GetComponent<tk2dSprite>().color = new Color(1, 1, 1, 0.3f);

        }

        private void ConditionalNailDamage(On.GameManager.orig_OnNextLevelReady orig, GameManager self) {
            orig(self);
            PlayerData.instance.SetInt(nameof(PlayerData.nailDamage), self.sceneName == "Deepnest_32" ? 5 : 0);
            HeroController.instance.transform.Find("Attacks").gameObject.LocateMyFSM("Set Slash Damage").SendEvent("UPDATE NAIL DAMAGE");
        }

        private void CheckNoskRoom(On.GameManager.orig_OnNextLevelReady orig, GameManager self) {
            orig(self);
            if(self.sceneName == "Deepnest_32") {
                inNoskRoom = true;
                HeroController.instance.GetComponent<tk2dSprite>().color = new Color(1, 1, 1, 1);
            }
            else
                inNoskRoom = false;
        }
    }
}
