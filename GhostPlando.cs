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
                ("Fungus1_04_boss", "Hornet Boss 1/Sphere Ball"),
                ("Fungus1_04_boss", "Hornet Boss 1/Needle"),
                ("Ruins1_24_boss", "Mage Lord"),
                ("Cliffs_02_boss", "Warrior/Ghost Warrior Slug"),
                ("Fungus3_archive_02_boss", "Mega Jellyfish"),
                ("GG_Painter", "Battle Scene/Sheo Boss"),
                ("Ruins2_11_boss", "Battle Scene/Jar Collector")
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
            BossAttacks.HornetP.sphereBallPrefab = preloadedObjects["Fungus1_04_boss"]["Hornet Boss 1/Sphere Ball"];
            BossAttacks.HornetP.needlePrefab = preloadedObjects["Fungus1_04_boss"]["Hornet Boss 1/Needle"];
            BossAttacks.SoulMaster.mageLordPrefab = preloadedObjects["Ruins1_24_boss"]["Mage Lord"];
            BossAttacks.Gorb.slugPrefab = preloadedObjects["Cliffs_02_boss"]["Warrior/Ghost Warrior Slug"];
            BossAttacks.Uumuu.jellyPrefab = preloadedObjects["Fungus3_archive_02_boss"]["Mega Jellyfish"];
            BossAttacks.Sheo.sheoPrefab = preloadedObjects["GG_Painter"]["Battle Scene/Sheo Boss"];
            BossAttacks.Collector.collectorPrefab = preloadedObjects["Ruins2_11_boss"]["Battle Scene/Jar Collector"];
        }
    }
}