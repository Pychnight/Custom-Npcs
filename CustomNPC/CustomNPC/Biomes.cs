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
    public enum BiomeTypes
    {
        /// <summary>
        /// Doesn't Spawn in Biomes
        /// </summary>
        None = 0,

        /// <summary>
        /// Player is in noZone
        /// </summary>
        Grass = 1 << 0,

        /// <summary>
        /// Player is in Corruption
        /// </summary>
        Corruption = 1 << 1,

        /// <summary>
        /// Player is in Crimsion
        /// </summary>
        Crimsion = 1 << 2,

        /// <summary>
        /// Player is in Crimsion
        /// </summary>
        Desert = 1 << 3,

        /// <summary>
        /// Player is in zoneDungeon
        /// </summary>
        Dungeon = 1 << 4,

        /// <summary>
        /// Player is in Glowshroom
        /// </summary>
        Glowshroom = 1 << 5,

        /// <summary>
        /// Player is in zoneMeteor
        /// </summary>
        Meteor = 1 << 6,

        /// <summary>
        /// Player is in zoneHoly
        /// </summary>
        Holy = 1 << 7,

        /// <summary>
        /// Player is in zoneJungle
        /// </summary>
        Jungle = 1 << 8,

        /// <summary>
        /// Player is in Peace Candle
        /// </summary>
        PeaceCandle = 1 << 9,

        /// <summary>
        /// Player is in zoneSnow
        /// </summary>
        Snow = 1 << 10,

        /// <summary>
        /// Player is in TowerNebula
        /// </summary>
        TowerNebula = 1 << 11,

        /// <summary>
        /// Player is in TowerSolar
        /// </summary>
        TowerSolar = 1 << 12,

        /// <summary>
        /// Player is in TowerStardust
        /// </summary>
        TowerStardust = 1 << 13,

        /// <summary>
        /// Player is in TowerVortex
        /// </summary>
        TowerVortex = 1 << 14,

        /// <summary>
        /// Player is in UndergroundDesert
        /// </summary>
        UndergroundDesert = 1 << 15,

        /// <summary>
        /// Player is in zoneWaterCandle
        /// </summary>
        WaterCandle = 1 << 16,

        /// <summary>
        /// Player is in Beach Zone
        /// </summary>
        Beach = 1 << 17,

        /// <summary>
        /// Player is in a Sandstorm
        /// </summary>
        Sandstorm = 1 << 23
    }
}
