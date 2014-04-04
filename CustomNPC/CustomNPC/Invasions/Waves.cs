namespace CustomNPC.Invasions
{
    public class Waves
    {
        public int WaveNumber { get; set; }

        public SpawnsGroups SpawnGroup { get; set; }

        public Waves(int wavenumber, SpawnsGroups spawngroup)
        {
            WaveNumber = wavenumber;
            SpawnGroup = spawngroup;
        }
    }
}