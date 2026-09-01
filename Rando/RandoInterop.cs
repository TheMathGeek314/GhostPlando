using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using ItemChanger;
using ItemChanger.Tags;
using MoreDoors.Data;
using RandomizerCore;
using RandomizerCore.Logic;
using RandomizerCore.LogicItems;
using RandomizerMod.RC;
using RandomizerMod.Settings;

namespace GhostPlando {
    internal static class RandoInterop {
        private static Dictionary<string, Dictionary<string, string>> currentEntrySheets = typeof(Language.Language).GetField("currentEntrySheets", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) as Dictionary<string, Dictionary<string, string>>;

        public static void Hook() {
            Finder.DefineCustomItem(new ShadelessItem());
            Finder.DefineCustomItem(new IncorporealItem());
            Finder.DefineCustomItem(new ImpossibleItem());

            RCData.RuntimeLogicOverride.Subscribe(50, ApplyLogic);

            RandoExtraInterop.Hook();
        }

        private static void ApplyLogic(GenerationSettings gs, LogicManagerBuilder lmb) {
            lmb.AddItem(new EmptyItem("Ghost_Shadeless"));
            lmb.AddItem(new EmptyItem("Ghost_Incorporeal"));
            lmb.AddItem(new SingleItem("Ghost_Impossible", new TermValue(lmb.GetOrAddTerm("Impossible"), 1)));
        }

        internal static void DefineDoors() {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string jsonName = assembly.GetManifestResourceNames().Single(str => str.EndsWith("GhostDoorData.json"));
            using Stream doorDataStream = assembly.GetManifestResourceStream(jsonName);
            foreach(GhostDoorData data in new ParseJson(doorDataStream).parseFile<GhostDoorData>()) {
                string bossNameSpaces = data.bossName.Replace("_", " ");
                DoorData.DoorInfo.Location mainDoor = new() {
                    SceneName = "Deepnest_31",
                    GateName = "right2",
                    NoKeyDesc = currentEntrySheets["CP3"]["GG_LOCKED_DESC_" + data.languageKey],
                    KeyDesc = $"The spirit of {bossNameSpaces} seeks justice.<br>Insert key?",
                    FadeOutOnOpen = true,
                    RequiresLantern = false,
                    X = data.transform.x,
                    Y = data.transform.y + 2.4f
                };
                string doorName = "Nosk" + data.bossName.Replace("_", "");
                string keyName = $"Mysterious_Key_({data.bossName})";
                DoorData ddata = new() {
                    CamelCaseName = doorName,
                    UpperCaseName = doorName.ToUpper(),
                    UIName = doorName,
                    Door = new() {
                        Sprite = new MoreDoors.IC.EmbeddedSprite("Doors.Deepnest"),
                        LeftLocation = data.transform.faceLeft ? mainDoor : null,
                        RightLocation = data.transform.faceLeft ? null : mainDoor,
                        Mode = data.transform.faceLeft ? DoorData.DoorInfo.SplitMode.LeftTwin : DoorData.DoorInfo.SplitMode.RightTwin
                    },
                    Key = new() {
                        ItemName = "MoreDoors-" + keyName,
                        UIItemName = keyName.Replace("_", " "),
                        InvDesc = $"The spirit of {bossNameSpaces} seeks justice.",
                        UsedInvDesc = $"For {bossNameSpaces}.",
                        Sprite = new EmbeddedGhostSprite("NoskKey")
                    },
                    NoRando = true
                };
                DoorData.AddExtensionDoor(doorName, ddata);
            }
        }

        internal static InteropTag AddTag(TaggableObject obj) {
            InteropTag tag = obj.GetOrAddTag<InteropTag>();
            tag.Message = "RandoSupplementalMetadata";
            tag.Properties["ModSource"] = GhostPlando.Instance.GetName();
            return tag;
        }
    }
}
