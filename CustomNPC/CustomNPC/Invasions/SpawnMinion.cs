namespace CustomNPC.Invasions
{
    public class SpawnMinion
    {
        public string MobID { get; set; }

        public double Chance { get; set; }

        public bool UnitToCheckKillAmount { get; set; }

        public bool IsBoss { get; set; }

        public SpawnConditions SpawnConditions { get; set; }

        public BiomeTypes BiomeConditions { get; set; }

        public SpawnMinion(string mobid, double chance, BiomeTypes biomeconditions, SpawnConditions spawnconditions, bool isboss = false, bool unittocheckkillamount = true)
        {
            MobID = mobid;
            Chance = chance;
            SpawnConditions = spawnconditions;
            BiomeConditions = biomeconditions;
            IsBoss = isboss;
            UnitToCheckKillAmount = unittocheckkillamount;
        }
    }
}