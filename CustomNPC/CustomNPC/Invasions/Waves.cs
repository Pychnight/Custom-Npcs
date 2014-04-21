namespace CustomNPC.Invasions
{
    public class Waves
    {
        public string WaveName { get; set; }
        public SpawnsGroups SpawnGroup { get; set; }
        public bool SpawnAnywhere { get; set; }
        public Waves(string wavename, SpawnsGroups spawngroup, bool spawnanywhere = false)
        {
            WaveName = wavename;
            SpawnGroup = spawngroup;
            SpawnAnywhere = spawnanywhere;
        }
    }
}