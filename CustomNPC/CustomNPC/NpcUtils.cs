using System;
#if TShock
#elif OTAPI
using OTA;
using OTA.Logging;
using Microsoft.Xna.Framework;
using System.Linq;
#endif

namespace CustomNPC
{
	public static class NpcUtils
	{
		public static int ActivePlayerCount
		{
			get
			{
				#if TShock
				return TShock.Utils.ActivePlayers()
				#elif OTAPI
				return Terraria.Main.player.Count(x => x != null && x.active);
				#endif
			}
		}

		public static void LogError(string fmt, params object[] args)
		{
			#if TShock
			TShock.Log.ConsoleError (String.Format(fmt, args));
			#elif OTAPI
			ProgramLog.Error.Log(fmt, args);
			#endif
		}

		/// <summary>
		/// TShock.Log.Error

		public static void LogConsole(string fmt, params object[] args)
		{
			#if TShock
			TShock.Log.ConsoleInfo (String.Format(fmt, args));
			#elif OTAPI
			ProgramLog.Log(fmt, args);
			#endif
		}

		public static void MessageAllPlayers(string message, Color color)
		{
			#if TShock
			TSPlayer.All.SendMessage(message, color);
			#elif OTAPI
			Tools.NotifyAllPlayers(message, color);
			#endif
		}

		public static void InfoMessageAllPlayers(string message, params object[] args)
		{
			#if TShock
			TSPlayer.All.SendInfoMessage(message, args);
			#elif OTAPI
			Tools.NotifyAllPlayers(String.Format(message, args), Color.Orange/*TODO*/);
			#endif
		}

		public static Terraria.NPC GetNPCById(int id)
		{
			Terraria.NPC npc = new Terraria.NPC();
			npc.netDefaults(id);
			return npc;
		}
	}
}

