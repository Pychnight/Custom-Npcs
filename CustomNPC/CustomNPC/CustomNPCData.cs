using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomNPC
{
    internal class CustomNPCData
    {
        internal Dictionary<string, CustomNPC> CustomNPCs = new Dictionary<string, CustomNPC>();

        /// <summary>
        /// Returns CustomNPC obj by custom ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal CustomNPC GetNPCbyID(string id)
        {
            foreach (CustomNPC obj in CustomNPCs.Values)
            {
                if (obj.customID == id)
                {
                    return obj;
                }
            }
            return null;
        }
    }
}
