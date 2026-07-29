using System;
using System.Collections.Generic;
using ItemChanger;
using RandomizerCore.Logic;
using RandomizerMod.RC;
using RandomizerMod.Settings;

namespace GhostPlando {
    // None of this class actually needs to exist by the time the plando is released
    // But it makes development easier for me and might be useful later
    internal static class DevPrep {
        private static bool AutoAddIcModules = false;//needed for template seed
        private static bool AlterRandoLogic = false;//needed for template seed
        private static (bool, bool) RemoveLocationsItems = (false, false);//optional for first-pass progression template, don't use it breaks things
        private static bool AddStartItems = false;//needed for template seed

        private static string[] locationsToRemove = [
            LocationNames.Ancient_Basin_Map,
            LocationNames.City_of_Tears_Map,
            LocationNames.Crossroads_Map,
            LocationNames.Crystal_Peak_Map,
            LocationNames.Deepnest_Map_Right,
            LocationNames.Deepnest_Map_Upper,
            LocationNames.Fog_Canyon_Map,
            LocationNames.Fungal_Wastes_Map,
            LocationNames.Greenpath_Map,
            LocationNames.Howling_Cliffs_Map,
            LocationNames.Kingdoms_Edge_Map,
            LocationNames.Queens_Gardens_Map,
            LocationNames.Royal_Waterways_Map,
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
            LocationNames.Nailmasters_Glory,
            LocationNames.Iselda,
            LocationNames.Mask_Shard_Brooding_Mawlek,
            LocationNames.Journal_Entry_Goam,
            LocationNames.Salubra,
            "Salubra_(Requires_Charms)",
            LocationNames.Rancid_Egg_Blue_Lake,
            LocationNames.Grub_Crossroads_Stag,
            LocationNames.Dirtmouth_Stag,
            LocationNames.Glowing_Womb,
            LocationNames.Boss_Essence_Failed_Champion,
            LocationNames.Lifeblood_Cocoon_Ancestral_Mound,
            LocationNames.Soul_Catcher,
            LocationNames.Hunters_Journal,
            LocationNames.Grimmkin_Flame_Greenpath,
            LocationNames.Lifeblood_Cocoon_Greenpath,
            LocationNames.Grub_Greenpath_Stag,
            LocationNames.Wanderers_Journal_Greenpath_Stag,
            LocationNames.Great_Slash,
            LocationNames.Howling_Wraiths,
            LocationNames.Grub_Fog_Canyon,
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
            LocationNames.Lore_Tablet_Fungal_Wastes_Below_Shrumal_Ogres,
            LocationNames.Lore_Tablet_Fungal_Wastes_Hidden,
            LocationNames.Boss_Essence_Grey_Prince_Zote,
            LocationNames.Mantis_Claw,
            LocationNames.Grimmkin_Flame_Fungal_Core,
            LocationNames.Lore_Tablet_Fungal_Core,
            LocationNames.Lifeblood_Cocoon_Mantis_Village,
            LocationNames.Lore_Tablet_Spore_Shroom,
            LocationNames.Grub_Deepnest_Mimic,
            LocationNames.Kings_Idol_Deepnest,
            LocationNames.Geo_Chest_Weavers_Den,
            LocationNames.Weaversong,
            LocationNames.Rancid_Egg_Weavers_Den,
            LocationNames.Rancid_Egg_Dark_Deepnest,
            LocationNames.Rancid_Egg_Beasts_Den,
            LocationNames.Grimmkin_Flame_Brumm,
            LocationNames.Rancid_Egg_Crystal_Peak_Dive_Entrance,
            LocationNames.Deep_Focus,
            LocationNames.Grimmkin_Flame_Crystal_Peak,
            LocationNames.Grub_Crystal_Peak_Mimic,
            LocationNames.Rancid_Egg_Crystal_Peak_Tall_Room,
            LocationNames.Grub_Crystallized_Mound,
            LocationNames.Descending_Dark,
            LocationNames.Grimmkin_Flame_Resting_Grounds,
            LocationNames.Mask_Shard_Grey_Mourner,
            LocationNames.Hallownest_Seal_Resting_Grounds_Catacombs,
            LocationNames.Wanderers_Journal_Resting_Grounds_Catacombs,
            LocationNames.Geo_Chest_Resting_Grounds,
            LocationNames.Soul_Eater,
            LocationNames.Grub_Resting_Grounds,
            LocationNames.Grimmkin_Flame_City_Storerooms,
            LocationNames.Boss_Geo_Elegant_Soul_Warrior,
            LocationNames.Shade_Soul,
            LocationNames.Lore_Tablet_Sanctum_Entrance,
            LocationNames.Boss_Geo_Sanctum_Soul_Warrior,
            LocationNames.Spell_Twister,
            LocationNames.Desolate_Dive,
            LocationNames.Boss_Essence_Soul_Tyrant,
            LocationNames.Lore_Tablet_Sanctum_Past_Soul_Master,
            LocationNames.Geo_Chest_Soul_Master,
            LocationNames.Hallownest_Seal_Soul_Sanctum,
            LocationNames.Grub_Soul_Sanctum,
            LocationNames.Grub_Watchers_Spire,
            LocationNames.Lore_Tablet_Watchers_Spire,
            LocationNames.Boss_Geo_Gorgeous_Husk,
            LocationNames.Vessel_Fragment_City,
            LocationNames.Rancid_Egg_Tuk_Defenders_Crest,
            LocationNames.Flukenest,
            LocationNames.Godtuner,
            LocationNames.Grub_Waterways_Main,
            LocationNames.Rancid_Egg_Waterways_West_Bluggsac,
            LocationNames.Rancid_Egg_Waterways_Main,
            LocationNames.Kings_Idol_Dung_Defender,
            LocationNames.Boss_Essence_White_Defender,
            LocationNames.Rancid_Egg_Waterways_East,
            LocationNames.Grimmkin_Flame_Ancient_Basin,
            LocationNames.Grub_Basin_Requires_Dive,
            LocationNames.Pale_Ore_Basin,
            LocationNames.Vessel_Fragment_Basin,
            LocationNames.Hidden_Station_Stag,
            LocationNames.Abyss_Shriek,
            LocationNames.Grimmkin_Flame_Hive,
            LocationNames.Mask_Shard_Hive,
            LocationNames.Hiveblood,
            LocationNames.Grub_Hive_Internal,
            LocationNames.Grub_Hive_External,
            LocationNames.Grimmkin_Flame_Kingdoms_Edge,
            LocationNames.Wanderers_Journal_Kingdoms_Edge_Entrance,
            LocationNames.Dash_Slash,
            LocationNames.Grub_Kingdoms_Edge_Oro,
            LocationNames.Lore_Tablet_Kingdoms_Edge,
            LocationNames.Rancid_Egg_Near_Quick_Slash,
            LocationNames.Quick_Slash,
            LocationNames.Wanderers_Journal_Kingdoms_Edge_Requires_Dive,
            LocationNames.Grub_Kingdoms_Edge_Camp,
            LocationNames.Wanderers_Journal_Kingdoms_Edge_Camp,
            LocationNames.Kings_Idol_Great_Hopper,
            LocationNames.Charm_Notch_Colosseum,
            LocationNames.Pale_Ore_Colosseum,
            LocationNames.Kings_Idol_Pale_Lurker,
            LocationNames.Simple_Key_Lurker,
            LocationNames.King_Fragment,
            LocationNames.Arcane_Egg_Lifeblood_Core,
            LocationNames.Mimic_Grub_Deepnest_1,
            LocationNames.Mimic_Grub_Deepnest_2,
            LocationNames.Mimic_Grub_Deepnest_3,
            LocationNames.Mask_Shard_Bretta
        ];

        internal static void OnInitialize() {
            if(AutoAddIcModules)
                RandoController.OnExportCompleted += AddModules;
            if(AlterRandoLogic)
                RCData.RuntimeLogicOverride.Subscribe(101010f, EditLogic);
            if(RemoveLocationsItems.Item1)
                RequestBuilder.OnUpdate.Subscribe(101010f, RemoveLocations);
            if(RemoveLocationsItems.Item2)
                RequestBuilder.OnUpdate.Subscribe(101010f, RemoveItems);
            if(AddStartItems)
                RequestBuilder.OnUpdate.Subscribe(-499, rb => {
                    rb.AddToStart("Ghost_Shadeless");
                    rb.AddToStart("Ghost_Incorporeal");
                    rb.AddItemByName("Ghost_Impossible");
                });
        }

        private static void AddModules(RandoController controller) {
            List<Type> moduleTypes = [typeof(Modules.IntangibleKnightModule), typeof(Modules.VanishedBossesModule), typeof(Modules.NoskBossModule), typeof(Modules.MimicMusicModule), typeof(Modules.ShadelessModule)];
            moduleTypes.ForEach(mType => ItemChangerMod.Modules.GetOrAdd(mType));
        }

        private static void EditLogic(GenerationSettings gs, LogicManagerBuilder lmb) {
            lmb.DoLogicEdit(new("Cliffs_02[right1]", "Cliffs_02"));
            lmb.DoSubst(new("Defeated_Gorb", "COMBAT[Gorb]", "TRUE"));
            lmb.DoLogicEdit(new("Cliffs_01[right4]", "Cliffs_01[right4]"));
            lmb.DoMacroEdit(new("ALLBALDURS", "FALSE"));
            lmb.DoMacroEdit(new("LEFTBALDURS", "FALSE"));
            lmb.DoMacroEdit(new("RIGHTBALDURS", "FALSE"));
            lmb.DoLogicEdit(new("Tutorial_01", "Tutorial_01[right1]"));
            lmb.DoLogicEdit(new("Defeated_Grimm", "Grimm_Main_Tent[left1] + GRIMMCHILD"));
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
            foreach(string substring in new string[] { "Hornet_1", "No_Eyes", "Uumuu" })
                lmb.DoSubst(new($"Defeated_{substring}", $"COMBAT[{substring}]", "TRUE"));
            lmb.DoLogicEdit(new("Fungus3_05[left1]", "Fungus3_05[left1]"));
            lmb.DoLogicEdit(new("Fungus3_05[right1]", "Fungus3_05[right1]"));
            lmb.DoSubst(new("Fungus3_05[right2]", "Fungus3_05[left1]", "FALSE"));
            foreach(string location in new string[] { "Deepnest_42[top1]", "Deepnest_42[bot1]", "Deepnest_42[left1]" })
                lmb.DoLogicEdit(new(location, location));
            lmb.DoLogicEdit(new("Defeated_Shrumal_Ogre_Arena", "FALSE"));
            lmb.DoSubst(new("Defeated_Elder_Hu", "COMBAT[Elder_Hu]", "TRUE"));
            lmb.DoLogicEdit(new("Fungus2_14[bot3]", "Fungus2_14[bot3]"));
            lmb.DoSubst(new("Fungus2_18", "Fungus2_18[right1]", "FALSE"));
            lmb.DoSubst(new("Defeated_Mantis_Lords", "COMBAT[Mantis_Lords]", "TRUE"));
            lmb.DoLogicEdit(new("Deepnest_02[right1]", "Deepnest_02[right1]"));
            lmb.DoLogicEdit(new("Deepnest_01[right1]", "Deepnest_01[right1]"));
            lmb.DoLogicEdit(new(LocationNames.Grub_Deepnest_Nosk, "Deepnest_31[right1] + MOREDOORS_NOSKGRUZMOTHER_KEY + MOREDOORS_NOSKVENGEFLYKING_KEY"));
            lmb.DoLogicEdit(new("Deepnest_31[right2]", "Deepnest_31[right2] | Deepnest_31[right1] + (ANYCLAW | $SHRIEKPOGO[3,before:AREASOUL]) + MOREDOORS_NOSKGRUZMOTHER_KEY + MOREDOORS_NOSKVENGEFLYKING_KEY + MOREDOORS_NOSKFALSEKNIGHT_KEY + MOREDOORS_NOSKHORNETPROTECTOR_KEY + MOREDOORS_NOSKHORNETSENTINEL_KEY + MOREDOORS_NOSKMASSIVEMOSSCHARGER_KEY + MOREDOORS_NOSKMANTISLORDS_KEY + MOREDOORS_NOSKBROKENVESSEL_KEY + MOREDOORS_NOSKCOLLECTOR_KEY + MOREDOORS_NOSKCRYSTALGUARDIAN_KEY + MOREDOORS_NOSKUUMUU_KEY + MOREDOORS_NOSKDUNGDEFENDER_KEY + MOREDOORS_NOSKWATCHERKNIGHTS_KEY + MOREDOORS_NOSKNOEYES_KEY + MOREDOORS_NOSKGALIEN_KEY + MOREDOORS_NOSKMARKOTH_KEY + MOREDOORS_NOSKXERO_KEY + MOREDOORS_NOSKGORB_KEY + MOREDOORS_NOSKELDERHU_KEY + MOREDOORS_NOSKGRIMM_KEY"));
            foreach(string location in new string[] { "Deepnest_33[top2]", "Deepnest_33[bot1]" })
                lmb.DoSubst(new(location, "Deepnest_33[top1]", "FALSE"));
            lmb.DoLogicEdit(new("Rescued_Deepnest_Zote", "FALSE"));
            lmb.DoSubst(new("Defeated_Galien", "COMBAT[Galien]", "TRUE"));
            lmb.DoLogicEdit(new("Deepnest_39[door1]", "Deepnest_39[door1]"));
            lmb.DoLogicEdit(new("Mines_03[top1]", "Mines_03[top1] | Mines_03[right1] + RIGHTCLAW | Mines_03 + LEFTCLAW"));
            lmb.DoLogicEdit(new("Mines_05[left2]", "Mines_05[left2]"));
            lmb.DoLogicEdit(new("Mines_20[left2]", "Mines_20[left2]"));
            lmb.DoLogicEdit(new("Mines_20[left3]", "Mines_20[left3] | Mines_20[bot1]"));
            lmb.DoLogicEdit(new("Mines_20[bot1]", "Mines_20[bot1] | Mines_20[left3]"));
            foreach(string location in new string[] { "Mines_04[left1]", "Mines_04" })
                lmb.DoSubst(new(location, "Mines_04[top1]", "FALSE"));
            foreach(string substring in new string[] { "Crystal_Guardian", "Enraged_Guardian", "Xero" })
                lmb.DoSubst(new($"Defeated_{substring}", $"COMBAT[{substring}]", "TRUE"));
            foreach(string location in new string[] { "Ruins2_10[right1]", "Ruins2_10[left1]" })
                lmb.DoLogicEdit(new(location, location));
            lmb.DoLogicEdit(new("Ruins1_09[top1]", "Ruins1_09[top1]"));
            foreach(string location in new string[] { "Ruins1_05", "Ruins1_05[right1]", LocationNames.Grub_City_of_Tears_Left })
                lmb.DoSubst(new(location, "Ruins1_05[top1]", "FALSE"));
            lmb.DoLogicEdit(new("Ruins1_05[top1]", "Ruins1_05[top1] + (RIGHTCLAW | WINGS + (ANYCLAW | PRECISEMOVEMENT))"));
            lmb.DoSubst(new("Defeated_Watcher_Knights", "COMBAT[Watcher_Knights]", "TRUE"));
            lmb.DoLogicEdit(new("Ruins_Bathhouse[right1]", "FALSE"));
            foreach(string location in new string[] { "Ruins2_10b[right1]", "Ruins2_10b[right2]", "Ruins2_10b[left1]" })
                lmb.DoSubst(new(location, "Right_Elevator", "FALSE"));
            lmb.DoLogicEdit(new("Waterways_01[right1]", "FALSE"));
            lmb.DoLogicEdit(new("Opened_Waterways_Exit", "(Waterways_09[left1] | Waterways_09[right1]) + (LONGNAIL | MARKOFPRIDE)"));
            lmb.DoSubst(new("Waterways_13", "Waterways_13[left1]", "FALSE"));
            foreach(string substring in new string[] { "Dung_Defender", "Broken_Vessel", "Lost_Kin" })
                lmb.DoSubst(new($"Defeated_{substring}", $"COMBAT[{substring}]", "TRUE"));
            lmb.DoLogicEdit(new("Abyss_03_c[right1]", "FALSE"));
            foreach(string substring in new string[] { "Markoth", "Hornet_2", "Collector" })
                lmb.DoSubst(new($"Defeated_{substring}", $"COMBAT[{substring}]", "TRUE"));
            lmb.DoLogicEdit(new("Room_Colosseum_Spectate[right1]", "FALSE"));
            lmb.DoLogicEdit(new(LocationNames.Simple_Key_City, "(Ruins1_17[top1] | Ruins1_17[right1] | Ruins1_17[bot1] + (LONGNAIL | MARKOFPRIDE | RIGHTDASHSLASH | GREATSLASH | CYCLONE)) + (RIGHTDASH | WINGS | RIGHTSUPERDASH | ENEMYPOGOS)"));
            foreach(string transition in new string[] { "Ruins1_17[top1]", "Ruins1_17[right1]" })
                lmb.DoLogicEdit(new(transition, "Ruins1_17[top1] | Ruins1_17[right1] | Ruins1_17[bot1] + (LONGNAIL | MARKOFPRIDE | RIGHTDASHSLASH | GREATSLASH | CYCLONE)"));
            lmb.DoMacroEdit(new("COMBAT[Nosk]", "WINGS + SHADOWDASH + DREAMGATE"));

            foreach(string location in locationsToRemove) {
                lmb.DoLogicEdit(new(location, "Impossible"));
            }
        }

        private static void RemoveLocations(RequestBuilder rb) {
            foreach(string locationName in locationsToRemove) {
                rb.RemoveLocationByName(locationName);
            }
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
                ItemNames.Grimmkin_Flame,
                ItemNames.Awoken_Dream_Nail,
                //unusable items or bloat, balancing locations/items
                ItemNames.Shopkeepers_Key,
                ItemNames.Elegant_Key,
                ItemNames.Pale_Ore,
                ItemNames.Rancid_Egg,
                ItemNames.Wanderers_Journal,
                ItemNames.Hallownest_Seal,
                ItemNames.Kings_Idol,
                ItemNames.Arcane_Egg,
                ItemNames.Lurien,
                ItemNames.Monomon,
                ItemNames.Herrah
            }) {
                rb.RemoveItemByName(itemName);
            }
            rb.RemoveItemsWhere(i => i.EndsWith("_Stag") && i != ItemNames.Lore_Tablet_Greenpath_Stag);
            //balancing locations/items
            rb.RemoveItemsWhere(i => i.StartsWith("Lore_Tablet"));
            rb.RemoveItemsWhere(i => i.EndsWith("_Map"));
            rb.RemoveItemsWhere(i => i.StartsWith("Boss_Geo"));
            rb.RemoveItemsWhere(i => i.StartsWith("Journal_Entry"));
            rb.RemoveItemsWhere(i => i.StartsWith("Geo_Chest"));
        }
    }
}
