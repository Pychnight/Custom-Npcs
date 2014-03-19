using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TShockAPI.DB;
using TShockAPI;
using Terraria;

namespace CustomNPC
{
    public class CustomNPCVars
    {
        public CustomNPCDefinition customNPC { get; set; }
        public DateTime lastAttemptedProjectile { get; set; }
        public bool isDead { get; set; }
        public NPC mainNPC { get; set; } 

        public CustomNPCVars(CustomNPCDefinition customnpc, DateTime lastattemptedprojectile, NPC mainnpc, bool isdead = false)
        {
            lastAttemptedProjectile = lastattemptedprojectile;
            isDead = isdead;
            customNPC = customnpc;
            mainNPC = mainnpc;
        }
    }
}
