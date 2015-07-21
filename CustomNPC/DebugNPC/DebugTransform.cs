using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomNPC;
using Terraria;

namespace DebugNPC
{
    public sealed class DebugTransform : CustomNPCDefinition
    {
        public DebugTransform()
            : base(1)
        {
        }

        public override string customID
        {
            get { return "DEBUGTRANSFORM"; }
        }

        public override string customName
        {
            get { return "DEBUG TRANSFORM"; }
        }

        public override int customHealth
        {
            get
            {
                return 120;
            }
        }
    }
}