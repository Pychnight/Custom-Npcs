using Microsoft.Xna.Framework;

namespace CustomNPC.EventSystem.Events
{
    public class NpcKilledEvent
    {
        public int NpcIndex { get; set; }

        public int PlayerIndex { get; set; }

        public int Damage { get; set; }

        public float Knockback { get; set; }

        public byte Direction { get; set; }

        public bool CriticalHit { get; set; }

        public Vector2 LastPosition { get; set; }
    }
}
