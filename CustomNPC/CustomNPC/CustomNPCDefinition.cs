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
    public class CustomNPCDefinition
    {
        /// <summary>
        /// NPC Variable Customizations
        /// </summary>
        internal string customName { get; set; }
        internal int customHealth { get; set; }
        internal string customID { get; set; }
        internal int customDefense { get; set; }
        internal int customSpeed { get; set; }
        internal int customAI { get; set; }
        internal bool isBoss { get; set; }
        internal bool noGravity { get; set; }
        internal bool noTileCollide { get; set; }
        internal bool lavaImmune { get; set; }
        internal int customBaseID { get; set; }

        /// <summary>
        /// NPC Restrictions/Conditions
        /// </summary>
        internal List<byte> customBiomeSpawn { get; set; }
        internal List<string> customRegionSpawn { get; set; }
        internal DateTime lastAttemptedSpawn { get; set; }

        /// <summary>
        /// NPC MISC
        /// </summary>
        internal List<int> customAreaDebuff { get; set; }
        //internal CustomParticle customParticle { get; set; }; 
        internal int customSpawnTimer { get; set; }
        internal List<CustomNPCProjectiles> customProjectiles { get; set; }
        internal double customSpawnChance { get; set; }
        internal string customSpawnMessage { get; set; }
                
    }
}
