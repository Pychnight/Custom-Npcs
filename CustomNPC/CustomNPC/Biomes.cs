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
        /// Player is in noZone
        /// </summary>
        Grass = 0,
        /// <summary>
        /// Player is in zoneEvil
        /// </summary>
        Corruption = 1,
        /// <summary>
        /// Player is in zoneDungeon
        /// </summary>
        Dungeon = 2,
        /// <summary>
        /// Player is in zoneMeteor
        /// </summary>
        Meteor =  3,
        /// <summary>
        /// Player is in zoneHoly
        /// </summary>
        Holy = 4,
        /// <summary>
        /// Player is in zoneJungle
        /// </summary>
        Jungle = 5,
        /// <summary>
        /// Player is in zoneSnow
        /// </summary>
        Snow = 6,
        /// <summary>
        /// Player is in zoneBlood
        /// </summary>
        Blood = 7,
        /// <summary>
        /// Player is in zoneCandle
        /// </summary>
        Candle = 8
    }
}
