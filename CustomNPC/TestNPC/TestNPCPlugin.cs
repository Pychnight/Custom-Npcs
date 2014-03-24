using System;
using System.Collections.Generic;
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
            Register.RegisterHandler<NpcUpdateEvent>(this, OnNpcUpdate, EventType.NpcUpdate);

            // add new npc definitions here
            Definitions.Add(new TestNPCDefinition());
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Register.DeregisterHandler(this, EventType.NpcDamage);
                Register.DeregisterHandler(this, EventType.NpcCollision);
                Register.DeregisterHandler(this, EventType.NpcUpdate);
            }
        }

        private void OnNpcUpdate(NpcUpdateEvent args)
        {
            NPCManager.DebuffNearbyPlayers(80, args.NpxIndex, 100);
        }

        private void OnNpcDamage(NpcDamageEvent args)
        {
            NPCManager.AddBuffToPlayer(args.PlayerIndex, 20, 10);
        }

        private void OnNpcCollision(NpcCollisionEvent args)
        {
            NPCManager.AddBuffToPlayer(args.PlayerIndex, 24, 10);
        }
    }
}
