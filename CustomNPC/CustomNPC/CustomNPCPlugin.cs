using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using CustomNPC.EventSystem;
using CustomNPC.EventSystem.Events;
using CustomNPC.Plugins;
using Terraria;

#if TShock
using TerrariaApi.Server;
using Wolfje.Plugins.SEconomy;
using Wolfje.Plugins.SEconomy.Journal;
using TShockAPI;
using TShockAPI.DB;

#elif OTAPI
using OTA;
using OTA.Plugin;
using OTA.Logging;
using OTA.Commands;
using OTA.Command;
using Microsoft.Xna.Framework;

[assembly: PluginDependency ("OTA.Commands")]
#endif

namespace CustomNPC
{
	#if TShock
    [ApiVersion(1, 22)]
    public class CustomNPCPlugin : TerrariaPlugin

#elif OTAPI
	[OTAVersion (1, 0)]
	public class CustomNPCPlugin : BasePlugin
#endif
    {
		internal Random rand = new Random ();

		public CustomNPCConfig ConfigObj { get; set; }
		#if TShock
		private String SavePath = TShock.SavePath;
		internal static string filepath { get { return Path.Combine(TShock.SavePath, "customnpcinvasion.json"); } }

#elif OTAPI
		private String SavePath = Globals.DataPath;

		internal static string filepath { get { return Path.Combine (Globals.DataPath, "customnpcinvasion.json"); } }
		#endif
		public static bool UsingSEConomy { get; set; }

		//16.66 milliseconds for 1/60th of a second.
		private Timer mainLoop = new Timer (1000 / 60.0);
		private EventManager eventManager;
		private DefinitionManager definitionManager;
		#if USE_APPDOMAIN
        private AppDomain pluginDomain;
#endif
		private PluginManager<NPCPlugin> pluginManager;

		#if TShock
		public CustomNPCPlugin(Main game)
		: base(game)
		{
			Init();
		}

#elif OTAPI
		public CustomNPCPlugin ()
		{
			base.Author = "IcyGaming(v1.0), Taeir(v1.1), Pychnight(v1.2 & v1.3)";
			base.Description = "Create Custom NPCs";
			base.Name = "CustomNPC";
			base.Version = "1.3";
		}
		#endif

		private void Init ()
		{
#if TShock
			UsingSEConomy = File.Exists(Path.Combine("ServerPlugins", "Wolfje.Plugins.SEconomy.dll")); //I wouldn't reccommend this...
#else
			UsingSEConomy = false;
#endif
			eventManager = new EventManager ();
			definitionManager = new DefinitionManager ();

#if USE_APPDOMAIN
			pluginDomain = CreateNewPluginDomain();
#endif
			AppDomain.CurrentDomain.AssemblyResolve += PluginDomain_OnAssemblyResolve;
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
			pluginManager = new PluginManager<NPCPlugin> (eventManager, definitionManager);
#endif
		}

		#if TShock
        public override string Author
        {
            get { return "IcyGaming(v1.0), Taeir(v1.1), Pychnight(v1.2 & v1.3)"; }
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
            get { return new Version("1.3"); }
        }
#endif

		#if TShock
		public override void Initialize ()
		{

#elif OTAPI
		protected override void Initialized (object state)
		{
			base.Initialized (state);
			Init ();
			OnInit ();
#endif
#if USE_APPDOMAIN
            pluginManager.Load(pluginDomain);
#else
			pluginManager.Load ();
#endif
			foreach (NPCPlugin plugin in pluginManager.Plugins)
			{
				Console.WriteLine ("\tLoading CustomNPC plugin: {0} v{1} by {2}", plugin.Name, plugin.Version, string.Join (", ", plugin.Authors));
			}

			NPCManager.LoadFrom (definitionManager);

#if TShock
			ServerApi.Hooks.GameInitialize.Register (this, OnInitialize);

			//one OnUpdate is needed for updating mob positioning
			ServerApi.Hooks.GameUpdate.Register (this, OnUpdate);
			ServerApi.Hooks.NpcSpawn.Register (this, OnNPCSpawn);
			ServerApi.Hooks.NpcLootDrop.Register (this, OnItemDrop);
			ServerApi.Hooks.ServerChat.Register (this, OnChat);
			ServerApi.Hooks.NetGetData.Register (this, OnGetData);
#endif
		}

		//#if USE_APPDOMAIN
		private Assembly PluginDomain_OnAssemblyResolve (object sender, ResolveEventArgs args)
		{
			if (args.Name.IndexOf ("CustomNPC,") > -1)
			{
				return typeof(CustomNPCPlugin).Assembly;
			}
			return null;
		}
		//#endif

		private void CreateCommands ()
		{
#if TShock
			Commands.ChatCommands.Add (new Command ("customnpc.spawn", CommandSpawnNPC, "csm"));
			Commands.ChatCommands.Add (new Command ("customnpc.invadereload", CommandIReload, "csminvadereload"));
			Commands.ChatCommands.Add (new Command ("customnpc.list", CommandListNPCS, "csmlist", "clist"));
			Commands.ChatCommands.Add (new Command ("customnpc.invade", CommandInvade, "csminvade", "cinvade"));
			Commands.ChatCommands.Add (new Command ("customnpc.info", CommandNPCInfo, "csminfo", "cinfo"));
#elif OTAPI
			this.AddCommand ("csm")
                .ByPermissionNode ("customnpc.spawn")
                .Calls (CommandSpawnNPC);
            this.AddCommand("csmlist")
                .ByPermissionNode("customnpc.list")
                .Calls(CommandListNPCS);
            this.AddCommand("csminfo")
                .ByPermissionNode("customnpc.info")
                .Calls(CommandNPCInfo);
            //this.AddCommand("csmreload")
              //.ByPermissionNode("customnpc.reload")
              //.Calls(CommandReload);
#endif
        }

#if TShock
		private void OnInitialize (EventArgs args)

#elif OTAPI
        private void OnInit ()
#endif
        {
			CreateCommands ();
			ConfigObj = new CustomNPCConfig ();
			SetupConfig ();
			NPCManager.CustomNPCInvasion.WaveSets = ConfigObj.WaveSets;
			mainLoop.Elapsed += mainLoop_Elapsed;
			mainLoop.Enabled = true;
		}
#if TShock
        void OnChat (ServerChatEventArgs args)
				{
					if (args.Handled)
					{
						return;
					}
		
					TSPlayer player = TShock.Players [args.Who];
		
					if (player == null)
					{
						args.Handled = true;
						return;
					}
		
					//Needs improvement and clean up //
					//Works as intended just looks messy
		
					string[] chat = args.Text.Split ();
					string cmd = chat [0].Substring (0);
		
					var ServerChatEventArgs = new ServerChatEvent ();
		
					int Who = args.Who;
					String Text = args.Text;
					MessageBuffer Buffer = args.Buffer;
		
					Who = player.Index;
					Text = cmd;
					Buffer = args.Buffer;
		
		    		ServerChatEventArgs.Who = Who;
					ServerChatEventArgs.Text = Text;
					ServerChatEventArgs.Buffer = Buffer;
		
					eventManager.InvokeHandler (ServerChatEventArgs, EventType.ServerChat);
				}
#endif

#if TShock
        /// <summary>
        /// For Custom Loot
        /// </summary>
        /// <param name="args"></param>
        private void OnItemDrop (NpcLootDropEventArgs args)
				{
					CustomNPCVars npcvar = NPCManager.GetCustomNPCByIndex (args.NpcArrayIndex);
					if (npcvar == null || npcvar.droppedLoot) return;
		
					npcvar.isDead = true;
					npcvar.droppedLoot = true;
					args.Handled = npcvar.customNPC.overrideBaseNPCLoot;
		
					//Check if monster has been customized
					if (npcvar.customNPC.customNPCLoots != null)
					{
						foreach (CustomNPCLoot obj in npcvar.customNPC.customNPCLoots)
						{
							if (obj.itemDropChance >= 100 || NPCManager.Chance (obj.itemDropChance))
							{
								int pre = 0;
								if (obj.itemPrefix != null)
								{
									pre = obj.itemPrefix [rand.Next (obj.itemPrefix.Count)];
								}
		
								Item.NewItem ((int)npcvar.mainNPC.position.X, (int)npcvar.mainNPC.position.Y, npcvar.mainNPC.width, npcvar.mainNPC.height, obj.itemID, obj.itemStack, false, pre, false);
							}
						}
					}
				}
#endif

#if TShock
        /// <summary>
        /// Spawn custom npc using /csm &lt;id&gt; [amount] [&lt;x&gt; &lt;y&gt;]
        /// </summary>
        /// <param name="args"></param>
        private void CommandSpawnNPC (CommandArgs args)
				{
					//Error if too many or too few params specified
					if (args.Parameters.Count == 0 || args.Parameters.Count > 4)
					{
						args.Player.SendInfoMessage ("Usage: /csm <id> [<amount>] [<x> <y>]");
						return;
					}
		
					//Get custom npc by id
					var cdef = NPCManager.Data.GetNPCbyID (args.Parameters [0]);
					if (cdef == null)
					{
						args.Player.SendErrorMessage ("Error: The custom npc with id \"{0}\" does not exist!", args.Parameters [0]);
						return;
					}

					//Default to 1 if amount is not defined
					int amount = 1;
		
					//Check if amount is defined
					if (args.Parameters.Count == 2 || args.Parameters.Count == 4)
					{
						int.TryParse (args.Parameters [1], out amount);
		
						//Check for too many mobs
						if (amount > 200)
						{
							args.Player.SendErrorMessage ("Error: Amount needs to be lower than 200!");
							return;
						}
					}
		
					//Check for X and Y
					int x;
					int y;
		
					//Not specified, use player's coordinates. (/csm <id> [amount])
					if (args.Parameters.Count <= 2)
					{
						x = (int)args.Player.X + rand.Next (-8, 9);
						y = (int)args.Player.Y + rand.Next (-8, 9);
					}
		
		            //Specified, no amount (/csm <id> <x> <y>)
		            else if (args.Parameters.Count == 3)
					{
						if (!int.TryParse (args.Parameters [1], out x))
						{
							args.Player.SendErrorMessage ("Error: Invalid x position defined!");
							return;
						}
						if (!int.TryParse (args.Parameters [2], out y))
						{
							args.Player.SendErrorMessage ("Error: Invalid y position defined!");
							return;
						}
					}
		
		            //All arguments specified (/csm <id> <amount> <x> <y>)
		            else
					{
						if (!int.TryParse (args.Parameters [2], out x))
						{
							args.Player.SendErrorMessage ("Error: Invalid x position defined!");
							return;
						}
						if (!int.TryParse (args.Parameters [3], out y))
						{
							args.Player.SendErrorMessage ("Error: Invalid y position defined!");
							return;
						}
					}
		
					//Keep track of spawns that fail.
					int failed = 0;
		
					//Spawn mobs
					for (int i = 0; i < amount; i++)
					{
						int j = NPCManager.SpawnNPCAtLocation (x, y, cdef);
		
						if (j == -1)
							failed++;
						else
							cdef.currSpawnsVar++;
					}
		
					//Inform player
					if (failed > 0)
						args.Player.SendWarningMessage ("Failed to spawn {0} of {1} \"{2}\"'s at ({3}, {4})", failed, amount, args.Parameters [0], x, y);
					else
						args.Player.SendSuccessMessage ("Spawned {0} \"{1}\"'s at ({2}, {3})", amount, args.Parameters [0], x, y);
				}
		

#elif OTAPI
		/// <summary>
		/// Spawn custom npc using /csm &lt;id&gt; [amount] [&lt;x&gt; &lt;y&gt;]
		/// </summary>
		/// <param name="args"></param>
		private void CommandSpawnNPC (ISender sender, ArgumentList args)
		{
			//Error if too many or too few params specified
			if (args.Count == 0 || args.Count > 4)
			{
				throw new CommandError ("Invalid usage");
				return;
			}

			//Get custom npc by id
			var id = args.GetString (0);
			var cdef = NPCManager.Data.GetNPCbyID (id);
			if (cdef == null)
			{
				throw new CommandError ("Error: The custom npc with id \"{0}\" does not exist!", id);
				return;
			}

			//Default to 1 if amount is not defined
			int amount = 1;

			//Check if amount is defined
			if (args.Count == 2 || args.Count == 4)
			{
				//Check for too many mobs
				if (!args.TryGetInt (1, out amount) || amount > 200)
				{
					throw new CommandError ("Error: Amount needs to be lower than 200!");
					return;
				}
			}

			//Check for X and Y
			int x;
			int y;

			//Not specified, use player's coordinates. (/csm <id> [amount])
			if (args.Count <= 2 && sender is Player)
			{
				x = (int)(sender as Player).position.X + rand.Next (-8, 9);
				y = (int)(sender as Player).position.Y + rand.Next (-8, 9);
			}

            //Specified, no amount (/csm <id> <x> <y>)
            else if (args.Count == 3)
			{
				if (!args.TryGetInt (1, out x))
				{
					throw new CommandError ("Error: Invalid x position defined!");
					return;
				}
				if (!args.TryGetInt (2, out y))
				{
					throw new CommandError ("Error: Invalid y position defined!");
					return;
				}
			}

            //All arguments specified (/csm <id> <amount> <x> <y>)
            else
			{
				if (!args.TryGetInt (2, out x))
				{
					throw new CommandError ("Error: Invalid x position defined!");
				}
				if (!args.TryGetInt (3, out y))
				{
					throw new CommandError ("Error: Invalid y position defined!");
				}
			}

			//Keep track of spawns that fail.
			int failed = 0;

			//Spawn mobs
			for (int i = 0; i < amount; i++)
			{
				int j = NPCManager.SpawnNPCAtLocation (x, y, cdef);

				if (j == -1)
					failed++;
				else
					cdef.currSpawnsVar++;
			}

			//Inform player
			if (failed > 0)
				sender.SendMessage (String.Format ("Failed to spawn {0} of {1} \"{2}\"'s at ({3}, {4})", failed, amount, args.GetInt (0), x, y), 255, 200, 200, 0);
			else
				sender.SendMessage (String.Format ("Spawned {0} \"{1}\"'s at ({2}, {3})", amount, args.GetInt (0), x, y), R: 0, B: 0);
		}
#endif

#if TShock
        /// <summary>
        /// List custom npcs using /csmlist &lt;page&gt; [onlyalive]
        /// </summary>
        /// <param name="args"></param>
        private void CommandListNPCS(CommandArgs args)
        {
            //Error if too many or too few params specified
            if (args.Parameters.Count == 0 || args.Parameters.Count > 2)
            {
                args.Player.SendInfoMessage("Usage: /csmlist <page> [onlyalive]");
                return;
            }

            int page;
            if (!int.TryParse(args.Parameters[0], out page))
            {
                args.Player.SendErrorMessage("Error: Invalid page defined! Please give a number.");
                return;
            }

            bool onlyalive = false;
            if (args.Parameters.Count == 2 && !bool.TryParse(args.Parameters[1], out onlyalive))
            {
                args.Player.SendErrorMessage("Error: Invalid onlyalive flag! onlyalive can be true or false.");
                return;
            }

            //Check if page is valid
            if (page < 1)
            {
                args.Player.SendErrorMessage("Error: Invalid page defined! Page has to be 1 or bigger.");
                return;
            }

            var vals = NPCManager.Data.CustomNPCs.Values.Where(cd => cd != null && (!onlyalive || cd.currSpawnsVar > 0));

            int start = (page - 1) * 6;
            int end = start + 6;
            int total = vals.Count();
            int totalpages = (total + 5) / 6;

            if (start > total)
            {
                args.Player.SendErrorMessage("Error: Page too large! The last page is \"{0}\".", totalpages);
                return;
            }

            args.Player.SendInfoMessage("Page {0} / {1}. Only Alive = {2}", page, totalpages, onlyalive);

            for (int i = start; i < end && i < total; i++)
            {
                CustomNPCDefinition obj = vals.ElementAt(i);

                args.Player.SendInfoMessage("[{0}]: {1}. Spawned: {2}", obj.customID, obj.customName, obj.currSpawnsVar);
            }
        }

#elif OTAPI

        // OTA Exclusive bugs
        // CSMLIST does not list all monsters loaded...
        // need solution for (args.Count == 2 && !bool.TryParse(00000, out onlyalive))

        /// <summary>
        /// List custom npcs using /csmlist &lt;page&gt; [onlyalive]
        /// </summary>
        /// <param name="args"></param>
        private void CommandListNPCS(ISender sender, ArgumentList args)
        {
            //Error if too many or too few params specified
            if (args.Count == 0 || args.Count > 2)
            {
                //throw new CommandError("Usage: /csmlist <page> [onlyalive]");
                sender.SendMessage(String.Format("Usage: /csmlist <page> [onlyalive]"));
                return;
            }

            int page;
            if (!args.TryGetInt(0, out page))
            {
                //throw new CommandError("Error: Invalid page defined! Please give a number.");
                sender.SendMessage(String.Format("Error: Invalid page defined! Please give a number."));
                return;
            }

            //TODO: Improve?
            //onlyalive used to be an bool with set to false.
            //having it as a int might create some side effects.
            int onlyalive = 0;
            if (args.Count == 2 && (!args.TryGetInt(1, out onlyalive)))
            {
                sender.SendMessage(String.Format("Error: Invalid onlyalive flag! onlyalive can be true or false."));
                //throw new CommandError("Error: Invalid onlyalive flag! onlyalive can be true or false.");
                return;
            }

            //Check if page is valid
            if (page < 1)
            {
                sender.SendMessage(String.Format("Error: Invalid page defined! Page has to be 1 or bigger."));
                //throw new CommandError("Error: Invalid page defined! Page has to be 1 or bigger.");
                return;
            }

            var vals = NPCManager.Data.CustomNPCs.Values.Where(cd => cd != null && (onlyalive == 0 || cd.currSpawnsVar > 0));

            int start = (page - 1) * 6;
            int end = start + 6;
            int total = vals.Count();
            int totalpages = (total + 5) / 6;

            if (start > total)
            {
                //throw new CommandError("Error: Page too large!The last page is \"{0}\".", totalpages);
                sender.SendMessage(String.Format("Error: Page too large!The last page is \"{0}\".", totalpages));
                return;
            }

            sender.SendMessage(String.Format("Spawned {0} \"{1}\"'s at ({2}, {3})", page, totalpages, onlyalive, args.GetString(0)));

            for (int i = start; i < end && i < total; i++)
            {
                CustomNPCDefinition obj = vals.ElementAt(i);

                sender.SendMessage(String.Format("[{0}]: {1}. Spawned: {2}", obj.customID, obj.customName, obj.currSpawnsVar));
                //throw new CommandError("[{0}]: {1}. Spawned: {2}", obj.customID, obj.customName, obj.currSpawnsVar);
            }
        }
#endif

#if TShock
        /// <summary>
        /// List custom npcs using /csminfo &lt;id&gt;
        /// </summary>
        /// <param name="args"></param>
        private void CommandNPCInfo (CommandArgs args)
        		{
        			//Error if too many or too few params specified
        			if (args.Parameters.Count != 1)
        			{
        				args.Player.SendInfoMessage ("Usage: /csminfo <id>");
        				return;
        			}
        
        			//Get custom npc by id
        			var cdef = NPCManager.Data.GetNPCbyID (args.Parameters [0]);
        			if (cdef == null)
        			{
        				args.Player.SendErrorMessage ("Error: The custom npc with id \"{0}\" does not exist!", args.Parameters [0]);
        				return;
       		    	}
        
        			//Info About CustomSlime
        			//Custom Name: Slimenator (Default: Slime)
        			//Base: Slime (10)
        			//Custom AI: No (Default: 1)
        			//Custom Health: 20 (Default: 15) -- Custom Defense: 10 (Default: 3)
        			//NoGravity: Yes (Default: No) -- NoTileCollide: Yes (Default: No)
        			//Boss: Yes (Default: No) -- Immune to Lava: Yes (Default: No)
        			//Custom Projectiles: No -- Custom Loot: Yes
        			//Custom Spawn Message: Yaaaarrrrrgh
        			//Alive: 2 / 20
        
        			args.Player.SendInfoMessage ("Info About {0}", cdef.customID);
        			args.Player.SendInfoMessage ("Custom Name: {0} (Default: {1})", TaeirUtil.ValOrNo (cdef.customName), cdef.customBase.name);
        			args.Player.SendInfoMessage ("Base: {0} ({1})", TaeirUtil.GetNPCName (cdef.customBase.netID), cdef.customBase.netID);
        			args.Player.SendInfoMessage ("Custom AI: {0} (Default: {1})", TaeirUtil.ValOrNo (cdef.customAI, cdef.customBase.aiStyle), cdef.customBase.aiStyle);
        			args.Player.SendInfoMessage ("Custom Health: {0} (Default: {1}) -- Custom Defense: {2} (Default: {3})", TaeirUtil.ValOrNo (cdef.customHealth, cdef.customBase.lifeMax), cdef.customBase.lifeMax, TaeirUtil.ValOrNo (cdef.customDefense, cdef.customBase.defense), cdef.customBase.defense);
        			args.Player.SendInfoMessage ("NoGravity: {0} (Default: {1}) -- NoTileCollide: {2} (Default: {3})", TaeirUtil.YesNo (cdef.noGravity), TaeirUtil.YesNo (cdef.customBase.noGravity), TaeirUtil.YesNo (cdef.noTileCollide), TaeirUtil.YesNo (cdef.customBase.noTileCollide));
        			args.Player.SendInfoMessage ("Boss: {0} (Default: {1}) -- Immune to Lava: {2} (Default: {3})", TaeirUtil.YesNo (cdef.isBoss), TaeirUtil.YesNo (cdef.customBase.boss), TaeirUtil.YesNo (cdef.lavaImmune), TaeirUtil.YesNo (cdef.customBase.lavaImmune));
        			args.Player.SendInfoMessage ("Custom Projectiles: {0} -- Custom Loot: {1}", TaeirUtil.YesNo (cdef.customProjectiles.Count != 0), TaeirUtil.YesNo (cdef.customNPCLoots.Count != 0));
        			args.Player.SendInfoMessage ("Custom Spawn Message: {0}", TaeirUtil.ValOrNo (cdef.customSpawnMessage));
        			args.Player.SendInfoMessage ("Alive: {0} / {1}", cdef.currSpawnsVar, TaeirUtil.ValOrNo (cdef.maxSpawns, -1, "No Max"));
        		}
#elif OTAPI
        /// <summary>
        /// List custom npcs using /csminfo &lt;id&gt;
        /// </summary>
        /// <param name="args"></param>
        private void CommandNPCInfo(ISender sender, ArgumentList args)
        {
            //Error if too many or too few params specified
            if (args.Count != 1)
            {
                sender.SendMessage(String.Format("Usage: /csminfo <id>"));
                return;
            }

            //Get custom npc by id
            var cdef = NPCManager.Data.GetNPCbyID(args.GetString(0));
            if (cdef == null)
            {
                sender.SendMessage(String.Format("Error: The custom npc with id \"{0}\" does not exist!", args.GetInt(0)));
                return;
            }

            //Info About CustomSlime
            //Custom Name: Slimenator (Default: Slime)
            //Base: Slime (10)
            //Custom AI: No (Default: 1)
            //Custom Health: 20 (Default: 15) -- Custom Defense: 10 (Default: 3)
            //NoGravity: Yes (Default: No) -- NoTileCollide: Yes (Default: No)
            //Boss: Yes (Default: No) -- Immune to Lava: Yes (Default: No)
            //Custom Projectiles: No -- Custom Loot: Yes
            //Custom Spawn Message: Yaaaarrrrrgh
            //Alive: 2 / 20

            sender.SendMessage(String.Format("Info About {0}", cdef.customID));
            sender.SendMessage(String.Format("Custom Name: {0} (Default: {1})", TaeirUtil.ValOrNo(cdef.customName), cdef.customBase.name));
            sender.SendMessage(String.Format("Base: {0} ({1})", TaeirUtil.GetNPCName(cdef.customBase.netID), cdef.customBase.netID));
            sender.SendMessage(String.Format("Custom AI: {0} (Default: {1})", TaeirUtil.ValOrNo(cdef.customAI, cdef.customBase.aiStyle), cdef.customBase.aiStyle));
            sender.SendMessage(String.Format("Custom Health: {0} (Default: {1}) -- Custom Defense: {2} (Default: {3})", TaeirUtil.ValOrNo(cdef.customHealth, cdef.customBase.lifeMax), cdef.customBase.lifeMax, TaeirUtil.ValOrNo(cdef.customDefense, cdef.customBase.defense), cdef.customBase.defense));
            sender.SendMessage(String.Format("NoGravity: {0} (Default: {1}) -- NoTileCollide: {2} (Default: {3})", TaeirUtil.YesNo(cdef.noGravity), TaeirUtil.YesNo(cdef.customBase.noGravity), TaeirUtil.YesNo(cdef.noTileCollide), TaeirUtil.YesNo(cdef.customBase.noTileCollide)));
            sender.SendMessage(String.Format("Boss: {0} (Default: {1}) -- Immune to Lava: {2} (Default: {3})", TaeirUtil.YesNo(cdef.isBoss), TaeirUtil.YesNo(cdef.customBase.boss), TaeirUtil.YesNo(cdef.lavaImmune), TaeirUtil.YesNo(cdef.customBase.lavaImmune)));
            sender.SendMessage(String.Format("Custom Projectiles: {0} -- Custom Loot: {1}", TaeirUtil.YesNo(cdef.customProjectiles.Count != 0), TaeirUtil.YesNo(cdef.customNPCLoots.Count != 0)));
            sender.SendMessage(String.Format("Custom Spawn Message: {0}", TaeirUtil.ValOrNo(cdef.customSpawnMessage)));
            sender.SendMessage(String.Format("Alive: {0} / {1}", cdef.currSpawnsVar, TaeirUtil.ValOrNo(cdef.maxSpawns, -1, "No Max")));
        }
#endif

        /// <summary>
        /// Called every time the server ticks.
        /// Adds Custom Monster to array for replacement
        /// </summary>
        /// <param name="args"></param>
        private void OnUpdate (EventArgs args)
		{
			//Update all NPCs with custom AI
			CustomNPCUpdate (true, false);
		}

#if TShock
        private void OnNPCSpawn (NpcSpawnEventArgs args)
						{
							//If the id falls outside the possible range, we can return.
							if (args.NpcId < 0 || args.NpcId >= 200) return;
		
							//This NPC is custom and not dead.
							if (NPCManager.NPCs [args.NpcId] != null && !NPCManager.NPCs [args.NpcId].isDead) return;
		
							NPC spawned = Main.npc [args.NpcId];
		
							//DEBUG
							TShock.Log.ConsoleInfo ("DEBUG [NPCSpawn] NPCIndex {0}", args.NpcId);
							//DEBUG
		
							foreach (CustomNPCDefinition customnpc in NPCManager.Data.CustomNPCs.Values)
							{
								if (!customnpc.isReplacement || spawned.netID != customnpc.customBase.netID) continue;
		
								DateTime[] dt = null;
								if (customnpc.customProjectiles != null)
								{
									dt = Enumerable.Repeat (DateTime.Now, customnpc.customProjectiles.Count).ToArray ();
								}
								NPCManager.NPCs [spawned.whoAmI] = new CustomNPCVars (customnpc, dt, spawned);
								NPCManager.Data.ConvertNPCToCustom (spawned.whoAmI, customnpc);
		
								break;
							}
						}
#endif


        private void ReadNpcStrike (int playerId, byte[] readBuffer, int index, int length)
		{
			int npcIndex;
			int damage;
			float knockback;
			byte direction;
			bool critical;
			using (var data = new MemoryStream (readBuffer, index, length))
			{
				using (var br = new BinaryReader (data))
				{
					npcIndex = br.ReadInt16 ();
					damage = br.ReadInt16 ();
					knockback = br.ReadSingle ();
					direction = (byte)(br.ReadByte () - 1);
					critical = br.ReadByte () == 1;
				}
			}
#if TShock
				var player = TShock.Players[playerId];
#elif OTAPI
			var player = Main.player [playerId];
#endif
			if (npcIndex > 0 && npcIndex < 200)
			{
				OnNpcDamaged (player, npcIndex, damage, knockback, direction, critical);
			}
		}

		#if OTAPI
		[Hook]
		void OnGetData (ref HookContext ctx, ref HookArgs.ReceiveNetMessage args)
		{
			if (args.PacketId != (int)Packet.DAMAGE_NPC) return;

			ReadNpcStrike (args.BufferId, Terraria.NetMessage.buffer [args.BufferId].readBuffer, args.Start, args.Length);
//			ctx.SetResult (HookResult.IGNORE);
		}
		#elif TShock
		private void OnGetData (GetDataEventArgs args)
		{
			//Ignore cancelled events
			if (args.Handled) return;

			PacketTypes type = args.MsgID;

			//Only handle NPC strike
			if (type != PacketTypes.NpcStrike) return;

			TSPlayer player = TShock.Players [args.Msg.whoAmI];
			if (player == null || !player.ConnectionAlive) return;

		ReadNpcStrike(args.Msg.whoAmI, args.Msg.readBuffer, args.Index, args.Length);
		}
#endif

		#region Event Dispatchers

		#if TShock
				private void OnNpcDamaged (TSPlayer player, int npcIndex, int damage, float knockback, byte direction, bool critical)



#elif OTAPI
		private void OnNpcDamaged (Player player, int npcIndex, int damage, float knockback, byte direction, bool critical)
#endif
        {
			CustomNPCVars npcvar = NPCManager.NPCs [npcIndex];
			if (npcvar != null)
			{
				//DEBUG
				NpcUtils.LogConsole ("DEBUG [NPCDamage] NPCIndex {0}", npcIndex);
				//DEBUG

				NPC npc = Main.npc [npcIndex];
				double damageDone = Main.CalculateDamage (damage, npc.ichor ? npc.defense - 20 : npc.defense);
				if (critical)
				{
					damageDone *= 2;
				}

				//Damage event
				var e = new NpcDamageEvent {
					NpcIndex = npcIndex,
#if TShock
                    PlayerIndex = player.Index,
#elif OTAPI
                    PlayerIndex = player.whoAmI,
#endif
					Damage = damage,
					Knockback = knockback,
					Direction = direction,
					CriticalHit = critical,
					NpcHealth = Math.Max (0, npc.life - (int)damageDone)
				};

				eventManager.InvokeHandler (e, EventType.NpcDamage);

				//Disable to prevent flooding (Debug)
				//Console.WriteLine (player.name + " Has Damaged " + npcvar.customNPC.customID + " in " + npcIndex);

				//This damage will kill the NPC.
				if (npc.active && npc.life > 0 && damageDone >= npc.life)
				{

					//Kill event
					var killedArgs = new NpcKilledEvent {
						NpcIndex = npcIndex,
#if TShock
                        PlayerIndex = player.Index,
#elif OTAPI
                        PlayerIndex = player.whoAmI,
#endif
                        Damage = damage,
						Knockback = knockback,
						Direction = direction,
						CriticalHit = critical,
						LastPosition = npc.position
					};

					eventManager.InvokeHandler (killedArgs, EventType.NpcKill);

                    //FIXME: Disabled Debug Use Debug Events NPC
                    //Console.WriteLine (player.name + " Has Killed " + npcvar.customNPC.customID + " in " + npcIndex);
#if TShock
                    if (UsingSEConomy == true)
										{
											var economyPlayer = Wolfje.Plugins.SEconomy.SEconomyPlugin.Instance.GetBankAccount (TSPlayer.Server.User.ID);
							
											if (economyPlayer.IsAccountEnabled)
											{
												var SEconReward = new Wolfje.Plugins.SEconomy.Money (npcvar.customNPC.SEconReward);
							
												if (npcvar.customNPC.SEconReward > 0)
												{
													IBankAccount Player = SEconomyPlugin.Instance.GetBankAccount (TSPlayer.Server.User.ID);
													SEconomyPlugin.Instance.WorldAccount.TransferToAsync (Player, SEconReward, BankAccountTransferOptions.AnnounceToReceiver, npcvar.customNPC.customName + " Bountie", "Custom Npc Kill");
													SEconReward = 0;
												}
											}
											else if (!economyPlayer.IsAccountEnabled)
											{
												TShock.Log.Error ("You cannot gain any bounty because your account is disabled.");
											}
										}
#endif

                    if (npcvar != null)
					{
						npcvar.markDead ();
					}

					npcvar.OnDeath ();

					if (npcvar != null && npcvar.isInvasion)
					{
						NPCManager.CustomNPCInvasion.WaveSize--;
					}

					Console.WriteLine (npcvar.customNPC.customID + " is dead " + npcIndex);

					NPCManager.DeallocateNpc(npcIndex);
				}
			}
		}

        #endregion

#if TShock
        protected override void Dispose (bool disposing)
				{
					if (disposing)
					{
						pluginManager.Unload ();
		#if USE_APPDOMAIN
		                AppDomain.Unload(pluginDomain);
		#endif
		
						ServerApi.Hooks.GameUpdate.Deregister (this, OnUpdate);
						ServerApi.Hooks.GameInitialize.Deregister (this, OnInitialize);
						ServerApi.Hooks.NpcLootDrop.Deregister (this, OnItemDrop);
						ServerApi.Hooks.NetGetData.Deregister (this, OnGetData);
						ServerApi.Hooks.ServerChat.Deregister (this, OnChat);
						ServerApi.Hooks.NpcSpawn.Deregister (this, OnNPCSpawn);
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
#endif

        /// <summary>
        /// Do Everything here
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void mainLoop_Elapsed (object sender, ElapsedEventArgs e)
		{
			//Don't run this when there is no players
			if (NpcUtils.ActivePlayerCount == 0)
			{
				return;
			}
			eventManager.InvokeHandler (PluginUpdateEvent.Empty, EventType.PluginUpdate);

			//check if NPC has been deactivated (could mean NPC despawned)
			CheckActiveNPCs ();
			//Spawn mobs into regions and specific biomes
			SpawnMobsInBiomeAndRegion ();

			//Commented out, since this should happen on the main thread
			//Update All NPCs with custom AI (that are alive)
			//CustomNPCUpdate(true, false);

			//fire projectiles towards closests player
			ProjectileCheck ();
			//Check for player Collision with NPC
			CollisionDetection ();

			eventManager.InvokeHandler (PluginUpdateEvent.Empty, EventType.PostPluginUpdate);
		}

		private void CustomNPCUpdate (bool onlyCustom = true, bool updateDead = false)
		{
			foreach (CustomNPCVars obj in NPCManager.NPCs)
			{
				//Dead
				if (obj == null) continue;

				if (obj.isDead || obj.mainNPC == null || obj.mainNPC.life <= 0 || obj.mainNPC.type == 0)
				{
					if (updateDead) obj.markDead ();
					continue;
				}

				if (!onlyCustom || obj.usingCustomAI)
				{
					if (Main.netMode == 2)
					{
						//Schedule updates to occur every server tick (once every 1/60th second)
						if (obj.mainNPC.netSpam > 0) obj.mainNPC.netSpam = 0;
						obj.mainNPC.netUpdate = true;
						obj.mainNPC.netUpdate2 = true;
					}
					else
					{
						NetMessage.SendData (23, -1, -1, "", obj.mainNPC.whoAmI, 0f, 0f, 0f, 0, 0, 0);
					}
				}
			}
		}

		private void CollisionDetection ()
		{
			foreach (CustomNPCVars obj in NPCManager.NPCs)
			{
				//check null and if dead
				if (obj == null || obj.isDead) continue;

				//check custom id first before doing anything to ensure we never get normal monsters
				if (obj.customNPC.customID == null) continue;

				Rectangle npcframe = new Rectangle ((int)obj.mainNPC.position.X, (int)obj.mainNPC.position.Y, obj.mainNPC.width, obj.mainNPC.height);

#if TShock
				foreach (TSPlayer player in TShock.Players)
#elif OTAPI
				foreach (var player in Terraria.Main.player)
#endif
                {
#if TShock
					if (player == null || player.Dead) continue;

					Rectangle playerframe = new Rectangle ((int)player.TPlayer.position.X, (int)player.TPlayer.position.Y, player.TPlayer.width, player.TPlayer.height);
					if (npcframe.Intersects (playerframe))
					{
						var args = new NpcCollisionEvent {
							NpcIndex = obj.mainNPC.whoAmI,
							PlayerIndex = player.Index
						};

						eventManager.InvokeHandler (args, EventType.NpcCollision);
					}
#elif OTAPI
					if (player == null || player.dead) continue;

					Rectangle playerframe = new Rectangle ((int)player.position.X, (int)player.position.Y, player.width, player.height);
					if (npcframe.Intersects (playerframe))
					{
						var args = new NpcCollisionEvent {
							NpcIndex = obj.mainNPC.whoAmI,
							PlayerIndex = player.whoAmI
						};

						eventManager.InvokeHandler (args, EventType.NpcCollision);
					}
#endif
				}
			}
		}

		/// <summary>
		/// Checks if any player is within targetable range of the npc, without obstacle and within firing cooldown timer.
		/// </summary>
		private void ProjectileCheck ()
		{
			//Loop through all custom npcs currently spawned
			foreach (CustomNPCVars obj in NPCManager.NPCs)
			{
				//Check if they exists and are active
				if (obj == null || obj.isDead || !obj.mainNPC.active) continue;

				//We only want the npcs with custom projectiles
				if (obj.customNPC.customProjectiles == null) continue;

				//Save the current time
				DateTime savedNow = DateTime.Now;

				int k = 0;
				//Loop through all npc projectiles they can fire
				foreach (CustomNPCProjectiles projectile in obj.customNPC.customProjectiles)
				{
					//Check if projectile last fire time is greater then equal to its next allowed fire time
					if ((savedNow - obj.lastAttemptedProjectile [k]).TotalMilliseconds >= projectile.projectileFireRate)
					{
						//Make sure chance is checked too, don't bother checking if its 100
						if (projectile.projectileFireChance == 100 || NPCManager.Chance (projectile.projectileFireChance))
						{
#if TShock
							TSPlayer target = null;
#elif OTAPI
							Terraria.Player target = null;
#endif
							if (projectile.projectileLookForTarget)
							{
								//Find a target for it to shoot that isn't dead or disconnected
#if TShock
								foreach (TSPlayer player in TShock.Players.Where(x => x != null && !x.Dead && x.ConnectionAlive))
								{
									//Check if that target can be shot ie/ no obstacles, or if it if projectile goes through walls ignore this check
									if (!projectile.projectileCheckCollision || Collision.CanHit (player.TPlayer.position, player.TPlayer.bodyFrame.Width, player.TPlayer.bodyFrame.Height, obj.mainNPC.position, obj.mainNPC.width, obj.mainNPC.height))
									{
										//Make sure distance isn't further then what tshock allows
										float currDistance = Vector2.DistanceSquared (player.TPlayer.position, obj.mainNPC.Center);

										//Distance^2 < 4194304 is the same as Distance < 2048, but faster
										if (currDistance < 4194304)
										{
											//Set the target player
											target = player;
											//Set npcs target to the player its shooting at
											obj.mainNPC.target = player.Index;
											//Break since no need to find another target
											break;
										}
									}
								}
#elif OTAPI
								foreach (Player player in Terraria.Main.player.Where(x => x != null && !x.dead && x.active))
								{
									//Check if that target can be shot ie/ no obstacles, or if it if projectile goes through walls ignore this check
									if (!projectile.projectileCheckCollision || CanHit (player.position, (int)player.bodyFrame.Width, (int)player.bodyFrame.Height, obj.mainNPC.position, (int)obj.mainNPC.frame.Width, (int)obj.mainNPC.frame.Height))
									{
										//Make sure distance isn't further then what tshock allows
										float currDistance = Vector2.DistanceSquared (player.position, obj.mainNPC.Center);

										//Distance^2 < 4194304 is the same as Distance < 2048, but faster
										if (currDistance < 4194304)
										{
											//Set the target player
											target = player;
											//Set npcs target to the player its shooting at
											obj.mainNPC.target = player.whoAmI;
											//Break since no need to find another target
											break;
										}
									}
								}
#endif
							}
							else
							{
#if TShock
								target = TShock.Players [obj.mainNPC.target];
#elif OTAPI
								target = Terraria.Main.player [obj.mainNPC.target];
#endif
							}

							//Check if we found a valid target
							if (target != null)
							{
								//All checks completed. Fire projectile
								FireProjectile (target, obj, projectile);
								//Set last attempted projectile to now
								obj.lastAttemptedProjectile [k] = savedNow;
							}
						}
						else
						{
							obj.lastAttemptedProjectile [k] = savedNow;
						}
					}

					//Increment Index
					k++;
				}
			}
		}


		public static bool CanHit (Vector2 Position1, int Width1, int Height1, Vector2 Position2, int Width2, int Height2)
		{
			int num = (int)((Position1.X + (float)(Width1 / 2)) / 16);
			int num2 = (int)((Position1.Y + (float)(Height1 / 2)) / 16);
			int num3 = (int)((Position2.X + (float)(Width2 / 2)) / 16);
			int num4 = (int)((Position2.Y + (float)(Height2 / 2)) / 16);
			if (num <= 1)
			{
				num = 1;
			}
			if (num >= Main.maxTilesX)
			{
				num = Main.maxTilesX - 1;
			}
			if (num3 <= 1)
			{
				num3 = 1;
			}
			if (num3 >= Main.maxTilesX)
			{
				num3 = Main.maxTilesX - 1;
			}
			if (num2 <= 1)
			{
				num2 = 1;
			}
			if (num2 >= Main.maxTilesY)
			{
				num2 = Main.maxTilesY - 1;
			}
			if (num4 <= 1)
			{
				num4 = 1;
			}
			if (!(num4 < Main.maxTilesY))
			{
				num4 = Main.maxTilesY - 1;
			}
			bool flag;
			try
			{
				while (true)
				{
					int num5 = Math.Abs (num - num3);
					int num6 = Math.Abs (num2 - num4);
					if (num == num3 && num2 == num4)
					{
						flag = true;
						return flag;
					}
					if (num5 <= num6)
					{
						num2 = ((num2 < num4) ? (num2 + 1) : (num2 - 1));
						if (Main.tile [num - 1, num2] == null)
						{
							break;
						}
						if (Main.tile [num + 1, num2] == null)
						{
							flag = false;
							return flag;
						}
						if (!Main.tile [num - 1, num2].inActive () && Main.tile [num - 1, num2].active () && Main.tileSolid [(int)Main.tile [num - 1, num2].type] && !Main.tileSolidTop [(int)Main.tile [num - 1, num2].type] && Main.tile [num - 1, num2].slope () == 0 && !Main.tile [num - 1, num2].halfBrick () && !Main.tile [num + 1, num2].inActive () && Main.tile [num + 1, num2].active () && Main.tileSolid [(int)Main.tile [num + 1, num2].type] && !Main.tileSolidTop [(int)Main.tile [num + 1, num2].type] && Main.tile [num + 1, num2].slope () == 0 && !Main.tile [num + 1, num2].halfBrick ())
						{
							flag = false;
							return flag;
						}
					}
					else
					{
						num = ((num < num3) ? (num + 1) : (num - 1));
						if (Main.tile [num, num2 - 1] == null)
						{
							flag = false;
							return flag;
						}
						if (Main.tile [num, num2 + 1] == null)
						{
							flag = false;
							return flag;
						}
						if (!Main.tile [num, num2 - 1].inActive () && Main.tile [num, num2 - 1].active () && Main.tileSolid [(int)Main.tile [num, num2 - 1].type] && !Main.tileSolidTop [(int)Main.tile [num, num2 - 1].type] && Main.tile [num, num2 - 1].slope () == 0 && !Main.tile [num, num2 - 1].halfBrick () && !Main.tile [num, num2 + 1].inActive () && Main.tile [num, num2 + 1].active () && Main.tileSolid [(int)Main.tile [num, num2 + 1].type] && !Main.tileSolidTop [(int)Main.tile [num, num2 + 1].type] && Main.tile [num, num2 + 1].slope () == 0 && !Main.tile [num, num2 + 1].halfBrick ())
						{
							flag = false;
							return flag;
						}
					}
					if (Main.tile [num, num2] == null)
					{
						flag = false;
						return flag;
					}
					if (!Main.tile [num, num2].inActive () && Main.tile [num, num2].active () && Main.tileSolid [(int)Main.tile [num, num2].type] && !Main.tileSolidTop [(int)Main.tile [num, num2].type])
					{
						flag = false;
					}
				}
				flag = false;
				return flag;
			}
			catch
			{
				flag = false;
			}
			return flag;
		}

		//
		/// <summary>
		/// Fires a projectile given the target starting position and projectile class
		/// </summary>
		/// <param name="target"></param>
		/// <param name="origin"></param>
		/// <param name="projectile"></param>
#if TShock
		private void FireProjectile (TSPlayer target, CustomNPCVars origin, CustomNPCProjectiles projectile)
#elif OTAPI
        private void FireProjectile (Player target, CustomNPCVars origin, CustomNPCProjectiles projectile)
#endif
        {
			//Loop through all ShotTiles
			foreach (ShotTile obj in projectile.projectileShotTiles)
			{
				//Make sure target actually exists - at this point it should always exist
				if (target == null) continue;

				//Calculate starting position
				Vector2 start = GetStartPosition (origin, obj);
				//Calculate speed of projectile
				Vector2 speed = CalculateSpeed (start, target);
				//Get the projectile AI
				Tuple<float, float> ai = Tuple.Create (projectile.projectileAIParams1, projectile.projectileAIParams2);

				// Create new projectile VIA Terraria's method, with all customizations
				int projectileIndex = Projectile.NewProjectile (
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
				var proj = Main.projectile [projectileIndex];
				proj.ai [0] = ai.Item1;
				proj.ai [1] = ai.Item2;

				proj.friendly = false;
				proj.hostile = true;
				proj.npcProj = true;
				proj.StatusNPC (255);
				proj.StatusPvP (1);
				proj.StatusPlayer (0);

				// send projectile as a packet
				NetMessage.SendData (27, -1, -1, string.Empty, projectileIndex, 0f, 0f, 0f, 0, 0, 0);
			}
		}

		// Returns start position of projectile with shottile offset
		private Vector2 GetStartPosition (CustomNPCVars origin, ShotTile shottile)
		{
			Vector2 offset = new Vector2 (shottile.X, shottile.Y);
			return origin.mainNPC.Center + offset;
		}

		//calculates the x y speed required angle
#if TShock
		private Vector2 CalculateSpeed (Vector2 start, TSPlayer target)
		



#elif OTAPI
		private Vector2 CalculateSpeed (Vector2 start, Player target)
#endif
        {
#if TShock
			Vector2 targetCenter = target.TPlayer.Center;
#elif OTAPI
			Vector2 targetCenter = target.Center;
#endif

			float dirX = targetCenter.X - start.X;
			float dirY = targetCenter.Y - start.Y;
			float factor = 10f / (float)Math.Sqrt ((dirX * dirX) + (dirY * dirY));
			float speedX = dirX * factor;
			float speedY = dirY * factor;

			return new Vector2 (speedX, speedY);
		}

		private void CheckActiveNPCs ()
		{
			foreach (CustomNPCVars obj in NPCManager.NPCs)
			{
				//if CustomNPC has been defined, and hasn't been set to dead yet, check if the terraria npc is active
				if (obj == null) continue;

				if ((obj.isDead && !obj.isUncounted) || obj.mainNPC == null || obj.mainNPC.life <= 0 || obj.mainNPC.type == 0)
				{
					obj.markDead ();
				}
				else if (!obj.isDead)
				{
					var args = new NpcUpdateEvent {
						NpcIndex = obj.mainNPC.whoAmI
					};

					eventManager.InvokeHandler (args, EventType.NpcUpdate);
				}
			}
		}

		private void SpawnMobsInBiomeAndRegion ()
		{
			NPCManager.SpawnMobsInBiomeAndRegion ();
		}

		/// <summary>
		/// Config for Custom Mob Invasions
		/// </summary>
		private void SetupConfig ()
		{
			try
			{
				if (File.Exists (filepath))
				{
					ConfigObj = new CustomNPCConfig ();
					ConfigObj = CustomNPCConfig.Read (filepath);
					return;
				}
				else
				{
					NpcUtils.LogError ("Config not found. Creating new one");
					ConfigObj.Write (filepath);
					return;
				}
			}
			catch (Exception ex)
			{
				NpcUtils.LogError (ex.Message);
				return;
			}
		}

        // TODO: Improve Reload commands, Maybe create 1 generic reloading command that reloads the json file first then subplugins?
        // Enabled features for tshock, Needs Conversion for OTA

#if TShock
        /// <summary>
        /// Reload Config File <type>
        /// </summary>
        /// <param name="args"></param>
        private void CommandIReload (CommandArgs args)
        {
        	if (NPCManager.CustomNPCInvasion.invasionStarted)
        	{
        		NPCManager.CustomNPCInvasion.StopInvasion ();
        	}
        
        	try
        	{
        		if (File.Exists (filepath))
        		{
        			ConfigObj = new CustomNPCConfig ();
       			ConfigObj = CustomNPCConfig.Read (filepath);
        			return;
        		}
        		else
        		{
        			TShock.Log.ConsoleError ("Config not found. Creating new one");
        			ConfigObj.Write (filepath);
        			return;
        		}
        	}
        	catch (Exception ex)
        	{
        		TShock.Log.ConsoleError (ex.Message);
        		return;
        	}
        }
#endif

#if TShock
        /// <summary>
        /// Start custom npc invasion using /cinvade <type>
        /// </summary>
        /// <param name="args"></param>
        private void CommandInvade (CommandArgs args)
		{
			//Error if too many or too few params specified
			if (args.Parameters.Count == 0 || args.Parameters.Count > 1)
			{
				args.Player.SendInfoMessage ("Info: /cinvade <invade>");
				return;
			}
			//Get custom invasion by name
			WaveSet waveset;
			if (!ConfigObj.WaveSets.TryGetValue (args.Parameters [0], out waveset))
			{
				args.Player.SendErrorMessage ("Error: The waveset \"{0}\" does not exist!", args.Parameters [0]);
				return;
			}
			if (!NPCManager.CustomNPCInvasion.invasionStarted)
			{
				NPCManager.CustomNPCInvasion.StartInvasion (waveset);
			}
			else
			{
				NPCManager.CustomNPCInvasion.StopInvasion ();
			}
		}
#endif

    }
}
