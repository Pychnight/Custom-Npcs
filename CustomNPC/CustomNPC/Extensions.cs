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
			var _player = player.TPlayer;
			#elif OTAPI
			var _player = player;
			#endif

			if (_player.ZoneCorrupt)
				biome |= BiomeTypes.Corruption;

			if (_player.ZoneCrimson)
				biome |= BiomeTypes.Crimsion;

			if (_player.ZoneDesert)
				biome |= BiomeTypes.Desert;

			if (_player.ZoneDungeon)
				biome |= BiomeTypes.Dungeon;

			if (_player.ZoneGlowshroom)
				biome |= BiomeTypes.Glowshroom;

			if (_player.ZoneHoly)
				biome |= BiomeTypes.Holy;

			if (_player.ZoneJungle)
				biome |= BiomeTypes.Jungle;

			if (_player.ZoneMeteor)
				biome |= BiomeTypes.Meteor;

			if (_player.ZonePeaceCandle)
				biome |= BiomeTypes.PeaceCandle;

			if (_player.ZoneSnow)
				biome |= BiomeTypes.Snow;

			if (_player.ZoneTowerNebula)
				biome |= BiomeTypes.TowerNebula;

			if (_player.ZoneTowerSolar)
				biome |= BiomeTypes.TowerSolar;

			if (_player.ZoneTowerStardust)
				biome |= BiomeTypes.TowerStardust;

			if (_player.ZoneTowerVortex)
				biome |= BiomeTypes.TowerVortex;

			if (_player.ZoneUndergroundDesert)
				biome |= BiomeTypes.UndergroundDesert;

			if (_player.ZoneWaterCandle)
				biome |= BiomeTypes.WaterCandle;

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
