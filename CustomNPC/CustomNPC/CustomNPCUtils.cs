using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TShockAPI;

namespace CustomNPC
{
    public class CustomNPCUtils
    {
        public Random rand = new Random();

        private static readonly CustomNPCUtils instance = new CustomNPCUtils();
        private CustomNPCUtils() { }
        internal static CustomNPCUtils Instance { get { return instance; } }

        /// <summary>
        /// Chance system, returns true based on the percentage passed through
        /// </summary>
        /// <param name="percentage">At most 2 decimal points</param>
        /// <returns></returns>
        public bool Chance(double percentage)
        {
            if (rand.NextDouble() * 100 <= percentage)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public BiomeTypes PlayersCurrBiome(TSPlayer player)
        {
            if (player.TPlayer.zoneEvil)
            {
                return BiomeTypes.Corruption;
            }
            else if (player.TPlayer.zoneBlood)
            {
                return BiomeTypes.Blood;
            }
            else if (player.TPlayer.zoneCandle)
            {
                return BiomeTypes.Candle;
            }
            else if (player.TPlayer.zoneDungeon)
            {
                return BiomeTypes.Dungeon;
            }
            else if (player.TPlayer.zoneHoly)
            {
                return BiomeTypes.Holy;
            }
            else if (player.TPlayer.zoneJungle)
            {
                return BiomeTypes.Jungle;
            }
            else if (player.TPlayer.zoneMeteor)
            {
                return BiomeTypes.Meteor;
            }
            else if (player.TPlayer.zoneSnow)
            {
                return BiomeTypes.Snow;
            }
            else return BiomeTypes.Grass;
        }
    }
}
