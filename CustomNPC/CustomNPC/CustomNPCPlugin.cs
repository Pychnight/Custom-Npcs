using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Streams;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using CustomNPC.EventSystem;
using CustomNPC.EventSystem.Events;
using CustomNPC.Plugins;
using Terraria;
using TerrariaApi.Server;
using TShockAPI;
using TShockAPI.DB;

namespace CustomNPC
{
    [ApiVersion(1, 15)]
    public class CustomNPCPlugin : TerrariaPlugin
    {
        internal Random rand = new Random();
        public CustomNPCConfig ConfigObj { get; set; }
        private String SavePath = TShock.SavePath;
        internal static string filepath { get { return Path.Combine(TShock.SavePath, "customnpcinvasion.json"); } }

        //16.66 milliseconds for 1/60th of a second.
        private Timer mainLoop = new Timer(1000 / 60.0);
        private EventManager eventManager;
        private DefinitionManager definitionManager;
#if USE_APPDOMAIN
        private AppDomain pluginDomain;
#endif
        private PluginManager<NPCPlugin> pluginManager;

        public CustomNPCPlugin(Main game)
            : base(game)
        {
            eventManager = new EventManager();
            definitionManager = new DefinitionManager();

#if USE_APPDOMAIN
            pluginDomain = CreateNewPluginDomain();
#endif
            ////pluginDomain.AssemblyResolve += PluginDomain_OnAssemblyResolve;
/*
            foreach (AssemblyName asm in Assembly.GetExecutingAssembly().GetReferencedAssemblies())
            {
                pluginDomain.Load(asm);
            }

            pluginDomain.Load(Assembly.GetExecutingAssembly().GetName());
*/

#if USE_APPDOMAIN
            pluginManager = pluginDomain.CreateInstanceAndUnwrap<PluginManager<NPCPlugin>>(eventManager, definitionManager);
#else
            pluginManager = new PluginManager<NPCPlugin>(eventManager, definitionManager);
#endif
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
            get { return new Version("1.0"); }
        }

        public override void Initialize()
        {
#if USE_APPDOMAIN
            pluginManager.Load(pluginDomain);
#else
            pluginManager.Load();
#endif
            foreach (NPCPlugin plugin in pluginManager.Plugins)
            {
                Console.WriteLine("\tLoading CustomNPC plugin: {0} v{1} by {2}", plugin.Name, plugin.Version, string.Join(", ", plugin.Authors));
            }

            NPCManager.LoadFrom(definitionManager);

            ServerApi.Hooks.GameInitialize.Register(this, OnInitialize);

            //one OnUpdate is needed for updating mob positioning
            ServerApi.Hooks.GameUpdate.Register(this, OnUpdate);
            ServerApi.Hooks.NpcSpawn.Register(this, OnNPCSpawn);
            ServerApi.Hooks.NpcLootDrop.Register(this, OnLootDrop);
            ServerApi.Hooks.NetGetData.Register(this, OnGetData);
        }

#if USE_APPDOMAIN
        private Assembly PluginDomain_OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            return null;
        }
#endif

        private void OnInitialize(EventArgs args)
        {
            Commands.ChatCommands.Add(new Command("customnpc.spawn", CommandSpawnNPC, "csm"));
            Commands.ChatCommands.Add(new Command("customnpc.invade", CommandInvade, "cinvade"));
            ConfigObj = new CustomNPCConfig();
            SetupConfig();
            NPCManager.CustomNPCInvasion.WaveSets = ConfigObj.WaveSets;
            mainLoop.Elapsed += mainLoop_Elapsed;
            mainLoop.Enabled = true;
        }

        /// <summary>
        /// For Custom Loot
        /// </summary>
        /// <param name="args"></param>
        private void OnLootDrop(NpcLootDropEventArgs args)
        {
            CustomNPCVars npcvar = NPCManager.GetCustomNPCByIndex(args.NpcArrayIndex);
            //check if monster has been customized
            if (npcvar == null || npcvar.droppedLoot)
            {
                return;
            }
			npcvar.isDead = true;
			npcvar.droppedLoot = true;
            args.Handled = npcvar.customNPC.overrideBaseNPCLoot;
            if (npcvar.customNPC.customNPCLoots != null)
            {
                foreach (CustomNPCLoot obj in npcvar.customNPC.customNPCLoots)
                {
                    if (obj.itemDropChance >= 100 || NPCManager.Chance(obj.itemDropChance))
                    {
                        int pre = 0;
                        if (obj.itemPrefix != null)
                        {
                            pre = obj.itemPrefix[rand.Next(obj.itemPrefix.Count)];
                        }

                        Item.NewItem((int)npcvar.mainNPC.position.X, (int)npcvar.mainNPC.position.Y, npcvar.mainNPC.width, npcvar.mainNPC.height, obj.itemID, obj.itemStack, false, pre, false);
                    }
                }
            }
        }

        /// <summary>
        /// Spawn custom npc using /csm <id> [amount] [x] [y]
        /// </summary>
        /// <param name="args"></param>
        private void CommandSpawnNPC(CommandArgs args)
        {
            //error if too many or too few params specified
            if (args.Parameters.Count == 0 || args.Parameters.Count > 4)
            {
                args.Player.SendInfoMessage("Info: /csm <id> [amount] [x] [y]");
                return;
            }
            //get custom npc by id
            var cdef = NPCManager.Data.GetNPCbyID(args.Parameters[0]);
            if (cdef == null)
            {
                args.Player.SendErrorMessage("Error: The custom npc id \"{0}\" does not exist!", args.Parameters[0]);
                return;
            }
            //default to 1 if amount is not defined
            int amount = 1;
            //check if amount is defined
            if (args.Parameters.Count == 2)
            {
                int.TryParse(args.Parameters[1], out amount);
            }
            int x = 0;
            int y = 0;
            if (args.Parameters.Count != 4)
            {
                x = (int)args.Player.X + rand.Next(0, 16) - 8;
                y = (int)args.Player.Y + rand.Next(0, 16) - 8;
            }
            else
            {
                if (!int.TryParse(args.Parameters[2], out x))
                {
                    args.Player.SendErrorMessage("Error: Invalid x position defined!");
                    return;
                }
                if (!int.TryParse(args.Parameters[3], out y))
                {
                    args.Player.SendErrorMessage("Error: Invalid y position defined!");
                    return;
                }
            }
            //all checks complete spawn mob
            for (int i = 0; i < amount; i++)
            {
                NPCManager.SpawnNPCAtLocation(x, y, cdef);
                cdef.currSpawnsVar++;
				if (args.Parameters.Count != 4)
				{
					x = (int)args.Player.X + rand.Next(0, 16) - 8;
					y = (int)args.Player.Y + rand.Next(0, 16) - 8;
				}
            }
        }

        /// <summary>
        /// Adds Custom Monster to array for replacement
        /// </summary>
        /// <param name="args"></param>
        private void OnUpdate(EventArgs args)
        {
            //Update all Custom NPCs
            CustomNPCUpdate();
        }

        private void OnNPCSpawn(NpcSpawnEventArgs args)
        {
            foreach (NPC obj in Main.npc)
            {
                foreach (CustomNPCDefinition customnpc in NPCManager.Data.CustomNPCs.Values)
                {
                    if (customnpc.isReplacement && obj.netID == customnpc.customBase.netID && (NPCManager.NPCs[obj.whoAmI] == null || NPCManager.NPCs[obj.whoAmI].isDead))
                    {
                        DateTime[] dt = null;
                        if (customnpc.customProjectiles != null)
                        {
                            dt = Enumerable.Repeat(DateTime.Now, customnpc.customProjectiles.Count).ToArray();
                        }
                        NPCManager.NPCs[obj.whoAmI] = new CustomNPCVars(customnpc, dt, obj);
                        NPCManager.Data.ConvertNPCToCustom(obj.whoAmI, customnpc);
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

            NPC npc = Main.npc[npcIndex];
            double damageDone = Main.CalculateDamage(damage, npc.ichor ? npc.defense - 20 : npc.defense);
            if (critical)
            {
                damageDone *= 2;
            }

            var e = new NpcDamageEvent
            {
                NpcIndex = npcIndex,
                PlayerIndex = args.Player.Index,
                Damage = damage,
                Knockback = knockback,
                Direction = direction,
                CriticalHit = critical,
                NpcHealth = Math.Max(0, npc.life - (int)damageDone)
            };

            eventManager.InvokeHandler(e, EventType.NpcDamage);

            if (npc.active && npc.life > 0 && damageDone >= npc.life)
            {
                CustomNPCVars npcvar = NPCManager.NPCs[npcIndex];
                if (npcvar != null)
                {
                    npcvar.isDead = true;
                    if (!npcvar.isClone && !npcvar.isInvasion)
                        npcvar.customNPC.currSpawnsVar--;
                }
                var killedArgs = new NpcKilledEvent
                {
                    NpcIndex = npcIndex,
                    PlayerIndex = args.Player.Index,
                    Damage = damage,
                    Knockback = knockback,
                    Direction = direction,
                    CriticalHit = critical
                };

                eventManager.InvokeHandler(killedArgs, EventType.NpcKill);

                if (npcvar != null && npcvar.isInvasion)
                {
                    NPCManager.CustomNPCInvasion.WaveSize--;
                }
            }
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                pluginManager.Unload();
#if USE_APPDOMAIN
                AppDomain.Unload(pluginDomain);
#endif

                ServerApi.Hooks.GameUpdate.Deregister(this, OnUpdate);
                ServerApi.Hooks.GameInitialize.Deregister(this, OnInitialize);
                ServerApi.Hooks.NpcLootDrop.Deregister(this, OnLootDrop);
                ServerApi.Hooks.NetGetData.Deregister(this, OnGetData);
                ServerApi.Hooks.NpcSpawn.Deregister(this, OnNPCSpawn);
            }
        }

#if USE_APPDOMAIN
        private AppDomain CreateNewPluginDomain()
        {
            AppDomainSetup info = new AppDomainSetup
            {
                // this allows the replacement of plugin files in the file system
                ShadowCopyFiles = bool.TrueString,

                ApplicationBase = Environment.CurrentDirectory,
                PrivateBinPath = @".\ServerPlugins"
            };

            return AppDomain.CreateDomain("Plugin Domain", AppDomain.CurrentDomain.Evidence, info);
        }
#endif

        /// <summary>
        /// Do Everything here
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mainLoop_Elapsed(object sender, ElapsedEventArgs e)
        {
            //Don't run this when there is no players
            if (TShock.Utils.ActivePlayers() == 0)
            {
                return;
            }
            eventManager.InvokeHandler(PluginUpdateEvent.Empty, EventType.PluginUpdate);

            //check if NPC has been deactivated (could mean NPC despawned)
            CheckActiveNPCs();
            //Spawn mobs into regions and specific biomes
            SpawnMobsInBiomeAndRegion();
            //Update All NPCs
            CustomNPCUpdate();
            //fire projectiles towards closests player
            ProjectileCheck();
            //Check for player Collision with NPC
            CollisionDetection();

            eventManager.InvokeHandler(PluginUpdateEvent.Empty, EventType.PostPluginUpdate);
        }

        private void CustomNPCUpdate()
        {
            foreach (CustomNPCVars obj in NPCManager.NPCs)
            {
                if (obj != null && !obj.isDead && obj.mainNPC.aiStyle != obj.customNPC.customAI)
                {
                    NetMessage.SendData(23, -1, -1, "", obj.mainNPC.whoAmI, 0f, 0f, 0f, 0);
                }
            }
        }


        private void CollisionDetection()
        {
            foreach (CustomNPCVars obj in NPCManager.NPCs)
            {
                if (obj == null || obj.isDead)
                    continue;

                foreach (TSPlayer player in TShock.Players)
                {
                    if (player == null || player.Dead)
                        continue;

                    Rectangle npcframe = new Rectangle((int)obj.mainNPC.position.X, (int)obj.mainNPC.position.Y, obj.mainNPC.width, obj.mainNPC.height);
                    Rectangle playerframe = new Rectangle((int)player.TPlayer.position.X, (int)player.TPlayer.position.Y, player.TPlayer.width, player.TPlayer.height);
                    if (npcframe.Intersects(playerframe))
                    {
                        var args = new NpcCollisionEvent
                        {
                            NpcIndex = obj.mainNPC.whoAmI,
                            PlayerIndex = player.Index
                        };

                        eventManager.InvokeHandler(args, EventType.NpcCollision);
                    }
                }
            }
        }

        /// <summary>
        /// Checks if any player is within targetable range of the npc, without obstacle and within firing cooldown timer.
        /// </summary>
        private void ProjectileCheck()
        {
            // loop through all custom npcs currently spawned
            foreach (CustomNPCVars obj in NPCManager.NPCs)
            {
                // check if they exists and are active
                if (obj != null && !obj.isDead && obj.mainNPC.active)
                {
                    if (obj.customNPC.customProjectiles != null)
                    {
                        // loop through all npc projectiles they can fire
                        foreach (CustomNPCProjectiles projectile in obj.customNPC.customProjectiles)
                        {
                            //custom projectile index
                            int projectileIndex = obj.customNPC.customProjectiles.IndexOf(projectile);
                            // check if projectile last fire time is greater then equal to its next allowed fire time
                            if ((DateTime.Now - obj.lastAttemptedProjectile[projectileIndex]).TotalMilliseconds >= projectile.projectileFireRate)
                            {
                                // make sure chance is checked too, don't bother checking if its 100
                                if (projectile.projectileFireChance == 100 || NPCManager.Chance(projectile.projectileFireChance))
                                {
                                    TSPlayer target = null;
                                    if (projectile.projectileLookForTarget)
                                    {
                                        // find a target for it to shoot that isn't dead or disconnected
                                        foreach (TSPlayer player in TShock.Players.Where(x => x != null && !x.Dead && x.ConnectionAlive))
                                        {
                                            // check if that target can be shot ie/ no obstacles, or if it if projectile goes through walls ignore this check
                                            if (!projectile.projectileCheckCollision || Collision.CanHit(player.TPlayer.position, player.TPlayer.bodyFrame.Width, player.TPlayer.bodyFrame.Height, obj.mainNPC.position, obj.mainNPC.width, obj.mainNPC.height))
                                            {
                                                // make sure distance isn't further then what tshock allows
                                                float currDistance = Math.Abs(Vector2.Distance(player.TPlayer.position, obj.mainNPC.center()));
                                                if (currDistance < 2048)
                                                {
                                                    // set the target player
                                                    target = player;
                                                    // set npcs target to the player its shooting at
                                                    obj.mainNPC.target = player.Index;
                                                    // break since no need to find another target
                                                    break;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        target = TShock.Players[obj.mainNPC.target];
                                    }

                                    // check if previous for loop was broken out of, or just ended because no valid target
                                    if (target != null)
                                    {
                                        // all checks completed fire projectile
                                        FireProjectile(target, obj, projectile);
                                        // set last attempted projectile to now
                                        obj.lastAttemptedProjectile[projectileIndex] = DateTime.Now;
                                    }
                                }
                                else
                                {
                                    obj.lastAttemptedProjectile[projectileIndex] = DateTime.Now;
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Fires a projectile given the target starting position and projectile class
        /// </summary>
        /// <param name="target"></param>
        /// <param name="origin"></param>
        /// <param name="projectile"></param>
        private void FireProjectile(TSPlayer target, CustomNPCVars origin, CustomNPCProjectiles projectile)
        {
            // loop through all ShotTiles
            foreach (ShotTile obj in projectile.projectileShotTiles)
            {
                // Make sure target actually exists - at this point it should always exist
                if (target != null)
                {
                    // calculate starting position
                    Vector2 start = GetStartPosition(origin, obj);
                    // calculate speed of projectile
                    Vector2 speed = CalculateSpeed(start, target);
                    // get the projectile AI
                    Tuple<float, float> ai = Tuple.Create(projectile.projectileAIParams1, projectile.projectileAIParams2);

                    // Create new projectile VIA Terraria's method, with all customizations
                    int projectileIndex = Projectile.NewProjectile(
                        start.X,
                        start.Y,
                        speed.X,
                        speed.Y,
                        projectile.projectileID,
                        projectile.projectileDamage,
                        0,
                        255,
                        ai.Item1,
                        ai.Item2);

                    // customize AI for projectile again, as it gets overwritten
                    var proj = Main.projectile[projectileIndex];
                    proj.ai[0] = ai.Item1;
                    proj.ai[1] = ai.Item2;

                    // send projectile as a packet
                    NetMessage.SendData(27, -1, -1, string.Empty, projectileIndex);
                }
            }
        }

        // Returns start position of projectile with shottile offset
        private Vector2 GetStartPosition(CustomNPCVars origin, ShotTile shottile)
        {
            Vector2 offset = new Vector2(shottile.X, shottile.Y);
            return origin.mainNPC.center() + offset;
        }

        //calculates the x y speed required angle
        private Vector2 CalculateSpeed(Vector2 start, TSPlayer target)
        {
            Vector2 targetCenter = target.TPlayer.center();

            float dirX = targetCenter.X - start.X;
            float dirY = targetCenter.Y - start.Y;
            float factor = 10f / (float)Math.Sqrt((dirX * dirX) + (dirY * dirY));
            float speedX = dirX * factor;
            float speedY = dirY * factor;

            return new Vector2(speedX, speedY);
        }

        private void CheckActiveNPCs()
        {
            foreach (CustomNPCVars obj in NPCManager.NPCs)
            {
                //if CustomNPC has been defined, and hasn't been set to dead yet, check if the terraria npc is active
                if (obj != null)
                {
                    if (!obj.isDead && (obj.mainNPC == null || obj.mainNPC.life <= 0 || obj.mainNPC.type == 0))
                    {
                        obj.isDead = true;
                        if (!obj.isClone && !obj.isInvasion)
                            obj.customNPC.currSpawnsVar--;
                    }
                    else if(!obj.isDead)
                    {
                        var args = new NpcUpdateEvent
                        {
                            NpcIndex = obj.mainNPC.whoAmI
                        };

                        eventManager.InvokeHandler(args, EventType.NpcUpdate);
                    }
                }
            }
        }

        private void SpawnMobsInBiomeAndRegion()
        {
            NPCManager.SpawnMobsInBiomeAndRegion();
        }

        /// <summary>
        /// Config for Custom Mob Invasions
        /// </summary>
        private void SetupConfig()
        {
            try
            {
                if (File.Exists(filepath))
                {
                    ConfigObj = new CustomNPCConfig();
                    ConfigObj = CustomNPCConfig.Read(filepath);
                    return;
                }
                else
                {
                    Log.ConsoleError("Config not found. Creating new one");
                    ConfigObj.Write(filepath);
                    return;
                }
            }
            catch (Exception ex)
            {
                Log.ConsoleError(ex.Message);
                return;
            }
        }

        /// <summary>
        /// Start custom npc invasion using /cinvade <type>
        /// </summary>
        /// <param name="args"></param>
        private void CommandInvade(CommandArgs args)
        {
            //error if too many or too few params specified
            if (args.Parameters.Count == 0 || args.Parameters.Count > 1)
            {
                args.Player.SendInfoMessage("Info: /cinvade <invade>");
                return;
            }
            //get custom invasion by name
            WaveSet waveset;
            if (!ConfigObj.WaveSets.TryGetValue(args.Parameters[0], out waveset))
            {
                args.Player.SendErrorMessage("Error: The custom npc id \"{0}\" does not exist!", args.Parameters[0]);
                return;
            }
            if (!NPCManager.CustomNPCInvasion.invasionStarted)
            {
                NPCManager.CustomNPCInvasion.StartInvasion(waveset);
            }
            else
            {
                NPCManager.CustomNPCInvasion.StopInvasion();
            }
        }
    }
}
