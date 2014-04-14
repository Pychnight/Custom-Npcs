namespace CustomNPC.Invasions
{
    public class Waves
    {
        public string WaveName { get; set; }
        public SpawnsGroups SpawnGroup { get; set; }
        public Waves(string wavename, SpawnsGroups spawngroup)
        {
            WaveName = wavename;
            SpawnGroup = spawngroup;
        }
    }
}