using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TShockAPI;

namespace CustomNPC
{
    internal static class Extensions
    {
        public static BiomeTypes GetCurrentBiomes(this TSPlayer player)
        {
            BiomeTypes biome = BiomeTypes.None;

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
