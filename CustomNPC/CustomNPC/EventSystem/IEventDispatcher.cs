namespace CustomNPC.EventSystem
{
    internal interface IEventDispatcher
    {
        void InvokeHandler<T>(T args, EventType type);
    }
}
