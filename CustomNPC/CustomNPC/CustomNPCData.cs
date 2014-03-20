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
    }
}
