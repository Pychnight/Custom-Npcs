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
    [ApiVersion(1, 17)]
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
            get { return new Version("1.1"); }
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
            Commands.ChatCommands.Add(new Command("customnpc.list", CommandListNPCS, "csmlist"));
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
            if (npcvar == null || npcvar.droppedLoot) return;

            args.Handled = npcvar.customNPC.overrideBaseNPCLoot;
            if (npcvar.droppedLoot) return;
            
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
            npcvar.isDead = true;
            npcvar.droppedLoot = true;
        }

        /// <summary>
        /// Spawn custom npc using /csm &lt;id&gt; [amount] [x] [y]
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
            }
        }

        /// <summary>
        /// List custom npcs using /csmlist &lt;page&gt;
        /// </summary>
        /// <param name="args"></param>
        private void CommandListNPCS(CommandArgs args)
        {
            //error if too many or too few params specified
            if (args.Parameters.Count == 0 || args.Parameters.Count > 1)
            {
                args.Player.SendInfoMessage("Info: /csmlist <page>");
                return;
            }

            int page;
            if (!int.TryParse(args.Parameters[0], out page))
            {
                args.Player.SendErrorMessage("Error: Invalid page defined! Please give a number.");
                return;
            }

            //Check if page is valid
            if (page < 1)
            {
                args.Player.SendErrorMessage("Error: Invalid page defined! Page has to be 1 or bigger.");
                return;
            }

            var vals = NPCManager.Data.CustomNPCs.Values;

            int start = (page - 1) * 5;
            int end = start + 5;
            int total = vals.Count();
            int totalpages = (total + 5) / 5;

            if (start > total)
            {
                args.Player.SendErrorMessage("Error: Page too large! The last page is \"{0}\".", totalpages);
                return;
            }

            args.Player.SendInfoMessage("Page {0} / {1} ", page, totalpages);

            for (int i = start; i < end && i < total; i++)
            {
                CustomNPCDefinition obj = vals.ElementAt(i);
                if (obj == null) continue;
                
                args.Player.SendInfoMessage("[{0}]: {1}. Spawned: {2}", obj.customID, obj.customName, obj.currSpawnsVar);
            }
        }

        /// <summary>
        /// Called every time the server ticks.
        /// Adds Custom Monster to array for replacement
        /// </summary>
        /// <param name="args"></param>
        private void OnUpdate(EventArgs args)
        {
            //Update all NPCs with custom AI
            CustomNPCUpdate(true, false);
        }

        private void OnNPCSpawn(NpcSpawnEventArgs args)
        {
            //If the id falls outside the possible range, we can return.
            if (args.NpcId < 0 || args.NpcId >= 200) return;

            //This NPC is custom and not dead.
            if (NPCManager.NPCs[args.NpcId] != null && !NPCManager.NPCs[args.NpcId].isDead) return;

            NPC spawned = Main.npc[args.NpcId];
            foreach (CustomNPCDefinition customnpc in NPCManager.Data.CustomNPCs.Values)
            {
                if (!customnpc.isReplacement || spawned.netID != customnpc.customBase.netID) continue;

                DateTime[] dt = null;
                if (customnpc.customProjectiles != null)
                {
                    dt = Enumerable.Repeat(DateTime.Now, customnpc.customProjectiles.Count).ToArray();
                }
                NPCManager.NPCs[spawned.whoAmI] = new CustomNPCVars(customnpc, dt, spawned);
                NPCManager.Data.ConvertNPCToCustom(spawned.whoAmI, customnpc);

                break;
            }
        }

        /// <summary>
        /// Old version of the OnNPCSpawn, which goes over ALL NPCs on the server for some reason.
        /// </summary>
        /// <param name="args"></param>
        private void AlternateOnNPCSpawn(NpcSpawnEventArgs args)
        {
            //Filter replacements beforehand
            List<CustomNPCDefinition> replacements = NPCManager.Data.CustomNPCs.Values.Where(cm => cm.isReplacement).ToList();
            if (replacements.Count == 0) return;

            foreach (NPC obj in Main.npc)
            {
                if (obj == null) continue;

                //Skip mobs which are already custom (and the custom is still alive)
                if (NPCManager.NPCs[obj.whoAmI] != null && !NPCManager.NPCs[obj.whoAmI].isDead) continue;

                foreach (CustomNPCDefinition customnpc in replacements)
                {
                    //Wrong type
                    if (obj.netID != customnpc.customBase.netID) continue;

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
                    if (!npcvar.isUncounted)
                    {
                        npcvar.isUncounted = true;
                        if (!npcvar.isClone && !npcvar.isInvasion)
                            npcvar.customNPC.currSpawnsVar--;
                    }
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

            //Commented out, since this should happen on the main thread
            //Update All NPCs with custom AI (that are alive)
            //CustomNPCUpdateSafe(true);

            //fire projectiles towards closests player
            ProjectileCheck();
            //Check for player Collision with NPC
            CollisionDetection();

            eventManager.InvokeHandler(PluginUpdateEvent.Empty, EventType.PostPluginUpdate);
        }

        private void CustomNPCUpdate(bool onlyCustom = true, bool updateDead = false)
        {
            foreach (CustomNPCVars obj in NPCManager.NPCs)
            {
                //Dead
                if (obj == null) continue;

                //Should be dead
                if (obj.isDead || obj.mainNPC == null || obj.mainNPC.life <= 0 || obj.mainNPC.type == 0)
                {
                    if (updateDead && obj.isUncounted)
                    {
                        obj.isDead = true;
                        obj.isUncounted = true;
                        if (!obj.isClone && !obj.isInvasion)
                            obj.customNPC.currSpawnsVar--;
                    }

                    continue;
                }

                if (!onlyCustom || obj.usingCustomAI)
                {
                    NetMessage.SendData(23, -1, -1, "", obj.mainNPC.whoAmI, 0f, 0f, 0f, 0);
                }
            }
        }

        private void CollisionDetection()
        {
            foreach (CustomNPCVars obj in NPCManager.NPCs)
            {
                if (obj == null || obj.isDead) continue;

                foreach (TSPlayer player in TShock.Players)
                {
                    if (player == null || player.Dead) continue;

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
                // Check if they exists and are active
                if (obj == null || obj.isDead || !obj.mainNPC.active) continue;

                // We only want the npcs with custom projectiles
                if (obj.customNPC.customProjectiles == null) continue;

                // Loop through all npc projectiles they can fire
                DateTime savedNow = DateTime.Now;

                int k = 0;
                foreach (CustomNPCProjectiles projectile in obj.customNPC.customProjectiles)
                {
                    //custom projectile index
                    //int projectileIndex = obj.customNPC.customProjectiles.IndexOf(projectile);

                    // check if projectile last fire time is greater then equal to its next allowed fire time
                    if ((savedNow - obj.lastAttemptedProjectile[k]).TotalMilliseconds >= projectile.projectileFireRate)
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
                                obj.lastAttemptedProjectile[k] = savedNow;
                            }
                        }
                        else
                        {
                            obj.lastAttemptedProjectile[k] = savedNow;
                        }
                    }

                    // Increment Index
                    k++;
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
                if (obj == null) continue;

                if ((obj.isDead && !obj.isUncounted) || obj.mainNPC == null || obj.mainNPC.life <= 0 || obj.mainNPC.type == 0)
                {
                    obj.isDead = true;
                    if (!obj.isUncounted)
                    {
                        obj.isUncounted = true;
                        if (!obj.isClone && !obj.isInvasion)
                            obj.customNPC.currSpawnsVar--;
                    }
                }
                else if (!obj.isDead)
                {
                    var args = new NpcUpdateEvent
                    {
                        NpcIndex = obj.mainNPC.whoAmI
                    };

                    eventManager.InvokeHandler(args, EventType.NpcUpdate);
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
                    TShock.Log.ConsoleError("Config not found. Creating new one");
                    ConfigObj.Write(filepath);
                    return;
                }
            }
            catch (Exception ex)
            {
                TShock.Log.ConsoleError(ex.Message);
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
