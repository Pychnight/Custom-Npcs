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

            if (player.TPlayer.zoneEvil)
                biome |= BiomeTypes.Corruption;

            if (player.TPlayer.zoneDungeon)
                biome |= BiomeTypes.Dungeon;

            if (player.TPlayer.zoneMeteor)
                biome |= BiomeTypes.Meteor;

            if (player.TPlayer.zoneHoly)
                biome |= BiomeTypes.Holy;

            if (player.TPlayer.zoneJungle)
                biome |= BiomeTypes.Jungle;

            if (player.TPlayer.zoneSnow)
                biome |= BiomeTypes.Snow;

            if (player.TPlayer.zoneBlood)
                biome |= BiomeTypes.Blood;

            if (player.TPlayer.zoneCandle)
                biome |= BiomeTypes.Candle;

            if (biome == BiomeTypes.None)
            {
                biome |= BiomeTypes.Grass;
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
    }
}
