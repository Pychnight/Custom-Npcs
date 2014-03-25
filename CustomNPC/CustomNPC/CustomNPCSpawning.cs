using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TShockAPI.DB;
using TShockAPI;

namespace CustomNPC
{
    class CustomNPCSpawning
    {
        public BiomeTypes spawnBiome { get; set; }
        public Region spawnRegion { get; set; }
        public int spawnRate { get; set; }
        public double spawnChance { get; set; }
        public List<SpawnConditions> spawnConditions { get; set; }
        public CustomNPCSpawning(BiomeTypes spawnbiome, int spawnrate, List<SpawnConditions> spawnconditions, string spawnregion = "", double spawnchance = 100.0)
        {
            Region region = TShock.Regions.GetRegionByName(spawnregion);
            if (region == null)
            {
                Log.ConsoleError("[CustomNPC]: Error Region \"{1}\" does not exist!", spawnregion);
            }
            spawnBiome = spawnbiome;
            spawnRegion = region;
            spawnRate = spawnrate;
            spawnChance = spawnchance;
            spawnConditions = spawnconditions;
        }
    }
}
