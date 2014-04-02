using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

using TShockAPI;
using Terraria;

namespace CustomNPC
{
    public class CustomNPCInvasion
    {
        private CustomNPCPlugin Plugin;
        private Timer InvasionTimer = new Timer(1000);
        private WaveSet CurrentInvasion { get; set; }
        private Waves CurrentWave { get; set; }
        public int WaveSize { get; set; }

        public void StartInvasion(WaveSet waveset)
        {
            CurrentInvasion = waveset;
            WaveSize = CurrentWave.SpawnGroup.KillAmount;
            if (CurrentWave.SpawnGroup.PlayerMultiplier)
            {
                WaveSize *= TShock.Utils.ActivePlayers();
            }
            CurrentWave = waveset.Waves[0];
            InvasionTimer.Elapsed += InvasionTimer_Elapsed;
            InvasionTimer.Enabled = true;
        }

        public void StopInvasion()
        {
            CurrentInvasion = null;
            CurrentWave = null;
            InvasionTimer.Elapsed -= InvasionTimer_Elapsed;
            InvasionTimer.Enabled = false;
        }

        private void InvasionTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (TShock.Utils.ActivePlayers() == 0)
            {
                return;
            }
            int spawnX = Main.spawnTileX - 150;
            int spawnY = Main.spawnTileY - 150;
            Rectangle SpawnRegion = new Rectangle(spawnX, spawnY, 300, 300);
            foreach(SpawnMinion minions in CurrentWave.SpawnGroup.SpawnMinions)
            {
                foreach(TSPlayer player in TShock.Players.Where(x => x != null))
                {
                    if (player.Dead || !player.Active || !NPCManager.Chance(minions.Chance))
                    {
                        continue;
                    }
                    Rectangle playerframe = new Rectangle((int)player.TPlayer.position.X, (int)player.TPlayer.position.Y, player.TPlayer.width, player.TPlayer.height);
                    if (!playerframe.Intersects(SpawnRegion))
                    {
                        continue;
                    }
                    var npcdef = NPCManager.Data.GetNPCbyID(minions.MobID);
                    if (npcdef == null)
                    {
                        Log.ConsoleError("[CustomNPC]: Error! The custom mob id \"{0}\" does not exist!", minions.MobID);
                        continue;
                    }
                    if (minions.SpawnConditions != SpawnConditions.None)
                    {
                        if (NPCManager.CheckSpawnConditions(minions.SpawnConditions))
                        {
                            continue;
                        }
                    }
                    if (minions.BiomeConditions != BiomeTypes.None)
                    {
                        BiomeTypes biomes = player.GetCurrentBiomes();
                        
                        if ((minions.BiomeConditions & biomes) == 0)
                        {
                            continue;
                        }
                    }
                    int mobid = -1;
                    while (mobid == -1)
                    {
                        mobid = NPCManager.SpawnMobAroundPlayer(player, npcdef);
                    }
                    NPCManager.GetCustomNPCByIndex(mobid).isInvasion = true;                   
                }
            }
        }

        public WaveSet ReturnWaveSetByName(string name)
        {
            WaveSet waveset;
            if (Plugin.ConfigObj.WaveSets.TryGetValue(name, out waveset))
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
            Plugin = main;
        }
    }
    public class SpawnMinion
    {
        public string MobID { get; set; }
        public double Chance { get; set; }
        public bool UnitToCheckKillAmount { get; set; }
        public bool isBoss { get; set; }
        public SpawnConditions SpawnConditions { get; set; }
        public BiomeTypes BiomeConditions { get; set; }
        public SpawnMinion(string mobid, double chance, BiomeTypes biomeconditions, SpawnConditions spawnconditions, bool isboss = false, bool unittocheckkillamount = true)
        {
            MobID = mobid;
            Chance = chance;
            SpawnConditions = spawnconditions;
            BiomeConditions = biomeconditions;
            isBoss = isboss;
            UnitToCheckKillAmount = unittocheckkillamount;
        }
    }
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
