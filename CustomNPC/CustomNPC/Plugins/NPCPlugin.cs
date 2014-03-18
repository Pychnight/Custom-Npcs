using System;

namespace CustomNPC.Plugins
{
    public abstract class NPCPlugin : MarshalByRefObject, IPlugin
    {
        private bool _disposed;

        ~NPCPlugin()
        {
            Dispose(false);
        }

        public abstract string Name { get; }

        public abstract string[] Authors { get; }

        public abstract Version Version { get; }

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
