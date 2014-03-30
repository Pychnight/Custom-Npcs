using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomNPC;
using Terraria;

namespace TestNPC
{
    public sealed class TesterNPCDefinition : CustomNPCDefinition
    {
        public TesterNPCDefinition()
            : base(-2)
        {
            customNPCSpawning.Add(new CustomNPCSpawning(2, SpawnConditions.NightTime, true, BiomeTypes.None, "testregion", 100));
        }

        public override string customID
        {
            get { return "testnpc2"; }
        }

        public override string customName
        {
            get { return "Tester NPC"; }
        }

        public override int customAI
        {
            get
            {
                return 14;
            }
        }

        public override bool noGravity
        {
            get
            {
                return true;
            }
        }

        public override int customHealth
        {
            get
            {
                return 500;
            }
        }
    }
}
