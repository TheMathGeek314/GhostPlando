using Modding;
using DebugMod;
using GhostPlando.Modules;
using ItemChanger;
using MenuChanger;
using MenuChanger.MenuElements;
using RandomizerMod.Logging;
using RandomizerMod.Menu;
using RandomizerMod.RandomizerData;
using RandomizerMod.RC;

namespace GhostPlando {
    public class GlobalSettings {
        public bool Enabled = false;
    }

    internal static class RandoExtraInterop {
        public static void Hook() {
            RandoController.OnExportCompleted += controller => {
                if(GhostPlando.globalSettings.Enabled) {
                    ItemChangerMod.Modules.GetOrAdd<MimicMusicModule>();
                    NoskBossModule nobomo = ItemChangerMod.Modules.GetOrAdd<NoskBossModule>();
                    nobomo.NonPlandoMode = true;
                    nobomo.EncounteredWingedNosk = false;
                    nobomo.ShowcaseMode = false;
                }
            };
            SettingsLog.AfterLogSettings += (args, w) => {
                w.WriteLine("Logging Mimic Paragon settings:");
                w.WriteLine(JsonUtil.Serialize(GhostPlando.globalSettings));
            };
            RandomizerMenuAPI.AddMenuPage(_ => { }, (MenuPage landingPage, out SmallButton settingsButton) => {
                SmallButton button = new(landingPage, "Mimic Paragon");
                void UpdateButtonColor() {
                    button.Text.color = GhostPlando.globalSettings.Enabled ? Colors.TRUE_COLOR : Colors.DEFAULT_COLOR;
                }
                UpdateButtonColor();
                button.OnClick += () => {
                    GhostPlando.globalSettings.Enabled = !GhostPlando.globalSettings.Enabled;
                    UpdateButtonColor();
                };
                settingsButton = button;
                return true;
            });
            if(ModHooks.GetMod("DebugMod") is Mod) {
                DebugInterop.Bind();
            }
        }
    }

    public static class DebugInterop {
        public static void Bind() {
            DebugMod.DebugMod.AddToKeyBindList(typeof(KeyBinds));
        }
    }

    public static class KeyBinds {
        [BindableMethod(name = "Reset Checkpoint", category = "Mimic Paragon Nosk")]
        public static void resetWosk() {
            NoskBossModule nobomo = ItemChangerMod.Modules.Get<NoskBossModule>();
            if(nobomo != null) {
                nobomo.EncounteredWingedNosk = false;
                Console.AddLine("Set Winged Nosk checkpoint to false");
            }
        }
    }
}
