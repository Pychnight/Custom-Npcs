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
            Register.RegisterHandler<NpcCollisionEvent>(this, OnNpcCollision, EventType.NpcCollision);
            Register.RegisterHandler<PluginUpdateEvent>(this, OnPreUpdate, EventType.PluginUpdate);

            // add new npc definitions here
            Definitions.Add(new TestNPCDefinition());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Register.DeregisterHandler(this, EventType.NpcDamage);
                Register.DeregisterHandler(this, EventType.NpcCollision);
                Register.DeregisterHandler(this, EventType.PluginUpdate);
            }
        }

        private void OnPreUpdate(PluginUpdateEvent args)
        {
            
        }

        private void OnNpcDamage(NpcDamageEvent args)
        {

        }

        private void OnNpcCollision(NpcCollisionEvent args)
        {
            throw new NotImplementedException();
        }
    }
}
