using System;
using System.Security.Permissions;
using CustomNPC.EventSystem;

namespace CustomNPC.Plugins
{
    public abstract class NPCPlugin : MarshalByRefObject, IPlugin
    {
        private bool _disposed;

        protected NPCPlugin(IEventRegister register, DefinitionManager definitions)
        {
            Register = register;
            Definitions = definitions;
        }

        ~NPCPlugin()
        {
            Dispose(false);
        }

        public abstract string Name { get; }

        public abstract string[] Authors { get; }

        public abstract Version Version { get; }

        protected IEventRegister Register { get; private set; }

        protected DefinitionManager Definitions { get; private set; }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Initialize()
        {
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                }

                _disposed = true;
            }
        }
    }
}
