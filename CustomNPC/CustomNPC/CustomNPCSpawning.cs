using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TShockAPI.DB;
using TShockAPI;

namespace CustomNPC
{
    public class CustomNPCSpawning
    {
        public BiomeTypes spawnBiome { get; set; }
        public string spawnRegion { get; set; }
        public int spawnRate { get; set; }
        public double spawnChance { get; set; }
        public List<SpawnConditions> spawnConditions { get; set; }
        public CustomNPCSpawning(int spawnrate, List<SpawnConditions> spawnconditions, BiomeTypes spawnbiome = BiomeTypes.None, string spawnregion = "", double spawnchance = 100.0)
        {
            spawnBiome = spawnbiome;
            spawnRegion = spawnregion;
            spawnRate = spawnrate;
            spawnChance = spawnchance;
            spawnConditions = spawnconditions;
        }
    }
}
