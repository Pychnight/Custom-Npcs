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
        internal static DateTime[] LastMultiplyTimes = new DateTime[Main.maxNPCs];

        public TesterNPCDefinition()
            : base(-2)
        {
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

        public override void OnDeath(CustomNPCVars vars)
        {
            LastMultiplyTimes[vars.mainNPC.whoAmI] = default(DateTime);
        }
    }
}
