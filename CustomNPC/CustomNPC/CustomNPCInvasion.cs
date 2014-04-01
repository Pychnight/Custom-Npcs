using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace CustomNPC
{
    public class CustomNPCInvasion
    {
        private CustomNPCPlugin Main;
        private Timer InvasionTimer = new Timer(1000);
        private Waves CurrentWave { get; set; }

        public void StartInvasion()
        {
            InvasionTimer.Elapsed += InvasionTimer_Elapsed;
            InvasionTimer.Enabled = true;
        }

        public void StopInvasion()
        {
            InvasionTimer.Elapsed -= InvasionTimer_Elapsed;
            InvasionTimer.Enabled = false;
        }

        void InvasionTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            throw new NotImplementedException();
        }

        public WaveSet ReturnWaveSetByName(string name)
        {
            WaveSet waveset;
            if (Main.ConfigObj.WaveSets.TryGetValue(name, out waveset))
            {
                return waveset;
            }
            else
            {
                return null;
            }
        }
        

        public CustomNPCInvasion(CustomNPCPlugin main)
        {
            Main = main;
        }
    }
    public class SpawnMinion
    {
        public int MobID { get; set; }
        public int Chance { get; set; }
        public bool UnitToCheckKillAmount { get; set; }
        public SpawnMinion(int mobid, int chance, bool unittocheckkillamount = false)
        {
            MobID = mobid;
            Chance = chance;
            UnitToCheckKillAmount = unittocheckkillamount;
        }
    }
    public class SpawnBoss
    {
        public int MobID { get; set; }
        public int Chance { get; set; }
        public bool UnitToCheckKillAmount { get; set; }
        public SpawnBoss(int mobid, int chance, bool unittokill = false, bool unittocheckkillamount = false)
        {
            MobID = mobid;
            Chance = chance;
            UnitToCheckKillAmount = unittocheckkillamount;
        }
    }
    public class SpawnsGroups
    {
        public int KillAmount { get; set; }
        public List<SpawnBoss> SpawnsBosses { get; set; }
        public List<SpawnMinion> SpawnMinions { get; set; }
        public SpawnsGroups(bool proceedbykillamount, List<SpawnBoss> spawnbosses, List<SpawnMinion> spawnminions, int killamount)
        {
            KillAmount = killamount;
            SpawnsBosses = spawnbosses;
            SpawnMinions = spawnminions;
        }
    }
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
    public class WaveSet
    {
        public string WaveName { get; set; }
        public List<Waves> Waves { get; set; }
        public WaveSet(string wavename, List<Waves> waves)
        {
            WaveName = wavename;
            Waves = waves;
        }
    }
}
