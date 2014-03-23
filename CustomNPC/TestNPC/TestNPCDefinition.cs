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
            : base(21)
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

        public override IList<CustomNPCLoot> customNPCLoots
        {
            get
            {
                return new[]
                {
                    new CustomNPCLoot(1553, new List<int> { 83 }, 1, 50), 
                };
            }
        }

        public override int customAI
        {
            get
            {
                return 2;
            }
        }

        protected override List<int> customAreaDebuff
        {
            get { throw new NotImplementedException(); }
        }
    }
}
