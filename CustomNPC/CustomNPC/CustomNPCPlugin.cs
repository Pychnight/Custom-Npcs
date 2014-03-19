using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Streams;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomNPC.EventSystem;
using CustomNPC.EventSystem.Events;
using CustomNPC.Plugins;
using Terraria;
using TerrariaApi.Server;
using System.Timers;
using TShockAPI;
using TShockAPI.DB;

namespace CustomNPC
{
    [ApiVersion(1, 15)]
    public class CustomNPCPlugin : TerrariaPlugin
    {
        internal static CustomNPCUtils CustomNPCUtils = CustomNPCUtils.Instance;
        internal CustomNPCVars[] CustomNPCs = new CustomNPCVars[200];
        internal CustomNPCData CustomNPCData = new CustomNPCData();

        //16.66 milliseconds for 1/60th of a second.
        private Timer mainLoop = new Timer(1000 / 60.0);
        private EventManager eventManager;
        private DefinitionManager definitionManager;
        private AppDomain pluginDomain;
        private PluginManager<NPCPlugin> pluginManager;

        public CustomNPCPlugin(Main game)
            : base(game)
        {
            eventManager = new EventManager();
            definitionManager = new DefinitionManager();

            pluginDomain = CreateNewPluginDomain();
            pluginManager = pluginDomain.CreateInstanceAndUnwrap<PluginManager<NPCPlugin>>(eventManager, definitionManager);
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

            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);

            //one OnUpdate is needed for replacement of mobs
            ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
            ServerApi.Hooks.NpcLootDrop.Register(this, OnLootDrop);
        }

        /// <summary>
        /// For Custom Loot
        /// </summary>
        /// <param name="args"></param>
        private void OnLootDrop(NpcLootDropEventArgs args)
        {

        }

        private void OnInitialize(EventArgs args)
        {
            mainLoop.Elapsed += mainLoop_Elapsed;
            mainLoop.Enabled = true;
        }

        /// <summary>
        /// Adds Custom Monster to array for replacement
        /// </summary>
        /// <param name="args"></param>
        private void OnUpdate(EventArgs args)
        {
            foreach (NPC obj in Main.npc)
            {
                foreach (CustomNPCDefinition customnpc in CustomNPCData.CustomNPCs.Values)
                {
                    if (obj.netID == customnpc.customBaseID && this.CustomNPCs[obj.whoAmI] == null)
                    {
                        this.CustomNPCs[obj.whoAmI] = new CustomNPCVars(customnpc, DateTime.Now, obj);
                        CustomNPCData.ConvertNPCToCustom(obj.whoAmI, customnpc);
                    }
                }
            }
        }

        private void OnGetData(GetDataEventArgs args)
        {
            if (args.Handled)
                return;

            PacketTypes type = args.MsgID;
            TSPlayer player = TShock.Players[args.Msg.whoAmI];
            if (player == null || !player.ConnectionAlive)
                return;

            using (var data = new MemoryStream(args.Msg.readBuffer, args.Index, args.Length))
            {
                switch (type)
                {
                    case PacketTypes.NpcStrike:
                        OnNpcDamaged(new GetDataHandlerArgs(player, data));
                        break;
                }
            }
        }

        #region Event Dispatchers

        private void OnNpcDamaged(GetDataHandlerArgs args)
        {
            int npcIndex = args.Data.ReadInt16();
            int damage = args.Data.ReadInt16();
            float knockback = args.Data.ReadSingle();
            byte direction = args.Data.ReadInt8();
            bool critical = args.Data.ReadBoolean();

            var e = new NpcDamageEvent
            {
                NpcIndex = npcIndex,
                PlayerIndex = args.Player.Index,
                Damage = damage,
                Knockback = knockback,
                Direction = direction,
                CriticalHit = critical
            };

            eventManager.InvokeHandler(e, EventType.NpcDamage);
        }

        #endregion

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

        /// <summary>
        /// Do Everything here
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mainLoop_Elapsed(object sender, ElapsedEventArgs e)
        {
            //Spawn mobs into regions and specific biomes
            SpawnMobsInBiomeAndRegion();
            //check if NPC has been deactivated (could mean NPC despawned)
            CheckActiveNPCs();
        }

        private void CheckActiveNPCs()
        {
            foreach (CustomNPCVars obj in this.CustomNPCs)
            {
                //if CustomNPC has been defined, and hasn't been set to dead yet, check if the terraria npc is active
                if (obj != null && !obj.isDead && !obj.mainNPC.active)
                {
                    obj.isDead = true;
                }
            }
        }

        private void SpawnMobsInBiomeAndRegion()
        {
            foreach(TSPlayer player in TShock.Players)
            {
                if (player != null && player.ConnectionAlive)
                {
                    //get list of mobs that can be spawned in that biome
                    foreach (string str in CustomNPCData.BiomeSpawns[CustomNPCUtils.PlayersCurrBiome(player)])
                    {
                        CustomNPCDefinition customnpc = CustomNPCData.GetNPCbyID(str);
                        if ((DateTime.Now - CustomNPCData.LastSpawnAttempt[customnpc.customID]).TotalSeconds >= customnpc.customSpawnTimer)
                        {
                            CustomNPCData.LastSpawnAttempt[customnpc.customID] = DateTime.Now;
                            if (CustomNPCUtils.Chance(customnpc.customSpawnChance))
                            {
                                int spawnX;
                                int spawnY;
                                TShock.Utils.GetRandomClearTileWithInRange(player.TileX, player.TileY, 50, 50, out spawnX, out spawnY);
                                SpawnMobsInStaticLocation(spawnX, spawnY, customnpc);
                            }
                        }
                    }
                    //then check regions as well
                    List<Region> playersInRegion = CustomNPCData.RegionSpawns.Keys.ToList().FindAll(region => region.InArea(new Rectangle((int)(player.TileX), (int)(player.TileY), player.TPlayer.width, player.TPlayer.height)));
                    foreach (Region obj in playersInRegion)
                    {
                        List<string> customNpcSpawns = CustomNPCData.RegionSpawns[obj];
                        foreach (string str in customNpcSpawns)
                        {
                            CustomNPCDefinition customnpc = CustomNPCData.GetNPCbyID(str);
                            if ((DateTime.Now - CustomNPCData.LastSpawnAttempt[customnpc.customID]).TotalSeconds >= customnpc.customSpawnTimer)
                            {
                                CustomNPCData.LastSpawnAttempt[customnpc.customID] = DateTime.Now;
                                if (CustomNPCUtils.Chance(customnpc.customSpawnChance))
                                {
                                    int spawnX;
                                    int spawnY;
                                    TShock.Utils.GetRandomClearTileWithInRange(player.TileX, player.TileY, 50, 50, out spawnX, out spawnY);
                                    SpawnMobsInStaticLocation(spawnX, spawnY, customnpc);
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Temporary function - until we get Terraria's code
        /// </summary>
        private void SpawnMobsInStaticLocation(int x, int y, CustomNPCDefinition customnpc)
        {
            int npcid = NPC.NewNPC(x, y, customnpc.customBaseID);
            this.CustomNPCs[npcid] = new CustomNPCVars(customnpc, DateTime.Now, Main.npc[npcid]);
        }
    }
}
