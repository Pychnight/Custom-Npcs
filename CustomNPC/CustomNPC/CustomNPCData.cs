using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;
using TShockAPI;
using TShockAPI.DB;

namespace CustomNPC
{
    public class CustomNPCData 
    {
        internal Dictionary<string, CustomNPCDefinition> CustomNPCs = new Dictionary<string, CustomNPCDefinition>();
        internal Dictionary<BiomeTypes, List<string>> BiomeSpawns = new Dictionary<BiomeTypes, List<string>>();
        internal Dictionary<Region, List<string>> RegionSpawns = new Dictionary<Region,List<string>>();
        internal Dictionary<string, DateTime> LastSpawnAttempt = new Dictionary<string, DateTime>();

        /// <summary>
        /// This is called once the CustomNPCDefinitions have been loaded into the DefinitionManager.
        /// </summary>
        /// <param name="definitions"></param>
        internal void Load(DefinitionManager definitions)
        {
            foreach (var pair in definitions.Definitions)
            {
                string id = pair.Key;
                CustomNPCDefinition definition = pair.Value;

                CustomNPCs.Add(id, definition);

                AddCustomNPCToBiome(definition.customBiomeSpawn, definition.customID);

                if (definition.customRegionSpawn != null)
                {
                    foreach (string regionName in definition.customRegionSpawn)
                    {
                        AddCustomNPCToRegion(regionName, definition.customID);
                    }
                }
            }
        }

        /// <summary>
        /// Returns CustomNPC obj by custom ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CustomNPCDefinition GetNPCbyID(string id)
        {
            foreach (CustomNPCDefinition obj in CustomNPCs.Values)
            {
                if (obj.customID == id)
                {
                    return obj;
                }
            }
            return null;
        }

        /// <summary>
        /// Customize NPC to Customized NPC specifications
        /// </summary>
        /// <param name="index"></param>
        /// <param name="obj"></param>
        public void ConvertNPCToCustom(int index, CustomNPCDefinition obj)
        {
            NPC npc = Main.npc[index];
            npc.netDefaults(obj.customBase.netID);
            //npc.type = obj.customBaseID;

            npc.life = obj.customHealth;
            npc.name = obj.customName;
            npc.displayName = obj.customName;
            npc.lifeMax = obj.customHealth;
            npc.life = obj.customHealth;
            npc.aiStyle = obj.customAI;
            npc.lavaImmune = obj.lavaImmune;
            npc.noGravity = obj.noGravity;
            npc.noTileCollide = obj.noTileCollide;

            if (obj.customSpawnMessage != "")
            {
                TSPlayer.All.SendMessage(obj.customSpawnMessage, Color.Green);
            }
        }

        private void AddCustomNPCToBiome(BiomeTypes biome, string id)
        {
            List<string> spawns;
            if (!BiomeSpawns.TryGetValue(biome, out spawns))
            {
                spawns = new List<string>();
                BiomeSpawns[biome] = spawns;
            }

            if (!spawns.Contains(id))
            {
                spawns.Add(id);
            }
        }

        private void AddCustomNPCToRegion(string regionName, string id)
        {
            Region region = TShock.Regions.GetRegionByName(regionName);
            if (region == null)
                return;

            List<string> spawns;
            if (!RegionSpawns.TryGetValue(region, out spawns))
            {
                spawns = new List<string>();
                RegionSpawns[region] = spawns;
            }

            if (!spawns.Contains(id))
            {
                spawns.Add(id);
            }
        }
    }
}
