using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomNPC
{

    /// <summary>
    /// Biome Types
    /// </summary>
    [Flags]
    public enum BiomeTypes : byte
    {
        /// <summary>
        /// Doesn't Spawn in Biomes
        /// </summary>
        None = 0,
        /// <summary>
        /// Player is in noZone
        /// </summary>
        Grass = 1,
        /// <summary>
        /// Player is in zoneEvil
        /// </summary>
        Corruption = 2,
        /// <summary>
        /// Player is in zoneDungeon
        /// </summary>
        Dungeon = 3,
        /// <summary>
        /// Player is in zoneMeteor
        /// </summary>
        Meteor =  4,
        /// <summary>
        /// Player is in zoneHoly
        /// </summary>
        Holy = 5,
        /// <summary>
        /// Player is in zoneJungle
        /// </summary>
        Jungle = 6,
        /// <summary>
        /// Player is in zoneSnow
        /// </summary>
        Snow = 7,
        /// <summary>
        /// Player is in zoneBlood
        /// </summary>
        Blood = 8,
        /// <summary>
        /// Player is in zoneCandle
        /// </summary>
        Candle = 9
    }
}
