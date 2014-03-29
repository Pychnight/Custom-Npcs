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
        private Random _random = new Random();
        private DateTime?[] _lastTestMultiply = new DateTime?[Main.maxNPCs];
        private DateTime?[] _lastTesterMultiply = new DateTime?[Main.maxNPCs];

        private string[] _transforms =
        {
            "testnpc", "testnpc2"
        };

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
            Register.RegisterHandler<NpcDamageEvent>(this, OnNpcDamage, EventType.NpcDamage);
            Register.RegisterHandler<NpcCollisionEvent>(this, OnNpcCollision, EventType.NpcCollision);
            Register.RegisterHandler<NpcUpdateEvent>(this, OnNpcUpdate, EventType.NpcUpdate);
            Register.RegisterHandler<NpcKilledEvent>(this, OnNpcKill, EventType.NpcKill);

            // add new npc definitions here
            Definitions.Add(new TestNPCDefinition());
            Definitions.Add(new TesterNPCDefinition());
        }

        //events are diposed of here
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Register.DeregisterHandler(this, EventType.NpcDamage);
                Register.DeregisterHandler(this, EventType.NpcCollision);
                Register.DeregisterHandler(this, EventType.NpcUpdate);
                Register.DeregisterHandler(this, EventType.NpcKill);
            }
        }

        //this update happens every 1/60th of a second
        private void OnNpcUpdate(NpcUpdateEvent args)
        {
            var npc = NPCManager.GetCustomNPCByIndex(args.NpcIndex);
            if (npc == null)
                return;

            switch (npc.customNPC.customID.ToLower())
            {
                case "testnpc":
                    lock (_lastTestMultiply)
                    {
                        if (_lastTestMultiply[args.NpcIndex] == null)
                        {
                            _lastTestMultiply[args.NpcIndex] = DateTime.Now;
                        }
                        else if ((DateTime.Now - _lastTestMultiply[args.NpcIndex]).Value.TotalMinutes >= 1)
                        {
                            npc.Multiply(_transforms[_random.Next(_transforms.Length)], 1);
                            _lastTestMultiply[args.NpcIndex] = DateTime.Now;
                        }
                    }
                    NPCManager.DebuffNearbyPlayers(80, args.NpcIndex, 100);
                    break;

                case "testnpc2":
                    lock (_lastTesterMultiply)
                    {
                        if (_lastTesterMultiply[args.NpcIndex] == null)
                        {
                            _lastTesterMultiply[args.NpcIndex] = DateTime.Now;
                        }
                        else if ((DateTime.Now - _lastTesterMultiply[args.NpcIndex].Value).TotalMinutes >= 2)
                        {
                            npc.Multiply("testnpc", 1);
                            _lastTesterMultiply[args.NpcIndex] = DateTime.Now;
                        }
                    }
                    break;
            }
        }

        private void OnNpcKill(NpcKilledEvent args)
        {
            _lastTestMultiply[args.NpcIndex] = default(DateTime);
            _lastTesterMultiply[args.NpcIndex] = default(DateTime);
        }

        //everytime the npc gets damaged
        private void OnNpcDamage(NpcDamageEvent args)
        {
            var npc = NPCManager.GetCustomNPCByIndex(args.NpcIndex);
            if (npc == null)
                return;

            switch (npc.customNPC.customID.ToLower())
            {
                case "testnpc":
                    NPCManager.AddBuffToPlayer(args.PlayerIndex, 20, 10);
                    break;
            }
        }

        //everytime someone collides with an npc
        private void OnNpcCollision(NpcCollisionEvent args)
        {
            var npc = NPCManager.GetCustomNPCByIndex(args.NpcIndex);
            if (npc == null)
                return;

            switch (npc.customNPC.customID.ToLower())
            {
                case "testnpc":
                    NPCManager.AddBuffToPlayer(args.PlayerIndex, 24, 10);
                    break;
            }
        }
    }
}
