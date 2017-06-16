using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CustomNPC
{
    /// <summary>
    /// Spawn Conditions
    /// </summary>
    [Flags]
    public enum SpawnConditions
    {
        /// <summary>
        /// Spawn anytime
        /// </summary>
        None = 0,

        /// <summary>
        /// Allowed in Day time (Main.dayTime == true)
        /// </summary>
        DayTime = 1 << 0,

        /// <summary>
        /// Allowed in Night time (Main.dayTime == true)
        /// </summary>
        NightTime = 1 << 1,

        /// <summary>
        /// Allowed in Eclipse
        /// </summary>
        Eclipse = 1 << 2,

        /// <summary>
        /// Allowed in Bloodmoon
        /// </summary>
        BloodMoon = 1 << 3,

        /// <summary>
        /// Allowed in SnowMoon
        /// </summary>
        SnowMoon = 1 << 10,

        /// <summary>
        /// Allowed when Raining
        /// </summary>
        Raining = 1 << 4,

        /// <summary>
        /// Allowed when Raining Slime
        /// </summary>
        SlimeRaining = 1 << 9,

        /// <summary>
        /// Allowed during day
        /// </summary>
        /// <remarks>
        /// <code>(Main.dayTime == true && Main.time => 150.0 && Main.time <= 26999.0)</code>
        /// </remarks>
        Day = 1 << 5,

        /// <summary>
        /// Allowed during night
        /// </summary>
        /// <remarks>
        /// <code>(Main.dayTime == false && Main.time => 0.0 && Main.16200)</code>
        /// </remarks>
        Night = 1 << 6,

        /// <summary>
        /// Allowed during noon
        /// </summary>
        /// <remarks>
        /// <code>(Main.dayTime == true && Main.time => 27000.0 && Main.time <= 54000)</code>
        /// </remarks>
        Noon = 1 << 7,

        /// <summary>
        /// Allowed during midnight
        /// </summary>
        /// <remarks>
        /// <code>(Main.dayTime == false && Main.time => 16200 && Main.time <= 32400)</code>
        /// </remarks>
        Midnight = 1 << 8
    }
}
