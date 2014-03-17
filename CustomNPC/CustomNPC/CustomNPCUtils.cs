using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomNPC
{
    internal class CustomNPCUtils
    {
        public Random rand = new Random();

        private static readonly CustomNPCUtils instance = new CustomNPCUtils();
        private CustomNPCUtils() { }
        internal static CustomNPCUtils Instance { get { return instance; } }

        internal bool Chance(double percentage)
        {
            if (rand.Next(101) <= percentage)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal CustomNPC GetNPCbyID(string id)
        {
            return null;
        }
    }
}
