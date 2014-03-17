using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomNPC
{
    internal class CustomNPC
    {
        /// <summary>
        /// NPC Variable Customizations
        /// </summary>
        internal string customName { get; set; }
        internal int customHealth { get; set; }
        internal int customID { get; set; }
        internal int customDefense { get; set; }
        internal int customSpeed { get; set; }
        internal int customAI { get; set; }
        internal bool isBoss { get; set; }
        internal bool noGravity { get; set; }
        internal bool noTileCollide { get; set; }
        internal bool lavaImmune { get; set; }

        /// <summary>
        /// NPC Restrictions/Conditions
        /// </summary>
        internal List<string> customBiomeSpawn { get; set; }
        internal List<string> customRegionSpawn { get; set; }

        /// <summary>
        /// NPC MISC
        /// </summary>
        internal List<int> customAreaDebuff { get; set; }
        //internal CustomParticle customParticle = new CustomParticle(); 
        internal int customSpawnTimer { get; set; }
        internal List<CustomNPCProjectiles> customProjectiles { get; set; }
        internal int customSpawnChance { get; set; }

    }
}
