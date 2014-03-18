using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Terraria;

namespace CustomNPC
{
    internal class CustomNPCData
    {
        internal Dictionary<string, CustomNPC> CustomNPCs = new Dictionary<string, CustomNPC>();

        /// <summary>
        /// Returns CustomNPC obj by custom ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal CustomNPC GetNPCbyID(string id)
        {
            foreach (CustomNPC obj in CustomNPCs.Values)
            {
                if (obj.customID == id)
                {
                    return obj;
                }
            }
            return null;
        }

        internal void ConvertNPCToCustom(int index, CustomNPC obj)
        {
            NPC npc = Main.npc[index];
            npc.netDefaults(obj.customBaseID);
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
        }
    }
}
