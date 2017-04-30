using CustomNPC.Plugins;

namespace CustomNPC.EventSystem
{
    public delegate void PluginEventHandler<in T>(T args);

    public interface IEventRegister
    {
        void RegisterHandler<T>(IPlugin plugin, PluginEventHandler<T> handler, EventType type);

        void DeregisterHandler(IPlugin plugin, EventType type);
    }
}
