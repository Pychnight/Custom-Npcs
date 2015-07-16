namespace CustomNPC.EventSystem
{
    public enum EventType
    {
        None = 0,

        PluginUpdate,
        PostPluginUpdate,

        NpcUpdate,
        PostNpcUpdate,

        NpcCollision,
        NpcDamage,
        NpcKill,
        ServerChat
    }
}
