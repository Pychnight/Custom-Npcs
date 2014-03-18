using System.Collections.Generic;
using CustomNPC.Plugins;

namespace CustomNPC.EventSystem
{
    internal class CustomNPCEventHandlerList<T> : IEventHandlerList
    {
        private readonly IDictionary<IPlugin, PluginEventHandler<T>> _handlers;

        public CustomNPCEventHandlerList()
        {
            _handlers = new Dictionary<IPlugin, PluginEventHandler<T>>();
        }

        public IDictionary<IPlugin, PluginEventHandler<T>> Handlers
        {
            get { return _handlers; }
        }

        public void Register(IPlugin plugin, object t)
        {
            lock (Handlers)
            {
                Handlers[plugin] = (PluginEventHandler<T>)t;
            }
        }

        public void Deregister(IPlugin plugin)
        {
            lock (Handlers)
            {
                if (Handlers.ContainsKey(plugin))
                {
                    Handlers.Remove(plugin);
                }
            }
        }

        public void Invoke(object args)
        {
            lock (Handlers)
            {
                foreach (var handler in Handlers.Values)
                {
                    handler.Invoke((T)args);
                }
            }
        }
    }
}
