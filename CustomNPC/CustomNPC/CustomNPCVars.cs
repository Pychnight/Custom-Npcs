using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

#if TShock
using TShockAPI;
using TShockAPI.DB;

#elif OTAPI
using Microsoft.Xna.Framework;
#endif

namespace CustomNPC
{
	public class CustomNPCVars : IDisposable
	{
		private bool _disposed;

		public CustomNPCDefinition customNPC { get; set; }

		public DateTime[] lastAttemptedProjectile { get; set; }

		public string customID { get { return customNPC.customID; } }

		public bool overrideBaseNPCLoot { get { return customNPC.overrideBaseNPCLoot; } }
		// used for debuging default loot droping regardless if this is true or false.

		#if IS_DEAD
		public bool isDead { get; set; }
		#else
		public bool isDead
		{
			get { return !mainNPC.active; }
			set { mainNPC.active = !value; }
		}
		#endif

		public bool isUncounted { get; set; }

		public NPC mainNPC { get; set; }

		public bool droppedLoot { get; set; }

		public bool isClone { get; set; }

		public bool isInvasion { get; set; }

		public bool usingCustomAI { get; set; }

		private Random rand = new Random ();
		private IDictionary<string, object> variables = new Dictionary<string, object> ();

		public CustomNPCVars (CustomNPCDefinition customnpc, DateTime[] lastattemptedprojectile, NPC mainnpc, bool isclone = false)
		{
			lastAttemptedProjectile = lastattemptedprojectile;
			#if IS_DEAD
			isDead = false;
			#endif
			isUncounted = false;
			isClone = isclone;
			customNPC = customnpc;
			mainNPC = mainnpc;
			droppedLoot = false;

			//Update if this mob is using custom ai.
			updateCustomAI ();
		}

		public void markDead ()
		{
			#if IS_DEAD
			isDead = true;
			#endif
			if (isUncounted) return;

			isUncounted = true;
			if (customNPC.currSpawnsVar >= 1 && isDead == true && !isClone)
			{
				customNPC.currSpawnsVar--;
			}
		}

		private void updateCustomAI ()
		{
			usingCustomAI = (customNPC.customAI != customNPC.customBase.aiStyle || customNPC.noGravity != customNPC.customBase.noGravity);
		}

		/// <summary>
		/// Applies a new base type. Effect wise the same as replacing this custom NPC with a new one.
		/// </summary>
		private void CustomTransform ()
		{
			//DEBUG
			NpcUtils.LogConsole ("DEBUG [CustomTransform] entry. NPC Pos={0}, {1} OldNet={2} OldType={3}", mainNPC.position.X, mainNPC.position.Y, mainNPC.netID, mainNPC.type);
			//DEBUG

			int oldLife = mainNPC.life;
			int[] array = new int[5];
			int[] array2 = new int[5];
			for (int i = 0; i < 5; i++)
			{
				array [i] = mainNPC.buffType [i];
				array2 [i] = mainNPC.buffTime [i];
			}
			Vector2 vector = mainNPC.velocity;
			mainNPC.position.Y = mainNPC.position.Y + (float)mainNPC.height;
			int spriteDir = mainNPC.spriteDirection;

			mainNPC.netDefaults (customNPC.customBase.netID);
			//DEBUG
			NpcUtils.LogConsole ("DEBUG [CustomTransform] Post netDefaults. NPC Pos={0}, {1} NewNet={2} NewType={3}", mainNPC.position.X, mainNPC.position.Y, mainNPC.netID, mainNPC.type);
			//DEBUG

			mainNPC.spriteDirection = spriteDir;
			mainNPC.velocity = vector;
			mainNPC.position.Y = mainNPC.position.Y - (float)mainNPC.height;

			mainNPC.name = customNPC.customName;
			mainNPC.displayName = customNPC.customName;
			mainNPC.lifeMax = customNPC.customHealth;
			mainNPC.life = oldLife;
			mainNPC.aiStyle = customNPC.customAI;
			mainNPC.lavaImmune = customNPC.lavaImmune;
			mainNPC.noGravity = customNPC.noGravity;
			mainNPC.noTileCollide = customNPC.noTileCollide;
			mainNPC.boss = customNPC.isBoss;

			mainNPC.defense = customNPC.customDefense;
			mainNPC.defDefense = customNPC.customDefense;

			updateCustomAI ();

			for (int j = 0; j < 5; j++)
			{
				mainNPC.buffType [j] = array [j];
				mainNPC.buffTime [j] = array2 [j];
			}

			mainNPC.netAlways = true;

			//DEBUG
			NpcUtils.LogConsole ("DEBUG [CustomTransform] exit. NPC Pos={0}, {1} NewNet={2} NewType={3}", mainNPC.position.X, mainNPC.position.Y, mainNPC.netID, mainNPC.type);
			//DEBUG
		}

		private void NormalTransform (NPC baseType)
		{
			//DEBUG
			NpcUtils.LogConsole ("DEBUG [NormalTransform] entry. NPC Pos={0}, {1} OldNet={2} OldType={3}", mainNPC.position.X, mainNPC.position.Y, mainNPC.netID, mainNPC.type);
			//DEBUG

			int oldLife = mainNPC.life;

			mainNPC.netDefaults (baseType.netID);

			mainNPC.netAlways = true;
			mainNPC.name = customNPC.customName;
			mainNPC.displayName = customNPC.customName;
			mainNPC.lifeMax = customNPC.customHealth;
			mainNPC.life = oldLife;

			//DEBUG
			NpcUtils.LogConsole ("DEBUG [NormalTransform] exit. NPC Pos={0}, {1} NewNet={2} NewType={3}", mainNPC.position.X, mainNPC.position.Y, mainNPC.netID, mainNPC.type);
			//DEBUG
		}

		private void postTransform ()
		{
			if (Main.netMode == 2)
			{
				//DEBUG
				NpcUtils.LogConsole ("DEBUG [PostTransform] NetMode=2, updating");
				//DEBUG
				mainNPC.netUpdate = true;
				NetMessage.SendData (23, -1, -1, "", mainNPC.whoAmI, 0f, 0f, 0f, 0, 0, 0);
				NetMessage.SendData (54, -1, -1, "", mainNPC.whoAmI, 0f, 0f, 0f, 0, 0, 0);
			}
			else
			{
				//DEBUG
				NpcUtils.LogConsole ("DEBUG [PostTransform] Wrong Netmode, no update. NetMode={0}", Main.netMode);
				//DEBUG
			}
		}

		/// <summary>
		/// Transforms a NPC to another Custom NPC
		/// </summary>
		/// <param name="npcdef">CustomNPC that will be replacing it</param>
		/// <param name="addhealth">Increase monsters Health</param>
		/// <param name="additionalhealth">Amount to Increase by, if 0 - get new monsters health and add that to NPC</param>
		public void Transform (CustomNPCDefinition npcdef, bool addhealth = false, int additionalhealth = 0)
		{
			//DEBUG
			NpcUtils.LogConsole ("DEBUG [TransformToCustom] addHealth={0} additionalHealth={1}", addhealth, additionalhealth);
			//DEBUG

			customNPC = npcdef;

			CustomTransform ();

			//mainNPC.type = npcdef.customBase.type;

			if (addhealth)
			{
				if (additionalhealth == 0)
				{
					mainNPC.life += npcdef.customHealth;
				}
				else
				{
					mainNPC.life += additionalhealth;
				}
			}

			postTransform ();

			//Temp Fix for projectiles not attaching to transformations, some one else should redo this in a better way..
			DateTime[] dt = null;
			if (npcdef.customProjectiles != null)
			{
				dt = Enumerable.Repeat (DateTime.Now, npcdef.customProjectiles.Count).ToArray ();
			}

			NPC spawned = Main.npc [mainNPC.whoAmI];
			NPCManager.NPCs [mainNPC.whoAmI] = new CustomNPCVars (npcdef, dt, spawned);
			NPCManager.Data.ConvertNPCToCustom (mainNPC.whoAmI, npcdef);

		}

		/// <summary>
		/// Transforms a NPC to another Custom NPC
		/// </summary>
		/// <param name="id">CustomNPC that will be replacing it</param>
		/// <param name="addhealth">Increase monsters Health</param>
		/// <param name="additionalhealth">Amount to Increase by, if 0 - get new monsters health and add that to NPC</param>
		/// <param name="fullTransform">When false, only the skin of the NPC changes. When true, also applies some of the given NPC's stats</param>
		public bool Transform (string id, bool addhealth = false, int additionalhealth = 0)
		{
			CustomNPCDefinition search = NPCManager.Data.GetNPCbyID (id);
			//If not found, return false
			if (search == null) return false;

			Transform (search, addhealth, additionalhealth);
			return true;
		}

		/// <summary>
		/// Transform Overload for non-custom NPCs
		/// </summary>
		/// <param name="id">ID of NPC - Can be Custom</param>
		/// <param name="addhealth">Increase monsters Health</param>
		/// <param name="additionalhealth">Amount to Increase by, if 0 - get new monsters health and add that to NPC</param>
		public void Transform (int id, bool addhealth = false, int additionalhealth = 0)
		{
			//DEBUG
			NpcUtils.LogConsole ("DEBUG [TransformToNormal] addHealth={0} additionalHealth={1}", addhealth, additionalhealth);
			//DEBUG

			var obj = NpcUtils.GetNPCById (id);

			//mainNPC.type = obj.netID;
			NormalTransform (obj);

			if (addhealth)
			{
				if (additionalhealth == 0)
				{
					mainNPC.life += obj.lifeMax;
				}
				else
				{
					mainNPC.life += additionalhealth;
				}
			}

			postTransform ();
		}

		/// <summary>
		/// Heals NPC by amount, cannot heal more then MaxHP set
		/// </summary>
		/// <param name="amount"></param>
		public void SelfHealing (int amount)
		{
			mainNPC.life = Math.Min (customNPC.customHealth, mainNPC.life + amount);
		}

		/// <summary>
		/// (Attempts to) Spawn monsters randomly around the current x y position.
		/// </summary>
		/// <param name="npcvar">The Custom NPC to use to spawn children</param>
		/// <param name="amount">The amount to spawn</param>
		/// <param name="sethealth"></param>
		/// <param name="health"></param>
		public void Multiply (CustomNPCDefinition type, int amount, bool sethealth = false, int health = 0)
		{
			//DEBUG
			NpcUtils.LogConsole ("DEBUG [Multiply] entry. amount={0}, sethealth={1}, health={2}", amount, sethealth, health);
			//DEBUG

			// MainNPC is gone.
			if (mainNPC == null) return;

			if (type == null) return;

			for (int i = 0; i < amount; i++)
			{
				int x = (int)mainNPC.position.X + rand.Next (-8, 9);
				int y = (int)mainNPC.position.Y + rand.Next (-8, 9);

				int npc = NPCManager.SpawnNPCAtLocation (x, y, type);
				if (npc == -1)
				{
					//DEBUG
					NpcUtils.LogConsole ("DEBUG [Multiply] spawn failed. location={0}, {1}", x, y);
					//DEBUG
					continue;
				}

				var spawned = NPCManager.GetCustomNPCByIndex (npc);
				if (spawned == null) continue;

				if (sethealth) spawned.mainNPC.life = health;

				spawned.isClone = true;
			}
		}

		/// <summary>
		/// (Attempts to) Spawn monsters randomly around the current x y position.
		/// </summary>
		/// <param name="npcvar">The Custom NPC to use to spawn children</param>
		/// <param name="amount">The amount to spawn</param>
		/// <param name="sethealth"></param>
		/// <param name="health"></param>
		public void Multiply (CustomNPCVars npcvar, int amount, bool sethealth = false, int health = 0)
		{
			// MainNPC is gone.
			if (mainNPC == null) return;

			if (npcvar == null) return;

			Multiply (npcvar.customNPC, amount, sethealth, health);
		}

		/// <summary>
		/// (Attempts to) Spawn monsters randomly around the current x y position.
		/// </summary>
		/// <param name="id">The type that the children should have</param>
		/// <param name="amount">The amount to spawn</param>
		/// <param name="sethealth"></param>
		/// <param name="health"></param>
		public void Multiply (string id, int amount, bool sethealth = false, int health = 0)
		{
			// MainNPC is gone.
			if (mainNPC == null) return;

			var def = NPCManager.Data.GetNPCbyID (id);
			if (def == null) return;

			Multiply (def, amount, sethealth, health);
		}

		/// <summary>
		/// Teleports a NPC to a specific location in a region
		/// </summary>
		/// <param name="region"></param>
		/// <param name="randompos"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void TeleportNPC (CustomNPCVars npcvar, string region, bool randompos = true, int x = 0, int y = 0)
		{
			#if TShock
            Region obj = null;
            try
            {
                obj = TShock.Regions.GetRegionByName(region);
            }
            catch
            {
				NpcUtils.LogError("Error: a defined region does not exist on this map \"{0}\"", region);
                return;
            }

            if (randompos)
            {
                TShock.Utils.GetRandomClearTileWithInRange(obj.Area.Left, obj.Area.Top, obj.Area.Width, obj.Area.Height, out x, out y);
            }
            else
            {
                x += obj.Area.Left;
                y += obj.Area.Top;
            }
			#endif

			TeleportNPC (npcvar, x, y);
		}

		/// <summary>
		/// Statically set x y based on the world coordinates, and teleport npc there
		/// </summary>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void TeleportNPC_NoUpdate (CustomNPCVars npcvar, int x, int y)
		{
			npcvar.mainNPC.position = new Vector2 (x * 16, y * 16);
		}

		/// <summary>
		/// Properly teleports an NPC to a new location.
		/// </summary>
		/// <param name="npcvar"></param>
		/// <param name="x"></param>
		/// <param name="y"></param>
		public void TeleportNPC (CustomNPCVars npcvar, int x, int y)
		{
			npcvar.mainNPC.Teleport (new Vector2 (x * 16, y * 16));
		}

		/// <summary>
		/// Returns the current position of the monster
		/// </summary>
		/// <returns></returns>
		public Vector2 ReturnPos (CustomNPCVars npcvar)
		{
			return new Vector2 (npcvar.mainNPC.position.X, npcvar.mainNPC.position.Y);
		}

		/// <summary>
		/// Sends a message to all nearby players, distance define-able
		/// </summary>
		/// <param name="distance"></param>
		/// <param name="message"></param>
		/// <param name="color"></param>
		public void MessageNearByPlayers (CustomNPCVars npcvar, int distance, string message, Color color)
		{
			Vector2 temp = ReturnPos (npcvar);
			int squaredist = distance * distance;

			#if TShock
            foreach (TSPlayer obj in TShock.Players)
            {
                if (obj == null || !obj.ConnectionAlive) continue;

                if (Vector2.DistanceSquared(temp, obj.LastNetPosition) <= squaredist)
                {
                    obj.SendMessage(message, color);
                }
            }
			#elif OTAPI
			foreach (var obj in Terraria.Main.player)
			{
				if (obj == null || !obj.active) continue;

				if (Vector2.DistanceSquared (temp, obj.position) <= squaredist)
				{
					obj.SendMessage (message, color);
				}
			}
			#endif
		}

		public void SetVariable<T> (string name, T value)
		{
			variables [name] = value;
		}

		public T GetVariable<T> (string name, T defaultValue = default(T))
		{
			object boxed;
			if (!variables.TryGetValue (name, out boxed))
				return defaultValue;

			return (T)boxed;
		}

		public void OnDeath ()
		{
			if (customNPC != null && customNPC.customID != null)
			{
				customNPC.OnDeath (this);
			}
		}

		public void Dispose ()
		{ 
			Dispose (true);
			GC.SuppressFinalize (this);           
		}

		protected virtual void Dispose (bool disposing)
		{
			if (_disposed)
				return; 

			if (disposing)
			{
				if (null != customNPC) customNPC = null;
				if (null != lastAttemptedProjectile) lastAttemptedProjectile = null;
				if (null != mainNPC) mainNPC = null;
			}

			_disposed = true;
		}
	}
}
