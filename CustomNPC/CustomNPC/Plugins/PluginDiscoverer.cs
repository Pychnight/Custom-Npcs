using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Security.Permissions;

namespace CustomNPC.Plugins
{
    public sealed class PluginDiscoverer<TPlugin> : MarshalByRefObject
    {
        private static readonly Type PluginType = typeof(TPlugin);

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            return null;
        }

        public string[] GetPluginTypeNames(string assemblyFile)
        {
            var typeNames = new List<string>();

            Assembly asm = Assembly.Load(File.ReadAllBytes(assemblyFile));
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
