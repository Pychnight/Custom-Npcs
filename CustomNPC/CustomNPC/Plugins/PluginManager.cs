using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Security.Permissions;
using CustomNPC.EventSystem;
using TShockAPI;

namespace CustomNPC.Plugins
{
    public sealed class PluginManager<TPlugin> : MarshalByRefObject
        where TPlugin : IPlugin
    {
        private IEventRegister _eventRegister;
        private PluginDiscoverer<TPlugin> _discoverer;
        private IList<TPlugin> _plugins;

        public PluginManager()
        {
            _eventRegister = new EventManager();
            _discoverer = new PluginDiscoverer<TPlugin>();
            _plugins = new List<TPlugin>();
        }

        public static string PluginPath
        {
            get { return Path.Combine(TShock.SavePath, "CustomNPCs"); }
        }

        public IEventRegister EventRegister
        {
            get { return _eventRegister; }
        }

        public IReadOnlyList<TPlugin> Plugins
        {
            get { return new ReadOnlyCollection<TPlugin>(_plugins); }
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void Load(AppDomain domain)
        {
            if (!Directory.Exists(PluginPath))
            {
                Directory.CreateDirectory(PluginPath);
            }

            foreach (var file in Directory.GetFiles(PluginPath, "*.dll"))
            {
                string[] typeNames = _discoverer.GetPluginTypeNames(file);
                foreach (string typeName in typeNames)
                {
                    object[] args =
                    {
                        EventRegister
                    };

                    var plugin = (TPlugin)domain.CreateInstanceFromAndUnwrap(file, typeName, true, BindingFlags.Default, null, args, null, null);
                    _plugins.Add(plugin);
                }
            }

            foreach (TPlugin plugin in _plugins)
            {
                plugin.Initialize();
            }
        }

        public void Reload()
        {
        }

        public void Unload()
        {
            foreach (TPlugin plugin in _plugins)
            {
                plugin.Dispose();
            }
        }
    }
}
