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
        public TestNPCDefinition()
            : base(1)
        {
        }

        public override string customID
        {
            get { return "testnpc"; }
        }

        public override string customName
        {
            get { return "Test NPC"; }
        }

        protected override List<byte> customBiomeSpawn
        {
            get { throw new NotImplementedException(); }
        }

        protected override List<string> customRegionSpawn
        {
            get { throw new NotImplementedException(); }
        }

        protected override List<int> customAreaDebuff
        {
            get { throw new NotImplementedException(); }
        }
    }
}
