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
    public enum SpawnConditions : byte
    {
        /// <summary>
        /// Spawn anytime
        /// </summary>
        None = 0,
        /// <summary>
        /// Allowed in Day time (Main.dayTime == true)
        /// </summary>
        DayTime = 1,
        /// <summary>
        /// Allowed in Night time (Main.dayTime == true)
        /// </summary>
        NightTime = 2,
        /// <summary>
        /// Allowed in Eclipse
        /// </summary>
        Eclipse = 3,
        /// <summary>
        /// Allowed in Bloodmoon
        /// </summary>
        Bloodmoon = 4,
        /// <summary>
        /// Allowed when Raining
        /// </summary>
        Raining = 6,
        /// <summary>
        /// Allowed during day (Main.dayTime == true && Main.time => 150.0 && Main.time <= 26999.0)
        /// </summary>
        Day = 7,
        /// <summary>
        /// Allowed during night (Main.dayTime == false && Main.time => 0.0 && Main.16200)
        /// </summary>
        Night = 8,
        /// <summary>
        /// Allowed during noon (Main.dayTime == true && Main.time => 27000.0 && Main.time <= 54000)
        /// </summary>
        Noon = 9,
        /// <summary>
        /// Allowed during midnight (Main.dayTime == false && Main.time => 16200 && Main.time <= 32400)
        /// </summary>
        Midnight = 10
    }
}
