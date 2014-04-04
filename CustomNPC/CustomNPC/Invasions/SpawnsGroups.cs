using System.Collections.Generic;

namespace CustomNPC.Invasions
{
    public class SpawnsGroups
    {
        public int KillAmount { get; set; }
        public bool PlayerMultiplier { get; set; }
        public List<SpawnMinion> SpawnMinions { get; set; }
        public SpawnsGroups(bool proceedbykillamount, bool playermultiplier, List<SpawnMinion> spawnminions, int killamount)
        {
            KillAmount = killamount;
            PlayerMultiplier = playermultiplier;
            SpawnMinions = spawnminions;
        }
    }
}