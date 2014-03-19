using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomNPC;

namespace TestNPC
{
    public sealed class TestNPCDefinition : CustomNPCDefinition
    {
        public override string customName
        {
            get { return "Test NPC"; }
        }

        public override int customHealth
        {
            get { return 13000; }
        }

        public override string customID
        {
            get { return "testnpc"; }
        }

        protected override int customDefense
        {
            get { return 0; }
        }

        protected override int customSpeed
        {
            get { return 0; }
        }

        public override int customAI
        {
            get { return 1; }
        }

        protected override bool isBoss
        {
            get { return false; }
        }

        public override bool noGravity
        {
            get { return false; }
        }

        public override bool noTileCollide
        {
            get { return true; }
        }

        public override bool lavaImmune
        {
            get { return true; }
        }

        public override int customBaseID
        {
            get { return 1; }
        }

        protected override List<byte> customBiomeSpawn
        {
            get { throw new NotImplementedException(); }
        }

        protected override List<string> customRegionSpawn
        {
            get { throw new NotImplementedException(); }
        }

        public override DateTime lastAttemptedSpawn { get; set; }

        protected override List<int> customAreaDebuff
        {
            get { throw new NotImplementedException(); }
        }
    }
}
