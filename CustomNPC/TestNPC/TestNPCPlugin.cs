using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomNPC;
using CustomNPC.EventSystem;
using CustomNPC.EventSystem.Events;
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
            Register.RegisterHandler<NpcDamageEvent>(this, OnNpcDamage, EventType.NpcDamage);

            // add new npc definitions here
            Definitions.Add(new TestNPCDefinition());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Register.DeregisterHandler(this, EventType.NpcDamage);
            }
        }

        private void OnNpcDamage(NpcDamageEvent args)
        {
            var npcvar = NPCManager.GetCustomNPCByIndex(args.NpcIndex);
            if (npcvar == null)
                return;

            if (npcvar.customNPC.customID == "testnpc" && npcvar.HealthBelow(200))
            {
                ////npcvar.Multiply(3);
            }
        }
    }
}
