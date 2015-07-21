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

namespace DebugNPC
{
    public class DebugNPCPlugin : NPCPlugin
    {
        public DebugNPCPlugin(IEventRegister register, DefinitionManager definitions)
            : base(register, definitions)
        {
        }

        public override string Name
        {
            get { return "Debug NPCs"; }
        }

        public override string[] Authors
        {
            get { return new[] { "Pychnight" }; }
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
            Register.RegisterHandler<NpcKilledEvent>(this, OnNpcKill, EventType.NpcKill);

            Definitions.Add(new DebugEventsNPCDefinition());
            Definitions.Add(new DebugLootNPCDefinition());
        }

    private void OnNpcDamage(NpcDamageEvent args)
    {
        	var npc = NPCManager.GetCustomNPCByIndex(args.NpcIndex);
            if (npc == null)
                return;


            switch (npc.customNPC.customID.ToUpper())
            {

                    case "debugevents":
                    {
                     Console.WriteLine("Damaged Custom Monster debugevents");
                    }
                    break;           
            }
        }        

    private void OnNpcCollision(NpcCollisionEvent args)
    {
        var npc = NPCManager.GetCustomNPCByIndex(args.NpcIndex);
        if (npc == null)
            return;


        switch (npc.customNPC.customID.ToUpper())
        {

            case "debugevents":
                {
                    Console.WriteLine("Collision Custom Monster debugevents");
                }
                break;
        }
    }

    private void OnNpcKill(NpcKilledEvent args)
        {
            var npc = NPCManager.GetCustomNPCByIndex(args.NpcIndex);
            if (npc == null)
                return;


            switch (npc.customNPC.customID.ToUpper())
            {

                    case "debugevents":
                    {
                     Console.WriteLine("Killed Custom Monster debugevents");
                    }
                    break;           
            }
        }        

    private void OnNpcUpdate(NpcUpdateEvent args)
    {
            var npc = NPCManager.GetCustomNPCByIndex(args.NpcIndex);
            if (npc == null)
                return;

            switch (npc.customNPC.customID.ToUpperInvariant())
            {
                case "debugevents":
                    {
                        if (NPCManager.HealthBelow(args.NpcIndex, 40))
                        {
                            npc.SelfHealing(200);
                        }
                    }
                    break;

            }
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
