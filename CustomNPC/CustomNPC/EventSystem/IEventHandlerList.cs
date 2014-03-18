using CustomNPC.Plugins;

namespace CustomNPC.EventSystem
{
    internal interface IEventHandlerList
    {
        void Register(IPlugin plugin, object t);

        void Deregister(IPlugin plugin);

        void Invoke(object args);
    }
}
