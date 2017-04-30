using System;
using System.Collections.Generic;
using System.Security.Permissions;

namespace CustomNPC
{
    public class DefinitionManager : MarshalByRefObject
    {
        private IDictionary<string, CustomNPCDefinition> _definitions;

        public DefinitionManager()
        {
            _definitions = new Dictionary<string, CustomNPCDefinition>(StringComparer.CurrentCultureIgnoreCase);
        }

        public IDictionary<string, CustomNPCDefinition> Definitions
        {
            get { return _definitions; }
        }

        [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.Infrastructure)]
        public override object InitializeLifetimeService()
        {
            return null;
        }

        public void Add(CustomNPCDefinition definition)
        {
            if (definition == null)
                throw new ArgumentNullException("definition");

            Definitions.Add(definition.customID, definition);
        }

        public bool Contains(string id)
        {
            return Definitions.ContainsKey(id);
        }

        public bool TryGetDefinition(string id, out CustomNPCDefinition definition)
        {
            return Definitions.TryGetValue(id, out definition);
        }
    }
}
