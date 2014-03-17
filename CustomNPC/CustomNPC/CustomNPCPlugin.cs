using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TerrariaApi.Server;
using System.Timers;

namespace CustomNPC
{
    [ApiVersion(1, 15)]
    public class CustomNPCPlugin : TerrariaPlugin
    {
        //16.66 milliseconds for 1/60th of a second rounded down.
        private Timer mainLoop = new Timer((1/60.0));
        internal static CustomNPCUtils CustomNPCUtils = CustomNPCUtils.Instance;
        internal NPC[] NPC = new NPC[200];

        public CustomNPCPlugin(Main game)
            : base(game)
        {

        }
        public override string Author
        {
            get { return "IcyGaming"; }
        }
        public override string Description
        {
            get { return "Create Custom NPCs"; }
        }

        public override string Name
        {
            get { return "CustomNPC"; }
        }

        public override Version Version
        {
            get { return new Version("0.1"); }
        }

        public override void Initialize()
        {
            //one OnUpdate is needed for replacement of mobs
            ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
        }

        private void OnUpdate(EventArgs args)
        {
            foreach(NPC obj in Main.npc)
            {
                //Search all CustomNPC and return first instance of replacement. - Incase of multiple defined replacements
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ServerApi.Hooks.GameUpdate.Deregister(this, OnUpdate);
            }
        }
    }
}
