using System;

namespace CustomNPC.Plugins
{
    public interface IPlugin : IDisposable
    {
        string Name { get; }

        string[] Authors { get; }

        Version Version { get; }

        void Initialize();
    }
}