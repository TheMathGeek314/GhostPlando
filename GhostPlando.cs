using Modding;
using System.Collections.Generic;
using UnityEngine;

namespace GhostPlando {
    public class GhostPlando: Mod {
        new public string GetName() => "GhostPlando";
        public override string GetVersion() => "1.0.0.0";
        internal static GhostPlando Instance { get; private set; }

        public GhostPlando(): base("GhostPlando") {
            Instance = this;
            DevPrep.OnConstructor();
        }
        
        public override List<(string, string)> GetPreloadNames() {
            return [
                ("Grimm_Nightmare", "Grimm Control/Appear Scene/Title"),
                ("Grimm_Nightmare", "Grimm Control/Appear Scene/black_fader"),
                ("Crossroads_10_boss", "Battle Scene/FK Barrel Summon"),
                ("Fungus1_04_boss", "Hornet Boss 1"),
                ("Ruins1_24_boss", "Mage Lord"),
                ("Cliffs_02_boss", "Warrior/Ghost Warrior Slug"),
                ("Fungus3_archive_02_boss", "Mega Jellyfish"),
                ("GG_Painter", "Battle Scene/Sheo Boss"),
                ("Ruins2_11_boss", "Battle Scene/Jar Collector"),
                ("Abyss_19", "Infected Knight"),
                ("Dream_04_White_Defender", "White Defender"),
                ("Fungus2_15_boss", "Mantis Battle/Battle Main/Mantis Lord"),
                ("Fungus2_32", "Warrior/Ghost Warrior Hu"),
                ("Fungus2_32", "Ring Holder"),
                ("Grimm_Main_Tent_boss", "Grimm Boss"),
                ("Grimm_Main_Tent_boss", "Grimm Spike Holder"),
                ("GG_Hollow_Knight", "Battle Scene/HK Prime"),
                ("GG_Hollow_Knight", "Battle Scene/Focus Blasts/HK Prime Blast"),
                ("Mines_18", "Laser Turret Mega (1)"),
                ("Room_Final_Boss_Core", "Boss Control/Hollow Knight Boss"),
                ("Fungus3_23_boss", "Battle Scene/Wave 3/Mantis Traitor Lord")
            ];
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects) {
            AssignPrefabs(preloadedObjects);
            DevPrep.OnInitialize();
        }

        private void AssignPrefabs(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects) {
            Modules.NoskBossModule.titlePrefab = preloadedObjects["Grimm_Nightmare"]["Grimm Control/Appear Scene/Title"];
            Modules.NoskBossModule.titleFaderPrefab = preloadedObjects["Grimm_Nightmare"]["Grimm Control/Appear Scene/black_fader"];
            BossAttacks.FalseKnight.summonPrefab = preloadedObjects["Crossroads_10_boss"]["Battle Scene/FK Barrel Summon"];
            BossAttacks.HornetP.hornetPrefab = preloadedObjects["Fungus1_04_boss"]["Hornet Boss 1"];
            BossAttacks.SoulMaster.mageLordPrefab = preloadedObjects["Ruins1_24_boss"]["Mage Lord"];
            BossAttacks.Gorb.slugPrefab = preloadedObjects["Cliffs_02_boss"]["Warrior/Ghost Warrior Slug"];
            BossAttacks.Uumuu.jellyPrefab = preloadedObjects["Fungus3_archive_02_boss"]["Mega Jellyfish"];
            BossAttacks.Sheo.sheoPrefab = preloadedObjects["GG_Painter"]["Battle Scene/Sheo Boss"];
            BossAttacks.Collector.collectorPrefab = preloadedObjects["Ruins2_11_boss"]["Battle Scene/Jar Collector"];
            BossAttacks.BrokenVessel.vesselPrefab = preloadedObjects["Abyss_19"]["Infected Knight"];
            BossAttacks.WDefender.defenderPrefab = preloadedObjects["Dream_04_White_Defender"]["White Defender"];
            BossAttacks.MantisLords.lordPrefab = preloadedObjects["Fungus2_15_boss"]["Mantis Battle/Battle Main/Mantis Lord"];
            BossAttacks.ElderHu.huPrefab = preloadedObjects["Fungus2_32"]["Warrior/Ghost Warrior Hu"];
            BossAttacks.ElderHu.ringHolderPrefab = preloadedObjects["Fungus2_32"]["Ring Holder"];
            BossAttacks.Grimm.grimmPrefab = preloadedObjects["Grimm_Main_Tent_boss"]["Grimm Boss"];
            BossAttacks.Grimm.spikeHolderPrefab = preloadedObjects["Grimm_Main_Tent_boss"]["Grimm Spike Holder"];
            BossAttacks.PureVessel.pvPrefab = preloadedObjects["GG_Hollow_Knight"]["Battle Scene/HK Prime"];
            BossAttacks.PureVessel.focusPrefab = preloadedObjects["GG_Hollow_Knight"]["Battle Scene/Focus Blasts/HK Prime Blast"];
            BossAttacks.CGuardian.turretPrefab = preloadedObjects["Mines_18"]["Laser Turret Mega (1)"];
            BossAttacks.HollowKnight.thkPrefab = preloadedObjects["Room_Final_Boss_Core"]["Boss Control/Hollow Knight Boss"];
            BossAttacks.TraitorLord.tLordPrefab = preloadedObjects["Fungus3_23_boss"]["Battle Scene/Wave 3/Mantis Traitor Lord"];
        }
    }
}