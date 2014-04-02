using System.Linq;
using System.Timers;
using Terraria;
using TShockAPI;

namespace CustomNPC.Invasions
{
    public class CustomNPCInvasion
    {
        private CustomNPCPlugin plugin;
        private Timer invasionTimer = new Timer(1000);
        private WaveSet currentInvasion;
        private Waves currentWave;
        private int currentWaveIndex;
        private int waveSize;

        public int WaveSize
        {
            get { return waveSize; }
            set
            {
                waveSize = value;
                if (waveSize == 0)
                {

                }
            }
        }

        public void NextWave()
        {
            if (currentInvasion.Waves.Count - 1 == currentWaveIndex)
            {

            }
        }

        public void StartInvasion(WaveSet waveset)
        {
            currentInvasion = waveset;
            WaveSize = currentWave.SpawnGroup.KillAmount;
            if (currentWave.SpawnGroup.PlayerMultiplier)
            {
                WaveSize *= TShock.Utils.ActivePlayers();
            }
            currentWave = waveset.Waves[0];
            currentWaveIndex = 0;
            invasionTimer.Elapsed += InvasionTimer_Elapsed;
            invasionTimer.Enabled = true;
        }

        public void StopInvasion()
        {
            currentInvasion = null;
            currentWave = null;
            WaveSize = 0;
            currentWaveIndex = 0;
            invasionTimer.Elapsed -= InvasionTimer_Elapsed;
            invasionTimer.Enabled = false;
        }

        private void InvasionTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (TShock.Utils.ActivePlayers() == 0)
                return;

            int spawnX = Main.spawnTileX - 150;
            int spawnY = Main.spawnTileY - 150;
            Rectangle spawnRegion = new Rectangle(spawnX, spawnY, 300, 300).ToPixels();
            foreach (SpawnMinion minions in currentWave.SpawnGroup.SpawnMinions)
            {
                foreach (TSPlayer player in TShock.Players.Where(x => x != null && !(x.Dead || !x.Active)))
                {
                    if (!NPCManager.Chance(minions.Chance))
                        continue;

                    var playerframe = new Rectangle((int)player.TPlayer.position.X, (int)player.TPlayer.position.Y, player.TPlayer.width, player.TPlayer.height);
                    if (!playerframe.Intersects(spawnRegion))
                        continue;

                    var npcdef = NPCManager.Data.GetNPCbyID(minions.MobID);
                    if (npcdef == null)
                    {
                        Log.ConsoleError("[CustomNPC]: Error! The custom mob id \"{0}\" does not exist!", minions.MobID);
                        continue;
                    }

                    if (minions.SpawnConditions != SpawnConditions.None && NPCManager.CheckSpawnConditions(minions.SpawnConditions))
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

                    int mobid;
                    do
                    {
                        mobid = NPCManager.SpawnMobAroundPlayer(player, npcdef);
                    }
                    while (mobid == -1);

                    NPCManager.GetCustomNPCByIndex(mobid).isInvasion = true;                   
                }
            }
        }

        public WaveSet GetWaveSetByName(string name)
        {
            WaveSet waveset;
            if (!plugin.ConfigObj.WaveSets.TryGetValue(name, out waveset))
                return null;

            return waveset;
        }
        
        public CustomNPCInvasion(CustomNPCPlugin main)
        {
            plugin = main;
        }
    }
}