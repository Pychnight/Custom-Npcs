namespace CustomNPC.EventSystem.Events
{
    public class NpcCollisionEvent
    {
        public int NpcIndex { get; set; }

        public int PlayerIndex { get; set; }
    }
}
