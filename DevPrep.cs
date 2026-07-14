using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ItemChanger;
using MoreDoors.Data;
using RandomizerCore.Exceptions;
using RandomizerCore.Logic;
using RandomizerCore.Randomization;
using RandomizerMod.RC;
using RandomizerMod.Settings;

namespace GhostPlando {
    // None of this class actually needs to exist by the time the plando is released
    // But it makes development easier for me and might be useful later
    internal static class DevPrep {
        private static bool GenerateDoorData = true;
        private static bool AutoAddIcModules = true;
        private static bool AlterRandoLogic = false;
        private static (bool, bool) RemoveLocationsItems = (true, false);
        private static bool ConstrainKeyPlacements = true;

        private static BossStrings[] bossData = [
            new(("Gruz_Mother", "GRUZ", LocationNames.Boss_Geo_Gruz_Mother)),
            new(("Vengefly_King", "BIGBUZZ", LocationNames.Boss_Geo_Vengefly_King)),
            new(("False_Knight", "FKNIGHT", LocationNames.City_Crest)),
            new(("Hornet_Protector", "HORNET", LocationNames.Mothwing_Cloak)),
            new(("Hornet_Sentinel", "HORNET", LocationNames.Kings_Brand)),
            new(("Massive_Moss_Charger", "MEGAMOSS", LocationNames.Boss_Geo_Massive_Moss_Charger)),
            new(("Mantis_Lords", "MANTISLORDS", "Mantis_Respect")),
            new(("Broken_Vessel", "BROKENVESSEL", LocationNames.Boss_Essence_Lost_Kin)),
            new(("Collector", "COLLECTOR", LocationNames.Collectors_Map)),
            new(("Crystal_Guardian", "MEGABEAMMINER", LocationNames.Boss_Geo_Enraged_Guardian)),
            new(("Uumuu", "UMMU", LocationNames.Journal_Entry_Charged_Lumafly)),
            //new(("Traitor_Lord", "TRAITORLORD")),
            //new(("Soul_Warrior", "MAGEKNIGHT")),
            //new(("Soul_Master", "SOULMASTER")),
            new(("Dung_Defender", "DUNGDEF", LocationNames.Defenders_Crest)),
            new(("Watcher_Knights", "WATCHERKNIGHTS", LocationNames.Hallownest_Seal_Watcher_Knight)),
            new(("No_Eyes", "GHOST_NOEYES", LocationNames.Boss_Essence_No_Eyes)),
            //new(("Marmu", "GHOST_MARMU")),
            new(("Galien", "GHOST_GALIEN", LocationNames.Boss_Essence_Galien)),
            new(("Markoth", "GHOST_MARKOTH", LocationNames.Boss_Essence_Markoth)),
            new(("Xero", "GHOST_XERO", LocationNames.Boss_Essence_Xero)),
            new(("Gorb", "GHOST_GORB", LocationNames.Boss_Essence_Gorb)),
            new(("Elder_Hu", "GHOST_HU", LocationNames.Boss_Essence_Elder_Hu)),
            new(("Grimm", "GRIM", LocationNames.Charm_Notch_Grimm))
        ];

        internal static void OnConstructor() {
            if(GenerateDoorData)
                AddAllNoskDoors();
        }

        internal static void OnInitialize() {
            if(AutoAddIcModules)
                RandoController.OnExportCompleted += AddModules;
            if(AlterRandoLogic)
                RCData.RuntimeLogicOverride.Subscribe(101010f, EditLogic);
            else
                RCData.RuntimeLogicOverride.Subscribe(101010f, BackupEditLogic);
            if(RemoveLocationsItems.Item1)
                RequestBuilder.OnUpdate.Subscribe(101010f, RemoveLocations);
            if(RemoveLocationsItems.Item2)
                RequestBuilder.OnUpdate.Subscribe(101010f, RemoveItems);
            if(ConstrainKeyPlacements)
                RequestBuilder.OnUpdate.Subscribe(101, DoConstraints);
        }

        private static void AddModules(RandoController controller) {
            List<Type> moduleTypes = [typeof(Modules.IntangibleKnightModule), typeof(Modules.VanishedBossesModule), typeof(Modules.NoskBossModule), typeof(Modules.MimicMusicModule)];
            moduleTypes.ForEach(mType => ItemChangerMod.Modules.GetOrAdd(mType));
        }

        private static void AddAllNoskDoors() {
            AddNoskDoor(1, 120, 52, true);
            AddNoskDoor(2, 108, 52, true);
            AddNoskDoor(3, 89, 44, true);
            AddNoskDoor(4, 72, 44, true);
            //AddNoskDoor(5, 64, 49, true);
            AddNoskDoor(5, 56, 49, true);
            AddNoskDoor(6, 40, 49, true);
            AddNoskDoor(7, 23, 49, true);
            AddNoskDoor(8, 13.5f, 53, true);
            //AddNoskDoor(10, 13.5f, 40.4f, false);//needs to be higher?
            AddNoskDoor(9, 61, 37, false);
            AddNoskDoor(10, 53, 21, true);
            AddNoskDoor(11, 39, 16, true);
            AddNoskDoor(12, 23, 33, true);
            AddNoskDoor(13, 7, 33, true);
            //AddNoskDoor(16, 8, 24, false);
            AddNoskDoor(14, 23.6f, 11, false);
            AddNoskDoor(15, 56, 11, false);
            AddNoskDoor(16, 67, 16, false);
            AddNoskDoor(17, 83, 16, false);
            AddNoskDoor(18, 103, 16, false);
            AddNoskDoor(19, 85, 3.85f, false);
            AddNoskDoor(20, 117, 4, false);
            //AddNoskDoor(24, 123, 4, false);
        }

        private static void AddNoskDoor(int iteration, float x, float y, bool faceLeft) {
            BossStrings boss = bossData[iteration - 1];
            AddSingleDoor(boss.DoorName, boss.KeyName, "Deepnest_31", "right2", boss.NoKeyDesc, $"The spirit of {boss.BossName} seeks justice.", $"The spirit of {boss.BossName} seeks justice.", $"For {boss.BossName}.", x, y, faceLeft);
        }

        private static void AddSingleDoor(string doorName, string keyName, string scene, string gate, string noKey, string yesKey, string invDesc, string usedInvDesc, float x, float y, bool faceLeft) {
            string newKeyName = keyName.Replace("_", " ");
            string newYesKey = $"{yesKey}<br>Insert key?";
            DoorData.DoorInfo.Location mainDoor = new() {
                SceneName = scene,
                GateName = gate,
                NoKeyDesc = noKey,
                KeyDesc = newYesKey,
                FadeOutOnOpen = true,
                RequiresLantern = false,
                X = x,
                Y = y + 2.4f
            };
            DoorData data = new() {
                CamelCaseName = doorName,
                UpperCaseName = doorName.ToUpper(),
                UIName = doorName,
                Door = new() {
                    Sprite = new MoreDoors.IC.EmbeddedSprite("Doors.Deepnest"),
                    LeftLocation = faceLeft ? mainDoor : null,
                    RightLocation = faceLeft ? null : mainDoor,
                    Mode = faceLeft ? DoorData.DoorInfo.SplitMode.LeftTwin : DoorData.DoorInfo.SplitMode.RightTwin
                },
                Key = new() {
                    ItemName = "MoreDoors-" + keyName,
                    UIItemName = newKeyName,
                    InvDesc = invDesc,
                    UsedInvDesc = usedInvDesc,
                    Sprite = new EmbeddedDoorSprite("NoskKey")
                }
            };
            DoorData.AddExtensionDoor(doorName, data);
        }

        private static void EditLogic(GenerationSettings gs, LogicManagerBuilder lmb) {
            lmb.DoLogicEdit(new("Cliffs_02[right1]", "Cliffs_02"));
            lmb.DoLogicEdit(new(LocationNames.Boss_Essence_Gorb, "Cliffs_02 + DREAMNAIL"));
            lmb.DoLogicEdit(new("Cliffs_01[right4]", "Cliffs_01[right4]"));
            lmb.DoMacroEdit(new("ALLBALDURS", "FALSE"));
            lmb.DoMacroEdit(new("LEFTBALDURS", "FALSE"));
            lmb.DoMacroEdit(new("RIGHTBALDURS", "FALSE"));
            lmb.DoLogicEdit(new("Tutorial_01", "Tutorial_01[right1]"));
            lmb.DoLogicEdit(new("Crossroads_07[left3]", "FALSE"));
            lmb.DoLogicEdit(new("Crossroads_49[left1]", "Crossroads_49[left1] | Crossroads_49[right1] + Elevator_Pass"));
            lmb.DoLogicEdit(new("Crossroads_49[right1]", "Crossroads_49[right1] | Crossroads_49[left1] + Elevator_Pass"));
            lmb.DoLogicEdit(new("Crossroads_49b[right1]", "Crossroads_49b[right1]"));
            lmb.DoSubst(new("Defeated_Gruz_Mother", "COMBAT[Gruz_Mother]", "TRUE"));
            lmb.DoLogicEdit(new("Can_Stag", "FALSE"));
            lmb.DoSubst(new("Defeated_False_Knight", "COMBAT[False_Knight]", "TRUE"));
            foreach(string location in new string[] { "Fungus1_32[bot1]", "Fungus1_32[left1]", LocationNames.Lore_Tablet_Greenpath_Lifeblood })
                lmb.DoSubst(new(location, "Fungus1_32[top1]", "FALSE"));
            lmb.DoLogicEdit(new("Fungus1_32[top1]", "Fungus1_32[top1]"));
            lmb.DoLogicEdit(new(LocationNames.Boss_Geo_Vengefly_King, "Fungus1_20_v02[bot1] | Fungus1_20_v02[bot2] | Fungus1_20_v02[right1]"));
            lmb.DoSubst(new("Defeated_Hornet_1", "COMBAT[Hornet_1]", "TRUE"));
            lmb.DoSubst(new("Defeated_No_Eyes", "COMBAT[No_Eyes]", "TRUE"));
            lmb.DoSubst(new("Defeated_Uumuu", "COMBAT[Uumuu]", "TRUE"));
            lmb.DoLogicEdit(new("Fungus3_05[left1]", "Fungus3_05[left1]"));
            lmb.DoLogicEdit(new("Fungus3_05[right1]", "Fungus3_05[right1]"));
            lmb.DoSubst(new("Fungus3_05[right2]", "Fungus3_05[left1]", "FALSE"));
            foreach(string location in new string[] { "Deepnest_42[top1]", "Deepnest_42[bot1]", "Deepnest_42[left1]" })
                lmb.DoLogicEdit(new(location, location));
            lmb.DoLogicEdit(new("Defeated_Shrumal_Ogre_Arena", "FALSE"));
            lmb.DoSubst(new("Defeated_Elder_Hu", "COMBAT[Elder_Hu]", "TRUE"));
            lmb.DoLogicEdit(new("Fungus2_14[bot3]", "Fungus2_14[bot3]"));
            lmb.DoSubst(new("Fungus2_18", "Fungus2_18[right1]", "FALSE"));
            lmb.DoLogicEdit(new("Deepnest_02[right1]", "Deepnest_02[right1]"));
            lmb.DoLogicEdit(new("Deepnest_01[right1]", "Deepnest_01[right1]"));
            lmb.DoLogicEdit(new(LocationNames.Grub_Deepnest_Nosk, "Deepnest_31[right1] + MOREDOORS_NOSKGRUZMOTHER_KEY + MOREDOORS_NOSKVENGEFLYKING_KEY"));
            lmb.DoLogicEdit(new("Deepnest_31[right2]", "Deepnest_31[right2] | Deepnest_31[right1] + (ANYCLAW | $SHRIEKPOGO[3,before:AREASOUL]) + MOREDOORS_NOSKGRUZMOTHER_KEY + MOREDOORS_NOSKVENGEFLYKING_KEY + MOREDOORS_NOSKFALSEKNIGHT_KEY + MOREDOORS_NOSKHORNETPROTECTOR_KEY + MOREDOORS_NOSKHORNETSENTINEL_KEY + MOREDOORS_NOSKMASSIVEMOSSCHARGER_KEY + MOREDOORS_NOSKMANTISLORDS_KEY + MOREDOORS_NOSKBROKENVESSEL_KEY + MOREDOORS_NOSKCOLLECTOR_KEY + MOREDOORS_NOSKCRYSTALGUARDIAN_KEY + MOREDOORS_NOSKUUMUU_KEY + MOREDOORS_NOSKDUNGDEFENDER_KEY + MOREDOORS_NOSKWATCHERKNIGHTS_KEY + MOREDOORS_NOSKNOEYES_KEY + MOREDOORS_NOSKGALIEN_KEY + MOREDOORS_NOSKMARKOTH_KEY + MOREDOORS_NOSKXERO_KEY + MOREDOORS_NOSKGORB_KEY + MOREDOORS_NOSKELDERHU_KEY + MOREDOORS_NOSKGRIMM_KEY"));
        }

        private static void BackupEditLogic(GenerationSettings gs, LogicManagerBuilder lmb) {
            lmb.DoLogicEdit(new("Deepnest_31[right2]", "Deepnest_31[right2] | Deepnest_31[right1] + (ANYCLAW | $SHRIEKPOGO[3,before:AREASOUL]) + MOREDOORS_NOSKGRUZMOTHER_KEY + MOREDOORS_NOSKVENGEFLYKING_KEY + MOREDOORS_NOSKFALSEKNIGHT_KEY + MOREDOORS_NOSKHORNETPROTECTOR_KEY + MOREDOORS_NOSKHORNETSENTINEL_KEY + MOREDOORS_NOSKMASSIVEMOSSCHARGER_KEY + MOREDOORS_NOSKMANTISLORDS_KEY + MOREDOORS_NOSKBROKENVESSEL_KEY + MOREDOORS_NOSKCOLLECTOR_KEY + MOREDOORS_NOSKCRYSTALGUARDIAN_KEY + MOREDOORS_NOSKUUMUU_KEY + MOREDOORS_NOSKDUNGDEFENDER_KEY + MOREDOORS_NOSKWATCHERKNIGHTS_KEY + MOREDOORS_NOSKNOEYES_KEY + MOREDOORS_NOSKGALIEN_KEY + MOREDOORS_NOSKMARKOTH_KEY + MOREDOORS_NOSKXERO_KEY + MOREDOORS_NOSKGORB_KEY + MOREDOORS_NOSKELDERHU_KEY + MOREDOORS_NOSKGRIMM_KEY"));
        }

        private static void RemoveLocations(RequestBuilder rb) {
            foreach(string locationName in new string[] {
                LocationNames.Cyclone_Slash,
                LocationNames.Lore_Tablet_Howling_Cliffs,
                LocationNames.Wanderers_Journal_Cliffs,
                LocationNames.Baldur_Shell,
                LocationNames.Grimmkin_Flame_Kings_Pass,
                LocationNames.Unbreakable_Greed,
                LocationNames.Unbreakable_Heart,
                LocationNames.Unbreakable_Strength,
                LocationNames.Sly,
                LocationNames.Sly_Key,
                LocationNames.Nailmasters_Glory,//verify Sly cannot be rescued
                LocationNames.Iselda,
                LocationNames.Mask_Shard_Brooding_Mawlek,
                LocationNames.Journal_Entry_Goam,
                LocationNames.Salubra,
                LocationNames.Rancid_Egg_Blue_Lake,
                LocationNames.Grub_Crossroads_Stag,
                LocationNames.Dirtmouth_Stag,
                LocationNames.Glowing_Womb,
                LocationNames.Boss_Essence_Failed_Champion,
                LocationNames.Lifeblood_Cocoon_Ancestral_Mound,
                LocationNames.Soul_Catcher,
                LocationNames.Hunters_Journal,
                LocationNames.Grimmkin_Flame_Greenpath,//check whether the kin still appear
                LocationNames.Lifeblood_Cocoon_Greenpath,
                LocationNames.Grub_Greenpath_Stag,//technically possible with dash slash as a grub jar
                LocationNames.Wanderers_Journal_Greenpath_Stag,
                LocationNames.Great_Slash,
                LocationNames.Howling_Wraiths,
                LocationNames.Grub_Fog_Canyon,
                //LocationNames.Monomon,//Previewable but can't speak to Quirrel, nvm not doing dreamers
                LocationNames.Rancid_Egg_Queens_Gardens,
                LocationNames.Love_Key,
                LocationNames.Grub_Queens_Gardens_Stag,
                LocationNames.Queens_Gardens_Stag,
                LocationNames.Hallownest_Seal_Queens_Gardens,
                LocationNames.Grub_Queens_Gardens_Marmu,
                LocationNames.Grub_Queens_Gardens_Top,
                LocationNames.Queen_Fragment,
                LocationNames.Boss_Essence_Marmu,
                LocationNames.Charm_Notch_Shrumal_Ogres,
                LocationNames.Leg_Eater,
                LocationNames.Boss_Essence_Grey_Prince_Zote,
                LocationNames.Mantis_Claw,
                LocationNames.Grimmkin_Flame_Fungal_Core,
                LocationNames.Lifeblood_Cocoon_Mantis_Village,
                LocationNames.Grub_Deepnest_Mimic,
                LocationNames.Vessel_Fragment_Deepnest

                //Resume with Deepnest west of hot spring, everything left of fungal/crossroads is done (but revisit grimm)
                //Also lots of locations are out of logic with missing spell/cdash items
            }) {
                rb.RemoveLocationByName(locationName);
            }
            rb.RemoveLocationsWhere(s => s.Contains("_Map") && s != LocationNames.Collectors_Map && s != LocationNames.Resting_Grounds_Map);            
        }

        private static void RemoveItems(RequestBuilder rb) {
            foreach(string itemName in new string[] {
                ItemNames.Crystal_Heart,
                ItemNames.Vengeful_Spirit,
                ItemNames.Shade_Soul,
                ItemNames.Desolate_Dive,
                ItemNames.Descending_Dark,
                ItemNames.Howling_Wraiths,
                ItemNames.Abyss_Shriek,
                ItemNames.Grubberflys_Elegy,
                ItemNames.Grimmkin_Flame,
                ItemNames.Defenders_Crest,
                ItemNames.Spore_Shroom,
                ItemNames.Weaversong,
                ItemNames.Glowing_Womb,
                ItemNames.Grimmkin_Flame
            }) {
                rb.RemoveItemByName(itemName);
            }
            rb.RemoveItemsWhere(i => i.EndsWith("_Stag") && i != ItemNames.Lore_Tablet_Greenpath_Stag);
        }

        private static void DoConstraints(RequestBuilder rb) {
            foreach(ItemGroupBuilder igb in rb.EnumerateItemGroups()) {
                if(igb.strategy is DefaultGroupPlacementStrategy dgps) {
                    dgps.ConstraintList.Add(new DefaultGroupPlacementStrategy.Constraint(
                        (item, location) => !bossData.Any(bs => (bs.KeyName == item.Name && bs.KeyLocation != location.Name)),
                        Label: "Nosk Key Placement",
                        Fail: (item, location) => throw new OutOfLocationsException()
                    ));
                }
            }
        }
    }

    internal class BossStrings {
        public string DoorName;
        public string KeyName;
        public string NoKeyDesc;
        public string BossName;
        public string KeyLocation;
        private Dictionary<string, Dictionary<string, string>> currentEntrySheets = typeof(Language.Language).GetField("currentEntrySheets", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null) as Dictionary<string, Dictionary<string, string>>;

        public BossStrings((string, string, string) data) {
            DoorName = "Nosk" + data.Item1.Replace("_", "");
            KeyName = "Nosk_" + data.Item1 + "_Key";
            NoKeyDesc = currentEntrySheets["CP3"]["GG_LOCKED_DESC_" + data.Item2];
            BossName = data.Item1.Replace("_", " ");
            KeyLocation = data.Item3;
        }
    }
}
