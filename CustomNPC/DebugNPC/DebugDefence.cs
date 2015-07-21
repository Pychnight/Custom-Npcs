using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomNPC;
using Terraria;

namespace DebugNPC
{
    public sealed class DebugDefence : CustomNPCDefinition
    {
        public DebugDefence()
            : base(1)
        {

        }

        public override string customID
        {
            get { return "DEBUGDEF"; }
        }

        public override string customName
        {
            get { return "Slime with 100 Defence"; }
        }

        public override bool overrideBaseNPCLoot
        {
            get { return true; }
        }

        public override string customSpawnMessage
        {
            get
            {
                return null;
            }
        }

        public override bool isBoss
        {
            get
            {
                return true;
            }
        }

        public override int customDefense
        {
            get
            {
                return 100;
            }
        }

        public override int customHealth
        {
            get
            {
                return 50;
            }
        }
    }
}