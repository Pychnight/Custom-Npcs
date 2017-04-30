using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomNPC
{
    public class CustomNPCLoot
    {
        public int itemID { get; set; }
        public List<int> itemPrefix { get; set; }
        public int itemStack { get; set; }
        public double itemDropChance { get; set; }

        public CustomNPCLoot(int itemid, List<int> itemprefix, int itemstack, double itemdropchance)
        {
            itemID = itemid;
            itemPrefix = itemprefix;
            itemStack = itemstack;
            itemDropChance = itemdropchance;
        }
    }
}
