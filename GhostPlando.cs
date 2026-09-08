using Modding;
using System.Collections.Generic;
using UnityEngine;

namespace GhostPlando {
    public class GhostPlando: Mod, IGlobalSettings<GlobalSettings> {
        new public string GetName() => "GhostPlando";
        public override string GetVersion() => "1.0.0.0";
        internal static GhostPlando Instance { get; private set; }

        public static GlobalSettings globalSettings { get; set; } = new();
        public void OnLoadGlobal(GlobalSettings s) => globalSettings = s;
        public GlobalSettings OnSaveGlobal() => globalSettings;

        public GhostPlando(): base("GhostPlando") {
            Instance = this;
            RandoInterop.DefineDoors();
        }

        public override List<(string, string)> GetPreloadNames() {
            return [
                ("Abyss_19", "Infected Knight"),
                ("Cliffs_02_boss", "Warrior/Ghost Warrior Slug"),
                ("Crossroads_10_boss", "Battle Scene/FK Barrel Summon"),
                ("Deepnest_40", "Warrior/Galien Hammer"),
                ("Deepnest_East_10", "Warrior/Ghost Warrior Markoth"),
                ("Deepnest_East_Hornet", "Barb Region"),
                ("Deepnest_East_Hornet_boss", "Hornet Boss 2"),
                ("Dream_04_White_Defender", "White Defender"),
                ("Dream_Mighty_Zote", "Zoteling"),
                ("Fungus1_04_boss", "Hornet Boss 1"),
                ("Fungus1_20_v02", "_Enemies/Giant Buzzer"),
                ("Fungus1_35", "Warrior/Ghost Warrior No Eyes"),
                ("Fungus2_15_boss", "Mantis Battle/Battle Main/Mantis Lord"),
                ("Fungus2_32", "Ring Holder"),
                ("Fungus2_32", "Warrior/Ghost Warrior Hu"),
                ("Fungus3_23_boss", "Battle Scene/Wave 3/Mantis Traitor Lord"),
                ("Fungus3_archive_02", "Mega Jellyfish Multizaps"),
                ("Fungus3_archive_02_boss", "Mega Jellyfish"),
                ("GG_Hollow_Knight", "Battle Scene/Focus Blasts/HK Prime Blast"),
                ("GG_Hollow_Knight", "Battle Scene/HK Prime"),
                ("GG_Nosk_Hornet", "Battle Scene/Hornet Nosk"),
                ("GG_Painter", "Battle Scene/Sheo Boss"),
                ("Grimm_Main_Tent_boss", "Grimm Boss"),
                ("Grimm_Main_Tent_boss", "Grimm Spike Holder"),
                ("Grimm_Nightmare", "Grimm Control/Appear Scene/Title"),
                ("Grimm_Nightmare", "Grimm Control/Appear Scene/black_fader"),
                ("Grimm_Nightmare", "Grimm Control/Nightmare Grimm Boss"),
                ("Hive_05", "Battle Scene"),
                ("Mines_18", "Laser Turret Mega (1)"),
                ("Mines_32", "Battle Scene/Zombie Beam Miner Rematch"),
                ("Room_Final_Boss_Core", "Boss Control/Hollow Knight Boss"),
                ("Ruins1_24_boss", "Mage Lord"),
                ("Ruins1_31b", "Battle Scene v2/Balloons/Mage Balloon Spawner"),
                ("Ruins2_11_boss", "Battle Scene/Jar Collector"),
                ("Waterways_12_boss", "Hatcher Cage (2)/Fluke Fly Spawner")
            ];
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects) {
            AssignPrefabs(preloadedObjects);
            RandoInterop.Hook();
        }

        private void AssignPrefabs(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects) {
            Modules.NoskBossModule.titleFaderPrefab = preloadedObjects["Grimm_Nightmare"]["Grimm Control/Appear Scene/black_fader"];
            Modules.NoskBossModule.titlePrefab = preloadedObjects["Grimm_Nightmare"]["Grimm Control/Appear Scene/Title"];
            Modules.NoskBossModule.wingedNoskPrefab = preloadedObjects["GG_Nosk_Hornet"]["Battle Scene/Hornet Nosk"];
            BossAttacks.BrokenVessel.vesselPrefab = preloadedObjects["Abyss_19"]["Infected Knight"];
            BossAttacks.CGuardian.turretPrefab = preloadedObjects["Mines_18"]["Laser Turret Mega (1)"];
            BossAttacks.Collector.collectorPrefab = preloadedObjects["Ruins2_11_boss"]["Battle Scene/Jar Collector"];
            BossAttacks.EGuardian.egPrefab = preloadedObjects["Mines_32"]["Battle Scene/Zombie Beam Miner Rematch"];
            BossAttacks.ElderHu.huPrefab = preloadedObjects["Fungus2_32"]["Warrior/Ghost Warrior Hu"];
            BossAttacks.ElderHu.ringHolderPrefab = preloadedObjects["Fungus2_32"]["Ring Holder"];
            BossAttacks.FalseKnight.summonPrefab = preloadedObjects["Crossroads_10_boss"]["Battle Scene/FK Barrel Summon"];
            BossAttacks.Flukemarm.flyPrefab = preloadedObjects["Waterways_12_boss"]["Hatcher Cage (2)/Fluke Fly Spawner"];
            BossAttacks.Galien.hammerPrefab = preloadedObjects["Deepnest_40"]["Warrior/Galien Hammer"];
            BossAttacks.Gorb.slugPrefab = preloadedObjects["Cliffs_02_boss"]["Warrior/Ghost Warrior Slug"];
            BossAttacks.GPZote.zotelingPrefab = preloadedObjects["Dream_Mighty_Zote"]["Zoteling"];
            BossAttacks.Grimm.grimmPrefab = preloadedObjects["Grimm_Main_Tent_boss"]["Grimm Boss"];
            BossAttacks.Grimm.spikeHolderPrefab = preloadedObjects["Grimm_Main_Tent_boss"]["Grimm Spike Holder"];
            BossAttacks.HiveKnight.battleScenePrefab = preloadedObjects["Hive_05"]["Battle Scene"];
            BossAttacks.HollowKnight.thkPrefab = preloadedObjects["Room_Final_Boss_Core"]["Boss Control/Hollow Knight Boss"];
            BossAttacks.HornetP.hornetPrefab = preloadedObjects["Fungus1_04_boss"]["Hornet Boss 1"];
            BossAttacks.HornetS.barbRegionPrefab = preloadedObjects["Deepnest_East_Hornet"]["Barb Region"];
            BossAttacks.HornetS.hornetPrefab = preloadedObjects["Deepnest_East_Hornet_boss"]["Hornet Boss 2"];
            BossAttacks.MantisLords.lordPrefab = preloadedObjects["Fungus2_15_boss"]["Mantis Battle/Battle Main/Mantis Lord"];
            BossAttacks.Markoth.markothPrefab = preloadedObjects["Deepnest_East_10"]["Warrior/Ghost Warrior Markoth"];
            BossAttacks.NightmareKing.nkgPrefab = preloadedObjects["Grimm_Nightmare"]["Grimm Control/Nightmare Grimm Boss"];
            BossAttacks.NoEyes.noEyesPrefab = preloadedObjects["Fungus1_35"]["Warrior/Ghost Warrior No Eyes"];
            BossAttacks.PureVessel.focusPrefab = preloadedObjects["GG_Hollow_Knight"]["Battle Scene/Focus Blasts/HK Prime Blast"];
            BossAttacks.PureVessel.pvPrefab = preloadedObjects["GG_Hollow_Knight"]["Battle Scene/HK Prime"];
            BossAttacks.Sheo.sheoPrefab = preloadedObjects["GG_Painter"]["Battle Scene/Sheo Boss"];
            BossAttacks.SoulMaster.mageLordPrefab = preloadedObjects["Ruins1_24_boss"]["Mage Lord"];
            BossAttacks.SoulWarrior.spawnerPrefab = preloadedObjects["Ruins1_31b"]["Battle Scene v2/Balloons/Mage Balloon Spawner"];
            BossAttacks.TraitorLord.tLordPrefab = preloadedObjects["Fungus3_23_boss"]["Battle Scene/Wave 3/Mantis Traitor Lord"];
            BossAttacks.Uumuu.multiZapPrefab = preloadedObjects["Fungus3_archive_02"]["Mega Jellyfish Multizaps"];
            BossAttacks.Uumuu.jellyPrefab = preloadedObjects["Fungus3_archive_02_boss"]["Mega Jellyfish"];
            BossAttacks.VengeflyKing.vfkPrefab = preloadedObjects["Fungus1_20_v02"]["_Enemies/Giant Buzzer"];
            BossAttacks.WDefender.defenderPrefab = preloadedObjects["Dream_04_White_Defender"]["White Defender"];
        }
    }
}