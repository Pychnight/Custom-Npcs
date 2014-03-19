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
    public abstract class CustomNPCDefinition
    {
        /// <summary>
        /// NPC Variable Customizations
        /// </summary>
        public abstract string customName { get; }

        public abstract int customHealth { get; }
        public abstract string customID { get; }
        protected abstract int customDefense { get; }
        protected abstract int customSpeed { get; }
        public abstract int customAI { get; }
        protected abstract bool isBoss { get; }
        public abstract bool noGravity { get; }
        public abstract bool noTileCollide { get; }
        public abstract bool lavaImmune { get; }
        public abstract int customBaseID { get; }

        /// <summary>
        /// NPC Restrictions/Conditions
        /// </summary>
        protected abstract List<byte> customBiomeSpawn { get; }
        protected abstract List<string> customRegionSpawn { get; }
        public abstract DateTime lastAttemptedSpawn { get; set; } // shouldn't this be in a different place? CustomNPC maybe

        /// <summary>
        /// NPC MISC
        /// </summary>
        protected abstract List<int> customAreaDebuff { get; }
        //internal CustomParticle customParticle { get; set; }; 
        internal int customSpawnTimer { get; set; }
        internal List<CustomNPCProjectiles> customProjectiles { get; set; }
        internal double customSpawnChance { get; set; }
        internal string customSpawnMessage { get; set; }
                
    }
}
