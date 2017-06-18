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
        internal Dictionary<BiomeTypes, List<Tuple<string, CustomNPCSpawning>>> BiomeSpawns = new Dictionary<BiomeTypes, List<Tuple<string, CustomNPCSpawning>>>();
        internal Dictionary<string, List<Tuple<string, CustomNPCSpawning>>> RegionSpawns = new Dictionary<string, List<Tuple<string, CustomNPCSpawning>>>();
        internal Dictionary<string, DateTime> LastSpawnAttempt = new Dictionary<string, DateTime>();

        /// <summary>
        /// This is called once the CustomNPCDefinitions have been loaded into the DefinitionManager.
        /// </summary>
        /// <param name="definitions"></param>
        internal void LoadFrom(DefinitionManager definitions)
        {
            foreach (var pair in definitions.Definitions)
            {
                string id = pair.Key;
                CustomNPCDefinition definition = pair.Value;

                CustomNPCs.Add(id, definition);
                foreach (var spawning in definition.customNPCSpawning)
                {
                    if (spawning.spawnBiome != BiomeTypes.None)
                    {
                        AddCustomNPCToBiome(spawning.spawnBiome, id, spawning);
                    }

                    if (!string.IsNullOrEmpty(spawning.spawnRegion))
                    {
                        AddCustomNPCToRegion(spawning.spawnRegion, id, spawning);
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
            CustomNPCDefinition npcdef;
            this.CustomNPCs.TryGetValue(id, out npcdef);
            return npcdef;
        }

        /// <summary>
        /// Customize NPC to Customized NPC specifications
        /// </summary>
        /// <param name="index"></param>
        /// <param name="obj"></param>
        /// <param name="netDef">If false, this method doesn't set netDefaults on the npc in question.</param>
        public void ConvertNPCToCustom(int index, CustomNPCDefinition obj, bool netDef = true)
        {
            NPC npc = Main.npc[index];
            if (netDef) npc.SetDefaults(obj.customBase.netID);

            npc._givenName = obj.customName;
            npc.lifeMax = obj.customHealth;
            npc.life = obj.customHealth + 1;
            npc.aiStyle = obj.customAI;
            npc.lavaImmune = obj.lavaImmune;
            npc.noGravity = obj.noGravity;
            npc.noTileCollide = obj.noTileCollide;

            npc.defense = obj.customDefense;
            npc.defDefense = obj.customDefense;

            npc.boss = obj.isBoss;

            if (!string.IsNullOrEmpty(obj.customSpawnMessage))
            {
                TSPlayer.All.SendMessage(obj.customSpawnMessage, obj.customSpawnMessageColor);
            }
        }

        private void AddCustomNPCToBiome(BiomeTypes biome, string id, CustomNPCSpawning spawning)
        {
            List<Tuple<string, CustomNPCSpawning>> spawns;
            if (!BiomeSpawns.TryGetValue(biome, out spawns))
            {
                spawns = new List<Tuple<string, CustomNPCSpawning>>();
                BiomeSpawns[biome] = spawns;
            }

            var pair = Tuple.Create(id, spawning);
            if (!spawns.Contains(pair))
            {
                spawns.Add(pair);
            }
        }

        private void AddCustomNPCToRegion(string regionName, string id, CustomNPCSpawning spawning)
        {
            List<Tuple<string, CustomNPCSpawning>> spawns;
            if (!RegionSpawns.TryGetValue(regionName, out spawns))
            {
                spawns = new List<Tuple<string, CustomNPCSpawning>>();
                RegionSpawns[regionName] = spawns;
            }

            var pair = Tuple.Create(id, spawning);
            if (!spawns.Contains(pair))
            {
                spawns.Add(pair);
            }
        }
    }
}
