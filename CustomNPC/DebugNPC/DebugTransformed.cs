using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomNPC;
using Terraria;

namespace DebugNPC
{
    public sealed class DebugTransformed : CustomNPCDefinition
    {
        public DebugTransformed()
            : base(3)
        {

            customProjectiles.Add(new CustomNPCProjectiles(3, new List<ShotTile>() { ShotTile.Middle }, 15, 250, true, 100,true));
        }

        public override string customID
        {
            get { return "DEBUGTRANSFORMED"; }
        }

        public override bool isBoss
        {
            get
            {
                return true;
            }
        }

        public override bool overrideBaseNPCLoot
        {
            get
            {
                return true;
            }
        }

        public override Color customSpawnMessageColor
        {
            get
            {
                return Color.Red;
            }
        }

        public override string customName
        {
            get { return "DEBUG TRANSFORMED"; }
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