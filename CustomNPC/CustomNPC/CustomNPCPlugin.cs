using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomNPC.Plugins;
using Terraria;
using TerrariaApi.Server;
using System.Timers;

namespace CustomNPC
{
    [ApiVersion(1, 15)]
    public class CustomNPCPlugin : TerrariaPlugin
    {
        internal static CustomNPCUtils CustomNPCUtils = CustomNPCUtils.Instance;
        internal CustomNPC[] CustomNPCs = new CustomNPC[200];
        internal CustomNPCData CustomNPCData = new CustomNPCData();

        //16.66 milliseconds for 1/60th of a second.
        private Timer mainLoop = new Timer(1000 / 60.0);
        private AppDomain pluginDomain;
        private PluginManager<NPCPlugin> pluginManager; 

        public CustomNPCPlugin(Main game)
            : base(game)
        {
            pluginDomain = CreateNewPluginDomain();
            pluginManager = pluginDomain.CreateInstanceAndUnwrap<PluginManager<NPCPlugin>>();
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
            pluginManager.Load(pluginDomain);

            //one OnUpdate is needed for replacement of mobs
            ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);
            ServerApi.Hooks.NpcLootDrop.Register(this, OnLootDrop);
        }

        private void OnLootDrop(NpcLootDropEventArgs args)
        {
            throw new NotImplementedException();
        }

        private void OnInitialize(EventArgs args)
        {
            mainLoop.Elapsed += mainLoop_Elapsed;
            mainLoop.Enabled = true;
        }

        private void OnUpdate(EventArgs args)
        {
            foreach(NPC obj in Main.npc)
            {
                //Search all CustomNPC and return first instance of replacement. - Incase of multiple defined replacements
            }
        }

        /// <summary>
        /// Do Everything here
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mainLoop_Elapsed(object sender, ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                pluginManager.Unload();
                AppDomain.Unload(pluginDomain);

                ServerApi.Hooks.GameUpdate.Deregister(this, OnUpdate);
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
                ServerApi.Hooks.NpcLootDrop.Deregister(this, OnLootDrop);
            }
        }

        private AppDomain CreateNewPluginDomain()
        {
            AppDomainSetup info = new AppDomainSetup
            {
                // this allows the replacement of plugin files in the file system
                ShadowCopyFiles = bool.TrueString
            };

            return AppDomain.CreateDomain("Plugin Domain", AppDomain.CurrentDomain.Evidence, info);
        }
    }
}
