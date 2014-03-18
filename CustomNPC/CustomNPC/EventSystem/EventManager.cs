using System.Collections.Generic;
using CustomNPC.Plugins;

namespace CustomNPC.EventSystem
{
    internal class EventManager : IEventDispatcher, IEventRegister
    {
        private IDictionary<EventType, IEventHandlerList> _handlers;

        public EventManager()
        {
            _handlers = new Dictionary<EventType, IEventHandlerList>();
        }

        public IDictionary<EventType, IEventHandlerList> Handlers
        {
            get { return _handlers; }
        }

        public void InvokeHandler<T>(T args, EventType type)
        {
            IEventHandlerList handlers;
            if (Handlers.TryGetValue(type, out handlers))
            {
                handlers.Invoke(args);
            }
        }

        public void RegisterHandler<T>(IPlugin plugin, PluginEventHandler<T> handler, EventType type)
        {
            IEventHandlerList handlers;
            if (!Handlers.TryGetValue(type, out handlers))
            {
                handlers = new CustomNPCEventHandlerList<T>();
                Handlers[type] = handlers;
            }

            handlers.Register(plugin, handler);
        }

        public void DeregisterHandler(IPlugin plugin, EventType type)
        {
            IEventHandlerList handlers;
            if (Handlers.TryGetValue(type, out handlers))
            {
                handlers.Deregister(plugin);
            }
        }
    }
}
