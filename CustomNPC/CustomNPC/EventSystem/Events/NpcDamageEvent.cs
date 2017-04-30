namespace CustomNPC.EventSystem.Events
{
    public class NpcDamageEvent
    {
        public int NpcIndex { get; set; }

        public int PlayerIndex { get; set; }

        public int Damage { get; set; }

        public float Knockback { get; set; }

        public byte Direction { get; set; }

        public bool CriticalHit { get; set; }

        public int NpcHealth { get; set; }
    }
}
