using System;
using System.Collections.Generic;
using System.Reflection;

namespace CustomNPC.Plugins
{
    public sealed class PluginDiscoverer<TPlugin> : MarshalByRefObject
    {
        private static readonly Type PluginType = typeof(TPlugin);

        public string[] GetPluginTypeNames(string assemblyFile)
        {
            var typeNames = new List<string>();

            Assembly asm = Assembly.LoadFrom(assemblyFile);
            foreach (Type type in asm.GetTypes())
            {
                if (type.IsPublic && PluginType.IsAssignableFrom(type))
                {
                    typeNames.Add(type.FullName);
                }
            }

            return typeNames.ToArray();
        }
    }
}
