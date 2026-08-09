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
            RandoInterop.DefineDoors();
        }
        
        public override List<(string, string)> GetPreloadNames() {
            return [
                ("Abyss_19", "Infected Knight"),
                ("Cliffs_02_boss", "Warrior/Ghost Warrior Slug"),
                ("Crossroads_10_boss", "Battle Scene/FK Barrel Summon"),
                ("Deepnest_East_10", "Warrior/Ghost Warrior Markoth"),
                ("Dream_04_White_Defender", "White Defender"),
                ("Fungus1_04_boss", "Hornet Boss 1"),
                ("Fungus2_15_boss", "Mantis Battle/Battle Main/Mantis Lord"),
                ("Fungus2_32", "Ring Holder"),
                ("Fungus2_32", "Warrior/Ghost Warrior Hu"),
                ("Fungus3_23_boss", "Battle Scene/Wave 3/Mantis Traitor Lord"),
                ("Fungus3_archive_02_boss", "Mega Jellyfish"),
                ("GG_Hollow_Knight", "Battle Scene/Focus Blasts/HK Prime Blast"),
                ("GG_Hollow_Knight", "Battle Scene/HK Prime"),
                ("GG_Nosk_Hornet", "Battle Scene/Hornet Nosk"),
                ("GG_Painter", "Battle Scene/Sheo Boss"),
                ("Grimm_Main_Tent_boss", "Grimm Boss"),
                ("Grimm_Main_Tent_boss", "Grimm Spike Holder"),
                ("Grimm_Nightmare", "Grimm Control/Appear Scene/Title"),
                ("Grimm_Nightmare", "Grimm Control/Appear Scene/black_fader"),
                ("Mines_18", "Laser Turret Mega (1)"),
                ("Mines_18_boss", "Mega Zombie Beam Miner (1)"),
                ("Room_Final_Boss_Core", "Boss Control/Hollow Knight Boss"),
                ("Ruins1_24_boss", "Mage Lord"),
                ("Ruins2_11_boss", "Battle Scene/Jar Collector")
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
            BossAttacks.CGuardian.cgPrefab = preloadedObjects["Mines_18_boss"]["Mega Zombie Beam Miner (1)"];
            BossAttacks.CGuardian.turretPrefab = preloadedObjects["Mines_18"]["Laser Turret Mega (1)"];
            BossAttacks.Collector.collectorPrefab = preloadedObjects["Ruins2_11_boss"]["Battle Scene/Jar Collector"];
            BossAttacks.ElderHu.huPrefab = preloadedObjects["Fungus2_32"]["Warrior/Ghost Warrior Hu"];
            BossAttacks.ElderHu.ringHolderPrefab = preloadedObjects["Fungus2_32"]["Ring Holder"];
            BossAttacks.FalseKnight.summonPrefab = preloadedObjects["Crossroads_10_boss"]["Battle Scene/FK Barrel Summon"];
            BossAttacks.Gorb.slugPrefab = preloadedObjects["Cliffs_02_boss"]["Warrior/Ghost Warrior Slug"];
            BossAttacks.Grimm.grimmPrefab = preloadedObjects["Grimm_Main_Tent_boss"]["Grimm Boss"];
            BossAttacks.Grimm.spikeHolderPrefab = preloadedObjects["Grimm_Main_Tent_boss"]["Grimm Spike Holder"];
            BossAttacks.HollowKnight.thkPrefab = preloadedObjects["Room_Final_Boss_Core"]["Boss Control/Hollow Knight Boss"];
            BossAttacks.HornetP.hornetPrefab = preloadedObjects["Fungus1_04_boss"]["Hornet Boss 1"];
            BossAttacks.MantisLords.lordPrefab = preloadedObjects["Fungus2_15_boss"]["Mantis Battle/Battle Main/Mantis Lord"];
            BossAttacks.Markoth.markothPrefab = preloadedObjects["Deepnest_East_10"]["Warrior/Ghost Warrior Markoth"];
            BossAttacks.PureVessel.focusPrefab = preloadedObjects["GG_Hollow_Knight"]["Battle Scene/Focus Blasts/HK Prime Blast"];
            BossAttacks.PureVessel.pvPrefab = preloadedObjects["GG_Hollow_Knight"]["Battle Scene/HK Prime"];
            BossAttacks.Sheo.sheoPrefab = preloadedObjects["GG_Painter"]["Battle Scene/Sheo Boss"];
            BossAttacks.SoulMaster.mageLordPrefab = preloadedObjects["Ruins1_24_boss"]["Mage Lord"];
            BossAttacks.TraitorLord.tLordPrefab = preloadedObjects["Fungus3_23_boss"]["Battle Scene/Wave 3/Mantis Traitor Lord"];
            BossAttacks.Uumuu.jellyPrefab = preloadedObjects["Fungus3_archive_02_boss"]["Mega Jellyfish"];
            BossAttacks.WDefender.defenderPrefab = preloadedObjects["Dream_04_White_Defender"]["White Defender"];
        }
    }
}