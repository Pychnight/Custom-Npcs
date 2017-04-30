using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomNPC;
using CustomNPC.EventSystem;
using CustomNPC.EventSystem.Events;
using CustomNPC.Plugins;
using Terraria;

namespace TestNPC
{
    public class TestNPCPlugin : NPCPlugin
    {
        //never changes execpt for "TestNPCPlugin"
        public TestNPCPlugin(IEventRegister register, DefinitionManager definitions)
            : base(register, definitions)
        {
        }
        //generic plugin name
        public override string Name
        {
            get { return "Test NPCs"; }
        }
        //generic plugin author
        public override string[] Authors
        {
            get { return new[] { "TheWanderer", "IcyPhoenix" }; }
        }
        //generic plugin version
        public override Version Version
        {
            get { return new Version(0, 1); }
        }

        //events are registered here
        public override void Initialize()
        {
            // add new npc definitions here
            Definitions.Add(new TestNPCDefinition());
        }

        //events are diposed of here
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }
        }
    }
}
