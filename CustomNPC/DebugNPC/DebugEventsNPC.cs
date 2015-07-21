using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomNPC;

namespace DebugNPC
{
    public sealed class DebugEventsNPCDefinition : CustomNPCDefinition
    {
        public DebugEventsNPCDefinition()
            //define base id type for Custom NPC
            : base(3)
        {
            customNPCSpawning.Add(new CustomNPCSpawning(1, SpawnConditions.Raining | SpawnConditions.DayTime, true, BiomeTypes.None, "test", 100.0));
        }

        //ID of Custom NPC - can be set to anything, this will be what is used to summon the npc in game ie/ c1, c2, c3 etc...
        public override string customID
        {
            get { return "debugevents"; }
        }

        public override string customSpawnMessage
        {
            get
            {
                return "Debug Events NPC";
            }
        }

        //Name of NPC - will display for namable NPCs
        public override string customName
        {
            get { return "Debug Events"; }
        }


        public override int customHealth
        {
            get
            {
                return 120;
            }
        }

        //On death 35% chance of multiplying into 3
        public override void OnDeath(CustomNPCVars vars)
        {
          
        }
       
    }
}
