using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TShockAPI;

namespace CustomNPC
{
    internal class CustomNPCUtils
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
    }
}
