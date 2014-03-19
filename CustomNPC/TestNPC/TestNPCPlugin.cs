using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomNPC;
using CustomNPC.EventSystem;
using CustomNPC.Plugins;

namespace TestNPC
{
    public class TestNPCPlugin : NPCPlugin
    {
        public TestNPCPlugin(IEventRegister register, DefinitionManager definitions)
            : base(register, definitions)
        {
        }

        public override string Name
        {
            get { return "Test NPCs"; }
        }

        public override string[] Authors
        {
            get { return new[] { "TheWanderer", "IcyPhoenix" }; }
        }

        public override Version Version
        {
            get { return new Version(0, 1); }
        }

        public override void Initialize()
        {
            // add new npc definitions here
            Definitions.Add(new TestNPCDefinition());
        }
    }
}
