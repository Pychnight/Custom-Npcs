using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

#if TShock
using TShockAPI;
#elif OTAPI
using Microsoft.Xna.Framework;
#endif

namespace CustomNPC
{
    internal static class Extensions
    {
		#if TShock
        public static BiomeTypes GetCurrentBiomes(this TSPlayer player)
		#elif OTAPI
		public static BiomeTypes GetCurrentBiomes(this Terraria.Player player)
		#endif
        {
            BiomeTypes biome = BiomeTypes.None;

			#if TShock
			if (player.TPlayer.ZoneCorrupt)
			biome |= BiomeTypes.Corruption;

			if (player.TPlayer.ZoneCrimson)
			biome |= BiomeTypes.Crimsion;

			if (player.TPlayer.ZoneDesert)
			biome |= BiomeTypes.Desert;

			if (player.TPlayer.ZoneDungeon)
			biome |= BiomeTypes.Dungeon;

			if (player.TPlayer.ZoneGlowshroom)
			biome |= BiomeTypes.Glowshroom;

			if (player.TPlayer.ZoneHoly)
			biome |= BiomeTypes.Holy;

			if (player.TPlayer.ZoneJungle)
			biome |= BiomeTypes.Jungle;

			if (player.TPlayer.ZoneMeteor)
			biome |= BiomeTypes.Meteor;

			if (player.TPlayer.ZonePeaceCandle)
			biome |= BiomeTypes.PeaceCandle;

			if (player.TPlayer.ZoneSnow)
			biome |= BiomeTypes.Snow;

			if (player.TPlayer.ZoneTowerNebula)
			biome |= BiomeTypes.TowerNebula;

			if (player.TPlayer.ZoneTowerSolar)
			biome |= BiomeTypes.TowerSolar;

			if (player.TPlayer.ZoneTowerStardust)
			biome |= BiomeTypes.TowerStardust;

			if (player.TPlayer.ZoneTowerVortex)
			biome |= BiomeTypes.TowerVortex;

			if (player.TPlayer.ZoneUndergroundDesert)
			biome |= BiomeTypes.UndergroundDesert;

			if (player.TPlayer.ZoneWaterCandle)
			biome |= BiomeTypes.WaterCandle;
			#elif OTAPI
			if (player.ZoneCorrupt)
				biome |= BiomeTypes.Corruption;

			if (player.ZoneCrimson)
				biome |= BiomeTypes.Crimsion;

			if (player.ZoneDesert)
				biome |= BiomeTypes.Desert;

			if (player.ZoneDungeon)
				biome |= BiomeTypes.Dungeon;

			if (player.ZoneGlowshroom)
				biome |= BiomeTypes.Glowshroom;

			if (player.ZoneHoly)
				biome |= BiomeTypes.Holy;

			if (player.ZoneJungle)
				biome |= BiomeTypes.Jungle;

			if (player.ZoneMeteor)
				biome |= BiomeTypes.Meteor;

			if (player.ZonePeaceCandle)
				biome |= BiomeTypes.PeaceCandle;

			if (player.ZoneSnow)
				biome |= BiomeTypes.Snow;

			if (player.ZoneTowerNebula)
				biome |= BiomeTypes.TowerNebula;

			if (player.ZoneTowerSolar)
				biome |= BiomeTypes.TowerSolar;

			if (player.ZoneTowerStardust)
				biome |= BiomeTypes.TowerStardust;

			if (player.ZoneTowerVortex)
				biome |= BiomeTypes.TowerVortex;

			if (player.ZoneUndergroundDesert)
				biome |= BiomeTypes.UndergroundDesert;

			if (player.ZoneWaterCandle)
				biome |= BiomeTypes.WaterCandle;
			#endif

            if (biome == BiomeTypes.None)
            {
                biome = BiomeTypes.Grass;
            }

            return biome;
        }

        public static T CreateInstanceAndUnwrap<T>(this AppDomain domain)
        {
            Type type = typeof(T);

            return (T)domain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName);
        }

        public static T CreateInstanceAndUnwrap<T>(this AppDomain domain, params object[] args)
        {
            Type type = typeof(T);

            return (T)domain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName, true, BindingFlags.Default, null, args, null, null);
        }

        public static Rectangle ToPixels(this Rectangle rectangle)
        {
            return new Rectangle(rectangle.X * 16, rectangle.Y * 16, rectangle.Width * 16, rectangle.Height * 16);
        }

        public static Rectangle ToTiles(this Rectangle rectangle)
        {
            return new Rectangle(rectangle.X / 16, rectangle.Y / 16, rectangle.Width / 16, rectangle.Height / 16);
        }
    }
}
