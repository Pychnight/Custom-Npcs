using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomNPC
{
    internal static class NPCManager
    {
        internal static CustomNPCVars[] NPCs = new CustomNPCVars[200];
        internal static CustomNPCData Data = new CustomNPCData();

        internal static void LoadFrom(DefinitionManager definitions)
        {
            Data.LoadFrom(definitions);
        }
    }
}
