namespace CustomNPC.EventSystem
{
    public enum EventType
    {
        None = 0,

        PluginUpdate,
        PostPluginUpdate,

        NpcCollision,
        NpcDamage,
        NpcKill
    }
}
