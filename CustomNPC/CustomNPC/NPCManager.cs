using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using Terraria;
using TShockAPI;
using TShockAPI.DB;

namespace CustomNPC
{
    public static class NPCManager
    {
        public static BiomeTypes[] availableBiomes = Enum.GetValues(typeof(BiomeTypes)).Cast<BiomeTypes>().Where(x => x != BiomeTypes.None).ToArray();
        private static Random rand = new Random();
        internal static CustomNPCVars[] NPCs = new CustomNPCVars[200];
        public static CustomNPCData Data = new CustomNPCData();

        internal static void LoadFrom(DefinitionManager definitions)
        {
            Data.LoadFrom(definitions);
        }

        internal static void SpawnMobsInBiomeAndRegion()
        {
            //loop through all players
            foreach (TSPlayer player in TShock.Players)
            {
                //check if they exist or are connected
                if (player != null && player.ConnectionAlive)
                {
                    //Log.ConsoleInfo("{0} - Checking spawn for player", player.Name);
                    //check all biome spawns
                    BiomeTypes biomes = player.GetCurrentBiomes();
                    foreach (BiomeTypes biome in availableBiomes.Where(x => biomes.HasFlag(x)))
                    {
                        //Log.ConsoleInfo("{0} - Checking biome for player", biome.ToString());
                        // get list of mobs that can be spawned in that biome
                        List<Tuple<string, CustomNPCSpawning>> biomeSpawns;
                        if (Data.BiomeSpawns.TryGetValue(biome, out biomeSpawns))
                        {
                            foreach (Tuple<string, CustomNPCSpawning> obj in biomeSpawns)
                            {
                                //Log.ConsoleInfo("{0} - Checking mob spawn", obj.Item1);
                                // check spawn conditions
                                if (!CheckSpawnConditions(obj.Item2.spawnConditions))
                                {
                                    //Log.ConsoleInfo("False Conditions");
                                    continue;
                                }
                                CustomNPCDefinition customnpc = Data.GetNPCbyID(obj.Item1);
                                // make sure not spawning more then maxSpawns
                                if (customnpc.maxSpawns != -1 && customnpc.currSpawnsVar >= customnpc.maxSpawns)
                                {
                                    continue;
                                }
                                // get the last spawn attempt
                                DateTime lastSpawnAttempt;
                                if (!Data.LastSpawnAttempt.TryGetValue(customnpc.customID, out lastSpawnAttempt))
                                {
                                    lastSpawnAttempt = default(DateTime);
                                    Data.LastSpawnAttempt[customnpc.customID] = lastSpawnAttempt;
                                }

                                if ((DateTime.Now - lastSpawnAttempt).TotalSeconds >= obj.Item2.spawnRate)
                                {
                                    // check spawn chance
                                    if (NPCManager.Chance(obj.Item2.spawnChance))
                                    {
                                        // check spawn method
                                        if (obj.Item2.useTerrariaSpawn)
                                        {
                                            // all checks completed spawn mob
                                            int npcid = SpawnMobAroundPlayer(player, customnpc);
                                            if (npcid != -1)
                                            {
                                                Main.npc[npcid].target = player.Index;
                                                Data.LastSpawnAttempt[customnpc.customID] = DateTime.Now;
                                                customnpc.currSpawnsVar++;
                                            }                                            
                                        }
                                        else
                                        {
                                            // all checks completed spawn mob
                                            int spawnX;
                                            int spawnY;
                                            TShock.Utils.GetRandomClearTileWithInRange(player.TileX, player.TileY, 50, 50, out spawnX, out spawnY);
                                            int npcid = SpawnNPCAtLocation((spawnX * 16) + 8, spawnY * 16, customnpc);
                                            if (npcid == -1)
                                                continue;

                                            Data.LastSpawnAttempt[customnpc.customID] = DateTime.Now;
                                            Main.npc[npcid].target = player.Index;
                                            customnpc.currSpawnsVar++;
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // then check regions as well
                    Rectangle playerRectangle = new Rectangle(player.TileX, player.TileY, player.TPlayer.width, player.TPlayer.height);
                    foreach (Region obj in Data.RegionSpawns.Keys.Select(name => TShock.Regions.GetRegionByName(name)).Where(region => region != null && region.InArea(playerRectangle)))
                    {
                        List<Tuple<string, CustomNPCSpawning>> regionSpawns;
                        if (Data.RegionSpawns.TryGetValue(obj.Name, out regionSpawns))
                        {
                            foreach (Tuple<string, CustomNPCSpawning> obj2 in regionSpawns)
                            {
                                if (!CheckSpawnConditions(obj2.Item2.spawnConditions))
                                    continue;

                                CustomNPCDefinition customnpc = Data.GetNPCbyID(obj2.Item1);
                                // make sure not spawning more then maxSpawns
                                if (customnpc.maxSpawns != -1 && customnpc.currSpawnsVar >= customnpc.maxSpawns)
                                {
                                    continue;
                                }
                                // get the last spawn attempt
                                DateTime lastSpawnAttempt;
                                if (!Data.LastSpawnAttempt.TryGetValue(customnpc.customID, out lastSpawnAttempt))
                                {
                                    lastSpawnAttempt = default(DateTime);
                                    Data.LastSpawnAttempt[customnpc.customID] = lastSpawnAttempt;
                                }

                                if ((DateTime.Now - lastSpawnAttempt).TotalSeconds >= obj2.Item2.spawnRate)
                                {
                                    Data.LastSpawnAttempt[customnpc.customID] = DateTime.Now;

                                    if (NPCManager.Chance(obj2.Item2.spawnChance))
                                    {
                                        int spawnX;
                                        int spawnY;
                                        TShock.Utils.GetRandomClearTileWithInRange(player.TileX, player.TileY, 50, 50, out spawnX, out spawnY);
                                        int npcid = SpawnNPCAtLocation((spawnX * 16) + 8, spawnY * 16, customnpc);
                                        if (npcid == -1)
                                            continue;

                                        Main.npc[npcid].target = player.Index;
                                        customnpc.currSpawnsVar++;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        internal static bool CheckSpawnConditions(SpawnConditions conditions)
        {
            if (conditions == SpawnConditions.None)
            {
                //Log.ConsoleInfo("Failed on None");
                return false;
            }
            if (conditions.HasFlag(SpawnConditions.BloodMoon) && !Main.bloodMoon)
            {
                //Log.ConsoleInfo("Failed on BloodMoon");
                return false;
            }
            if (conditions.HasFlag(SpawnConditions.Eclipse) && !Main.eclipse)
            {
                //Log.ConsoleInfo("Failed on Eclipse");
                return false;
            }
            if (conditions.HasFlag(SpawnConditions.DayTime) && !Main.dayTime)
            {
                //Log.ConsoleInfo("Failed on DayTime");
                return false;
            }
            if (conditions.HasFlag(SpawnConditions.NightTime) && Main.dayTime)
            {
                //Log.ConsoleInfo("Failed on NightTime");
                return false;
            }
            if (conditions.HasFlag(SpawnConditions.Day) && (!Main.dayTime || (Main.dayTime && Main.time <= 150.0 && Main.time >= 26999.0)))
            {
                //Log.ConsoleInfo("Failed on Day");
                return false;
            }
            if (conditions.HasFlag(SpawnConditions.Noon) && (!Main.dayTime || (Main.dayTime && Main.time <= 16200.0 && Main.time >= 32400.0)))
            {
                //Log.ConsoleInfo("Failed on Noon");
                return false;
            }
            if (conditions.HasFlag(SpawnConditions.Night) && (Main.dayTime || (!Main.dayTime && Main.time <= 27000.0 && Main.time >= 54000.0)))
            {
                //Log.ConsoleInfo("Failed on Night");
                return false;
            }
            if (conditions.HasFlag(SpawnConditions.Midnight) && (Main.dayTime || (!Main.dayTime && Main.time <= 16200.0 && Main.time >= 32400.0)))
            {
                //Log.ConsoleInfo("Failed on Midnight");
                return false;
            }
            if (conditions.HasFlag(SpawnConditions.Raining) && !Main.raining)
            {
                //Log.ConsoleInfo("Failed on Raining");
                return false;
            }
            return true;
        }

        public static CustomNPCVars GetCustomNPCByIndex(int index)
        {
            // do a short range check here...
            if (index < 0 || index >= NPCs.Length)
                return null;

            return NPCs[index];
        }

        /// <summary>
        /// Returns a list of all Custom NPC that are spawned of the given type.
        /// </summary>
        /// <param name="customnpc">The type to look for</param>
        /// <returns></returns>
        public static List<CustomNPCVars> GetAllOfType(CustomNPCDefinition customnpc)
        {
            List<CustomNPCVars> tbr = new List<CustomNPCVars>();
            for (int i = 0; i < NPCs.Length; i++)
            {
                CustomNPCVars v = NPCs[i];
                if (v == null) continue;

                if (v.customNPC == customnpc) tbr.Add(v);
            }

            return tbr;
        }

        public static int SpawnNPCAtLocation(int x, int y, CustomNPCDefinition customnpc)
        {
            int npcid = NPC.NewNPC(x, y, customnpc.customBase.type);
            if (npcid == 200)
            {
                return -1;
            }
            Data.ConvertNPCToCustom(npcid, customnpc);
            DateTime[] dt = null;
            if (customnpc.customProjectiles != null)
            {
                dt = Enumerable.Repeat(DateTime.Now, customnpc.customProjectiles.Count).ToArray();
            }
            NPCs[npcid] = new CustomNPCVars(customnpc, dt, Main.npc[npcid]);

            TSPlayer.All.SendData(PacketTypes.NpcUpdate, "", npcid);
            return npcid;
        }

        public static int SpawnNPCAroundNPC(int npcindex, ShotTile shottile, CustomNPCDefinition customnpc)
        {
            NPC npc = Main.npc[npcindex];
            if (npc == null)
                return -1;
            int x = (int)(npc.position.X + shottile.X);
            int y = (int)(npc.position.Y + shottile.Y);
            int npcid = NPC.NewNPC(x, y, customnpc.customBase.type);
            if (npcid == 200)
            {
                return -1;
            }
            Data.ConvertNPCToCustom(npcid, customnpc);
            DateTime[] dt = null;
            if (customnpc.customProjectiles != null)
            {
                dt = Enumerable.Repeat(DateTime.Now, customnpc.customProjectiles.Count).ToArray();
            }
            NPCs[npcid] = new CustomNPCVars(customnpc, dt, Main.npc[npcid]);

            TSPlayer.All.SendData(PacketTypes.NpcUpdate, "", npcid);
            return npcid;
        }

        public static int SpawnMobAroundPlayer(TSPlayer player, CustomNPCDefinition definition)
        {
            const int SpawnSpaceX = 3;
            const int SpawnSpaceY = 3;

            // search for a location
            int screenTilesX = (int)(NPC.sWidth / 16f);
            int screenTilesY = (int)(NPC.sHeight / 16f);
            int spawnRangeX = (int)(screenTilesX * 0.7);
            int spawnRangeY = (int)(screenTilesY * 0.7);
            int safeRangeX = (int)(screenTilesX * 0.52);
            int safeRangeY = (int)(screenTilesY * 0.52);

            Vector2 position = player.TPlayer.position;
            int playerTileX = (int)(position.X / 16f);
            int playerTileY = (int)(position.Y / 16f);

            int spawnRangeMinX = Math.Max(0, Math.Min(Main.maxTilesX, playerTileX - spawnRangeX));
            int spawnRangeMaxX = Math.Max(0, Math.Min(Main.maxTilesX, playerTileX + spawnRangeX));
            int spawnRangeMinY = Math.Max(0, Math.Min(Main.maxTilesY, playerTileY - spawnRangeY));
            int spawnRangeMaxY = Math.Max(0, Math.Min(Main.maxTilesY, playerTileY + spawnRangeY));

            int safeRangeMinX = Math.Max(0, Math.Min(Main.maxTilesX, playerTileX - safeRangeX));
            int safeRangeMaxX = Math.Max(0, Math.Min(Main.maxTilesX, playerTileX + safeRangeX));
            int safeRangeMinY = Math.Max(0, Math.Min(Main.maxTilesY, playerTileY - safeRangeY));
            int safeRangeMaxY = Math.Max(0, Math.Min(Main.maxTilesY, playerTileY + safeRangeY));

            int spawnX = 0;
            int spawnY = 0;

            bool found = false;
            int attempts = 0;
            while (attempts < 50)
            {
                int testX = rand.Next(spawnRangeMinX, spawnRangeMaxX);
                int testY = rand.Next(spawnRangeMinY, spawnRangeMaxY);

                Tile testTile = Main.tile[testX, testY];
                if (testTile.nactive() && Main.tileSolid[testTile.type])
                {
                    attempts++;
                    continue;
                }

                if (!Main.wallHouse[testTile.wall])
                {
                    for (int y = testY; y < Main.maxTilesY; y++)
                    {
                        Tile test = Main.tile[testX, y];
                        if (test.nactive() && Main.tileSolid[test.type])
                        {
                            if (testX < safeRangeMinX || testX > safeRangeMaxX || y < safeRangeMinY || y > safeRangeMaxY)
                            {
                                spawnX = testX;
                                spawnY = y;
                                found = true;
                                break;
                            }
                        }
                    }

                    if (!found)
                    {
                        attempts++;
                    }

                    int spaceMinX = spawnX - (SpawnSpaceX / 2);
                    int spaceMaxX = spawnX + (SpawnSpaceX / 2);
                    int spaceMinY = spawnY - SpawnSpaceY;
                    int spaceMaxY = spawnY;
                    if (spaceMinX < 0 || spaceMaxX > Main.maxTilesX)
                    {
                        attempts++;
                        continue;
                    }

                    if (spaceMinY < 0 || spaceMaxY > Main.maxTilesY)
                    {
                        attempts++;
                        continue;
                    }

                    if (found)
                    {
                        for (int x = spaceMinX; x < spaceMaxX; x++)
                        {
                            for (int y = spaceMinY; y < spaceMaxY; y++)
                            {
                                if (Main.tile[x, y].nactive() && Main.tileSolid[Main.tile[x, y].type])
                                {
                                    found = false;
                                    break;
                                }

                                if (Main.tile[x, y].lava())
                                {
                                    found = false;
                                    break;
                                }
                            }
                        }

                        if (!found)
                        {
                            attempts++;
                            continue;
                        }
                    }

                    if (spawnX >= safeRangeMinX && spawnX <= safeRangeMaxX)
                    {
                        if (!found)
                        {
                            attempts++;
                            continue;
                        }
                    }
                }
            }

            if (found)
            {
                int npcid = NPC.NewNPC((spawnX * 16) + 8, spawnY * 16, definition.customBase.type);
                if (npcid == 200)
                    return -1;

                Data.ConvertNPCToCustom(npcid, definition);
                DateTime[] dt = null;
                if (definition.customProjectiles != null)
                {
                    dt = Enumerable.Repeat(DateTime.Now, definition.customProjectiles.Count).ToArray();
                }
                NPCs[npcid] = new CustomNPCVars(definition, dt, Main.npc[npcid]);

                TSPlayer.All.SendData(PacketTypes.NpcUpdate, "", npcid);
                return npcid;
            }

            return -1;
        }

        /// <summary>
        /// Chance system, returns true based on the percentage passed through
        /// </summary>
        /// <param name="percentage">At most 2 decimal points</param>
        /// <returns></returns>
        public static bool Chance(double percentage)
        {
            return rand.NextDouble() * 100 <= percentage;
        }

        public static void DebuffNearbyPlayers(int debuffid, int seconds, int npcindex, int distance)
        {
            NPC npc = Main.npc[npcindex];
            if (npc == null)
            {
                return;
            }
            Vector2 position = npc.position;
            foreach (TSPlayer player in PlayersNearBy(position, distance))
            {
                player.SetBuff(debuffid, seconds * 60);
            }
        }

        public static void SendPrivateMessageNearbyPlayers(string message, Color color, int npcindex, int distance)
        {
            NPC npc = Main.npc[npcindex];
            if (npc == null)
            {
                return;
            }
            Vector2 position = npc.position;
            foreach (TSPlayer player in PlayersNearBy(position, distance))
            {
                player.SendMessage(message, color);
            }
        }

        public static List<TSPlayer> PlayersNearBy(Vector2 position, int distance)
        {
            List<TSPlayer> playerlist = new List<TSPlayer>();
            foreach (TSPlayer player in TShock.Players)
            {
                if (player != null && !player.Dead && player.ConnectionAlive)
                {
                    if (Math.Abs(Vector2.Distance(player.TPlayer.position, position)) <= distance * 16)
                    {
                        playerlist.Add(player);
                    }
                }
            }
            return playerlist;
        }

        /// <summary>
        /// Checks if the NPC's current health is above the passed amount
        /// </summary>
        /// <param name="Health"></param>
        /// <returns></returns>
        public static bool HealthAbove(int npcid, int Health)
        {
            NPC mainNPC = Main.npc[npcid];
            if (mainNPC == null)
            {
                return false;
            }
            return mainNPC.life >= Health;
        }

        /// <summary>
        /// Checks if the NPC's current health is below the passed amount
        /// </summary>
        /// <param name="Health"></param>
        /// <returns></returns>
        public static bool HealthBelow(int npcid, int Health)
        {
            NPC mainNPC = Main.npc[npcid];
            if (mainNPC == null)
            {
                return false;
            }
            return mainNPC.life <= Health;
        }

        /// <summary>
        /// Checks if the NPC currently has a buff placed on them
        /// </summary>
        /// <param name="buffid"></param>
        /// <returns></returns>
        public static bool HasBuff(int npcid, int buffid)
        {
            NPC mainNPC = Main.npc[npcid];
            if (mainNPC == null)
            {
                return false;
            }
            return mainNPC.buffType.Contains(buffid);
        }

        public static void AddBuffToPlayer(int playerindex, int buffid, int seconds)
        {
            TSPlayer player = TShock.Players[playerindex];
            if (player == null)
                return;
            player.TPlayer.AddBuff(40, seconds * 60);
            player.SetBuff(buffid, seconds * 60);
        }

        public static int AliveCount(string customid)
        {
            int count = 0;
            foreach(CustomNPCVars obj in NPCs)
            {
                if (obj == null)
                    continue;
                if (obj.customNPC.customID.ToLower() == customid.ToLower())
                {
                    count++;
                }
            }
            return count;
        }

        public static class CustomNPCInvasion
        {
            public static Dictionary<string, WaveSet> WaveSets { get; set; }
            private static Timer InvasionTimer = new Timer(1000);
            private static WaveSet CurrentInvasion { get; set; }
            private static Waves CurrentWave { get; set; }
            private static int CurrentWaveIndex { get; set; }
            private static int waveSize { get; set; }
            public static bool invasionStarted = false;
            public static int WaveSize
            {
                get { return waveSize; }
                set
                {
                    waveSize = value;
                    if (value == 0)
                    {
                        NextWave();
                    }
                }
            }

            public static void NextWave()
            {
                if (CurrentInvasion == null)
                    return;

                if (CurrentInvasion.Waves.Count - 1 == CurrentWaveIndex)
                {
                    TSPlayer.All.SendInfoMessage("{0} has been defeated!", CurrentInvasion.WaveSetName);
                    StopInvasion();
                    return;
                }
                CurrentWaveIndex++;
                CurrentWave = CurrentInvasion.Waves[CurrentWaveIndex];
                WaveSize = CurrentWave.SpawnGroup.KillAmount;
                TSPlayer.All.SendInfoMessage("Wave {0}: {1} has begun!", CurrentWaveIndex + 1, CurrentWave.WaveName);
                SpawnBoss();
            }

            public static void StartInvasion(WaveSet waveset)
            {
                CurrentInvasion = waveset;
                CurrentWave = waveset.Waves[0];
                WaveSize = CurrentWave.SpawnGroup.KillAmount;
                if (CurrentWave.SpawnGroup.PlayerMultiplier)
                {
                    WaveSize *= TShock.Utils.ActivePlayers();
                }
                CurrentWaveIndex = 0;
                InvasionTimer.Elapsed += InvasionTimer_Elapsed;
                InvasionTimer.Enabled = true;
                invasionStarted = true;
                TSPlayer.All.SendInfoMessage("Invasion: {0} has started!", waveset.WaveSetName);
                SpawnBoss();
            }
            
            public static void SpawnBoss()
            {
                if (TShock.Utils.ActivePlayers() == 0)
                {
                    return;
                }
                int spawnX = Main.spawnTileX - 150;
                int spawnY = Main.spawnTileY - 150;
                Rectangle spawnRegion = new Rectangle(spawnX, spawnY, 300, 300).ToPixels();
                foreach (SpawnMinion minions in CurrentWave.SpawnGroup.SpawnMinions.Where(p => p.isBoss))
                {
                    foreach (TSPlayer player in TShock.Players.Where(x => x != null && !x.Dead && x.Active))
                    {
                        if (!NPCManager.Chance(minions.Chance))
                        {
                            continue;
                        }
                        if (!CurrentWave.SpawnAnywhere)
                        {
                            Rectangle playerFrame = new Rectangle((int)player.TPlayer.position.X, (int)player.TPlayer.position.Y, player.TPlayer.width, player.TPlayer.height);
                            if (!playerFrame.Intersects(spawnRegion))
                            {
                                continue;
                            }
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
           
            public static void StopInvasion()
            {
                TSPlayer.All.SendInfoMessage("Invasion: {0} has stopped!", CurrentInvasion.WaveSetName);
                InvasionTimer.Elapsed -= InvasionTimer_Elapsed;
                InvasionTimer.Enabled = false;
                CurrentInvasion = null;
                CurrentWave = null;
                WaveSize = 0;
                CurrentWaveIndex = 0;
                invasionStarted = false;
            }

            private static void InvasionTimer_Elapsed(object sender, ElapsedEventArgs e)
            {
                if (TShock.Utils.ActivePlayers() == 0)
                {
                    return;
                }
                int spawnX = Main.spawnTileX - 150;
                int spawnY = Main.spawnTileY - 150;
                Rectangle spawnRegion = new Rectangle(spawnX, spawnY, 300, 300).ToPixels();
                foreach (SpawnMinion minions in CurrentWave.SpawnGroup.SpawnMinions.Where(p => !p.isBoss))
                {
                    foreach (TSPlayer player in TShock.Players.Where(x => x != null && !x.Dead && x.Active))
                    {
                        if (!NPCManager.Chance(minions.Chance))
                        {
                            continue;
                        }
                        if (!CurrentWave.SpawnAnywhere)
                        {
                            Rectangle playerFrame = new Rectangle((int)player.TPlayer.position.X, (int)player.TPlayer.position.Y, player.TPlayer.width, player.TPlayer.height);
                            if (!playerFrame.Intersects(spawnRegion))
                            {
                                continue;
                            }
                        }
                        var npcdef = NPCManager.Data.GetNPCbyID(minions.MobID);
                        if (npcdef == null)
                        {
                            TShock.Log.ConsoleError("[CustomNPC]: Error! The custom mob id \"{0}\" does not exist!", minions.MobID);
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

            public static WaveSet ReturnWaveSetByName(string name)
            {
                WaveSet waveset;
                if (WaveSets.TryGetValue(name, out waveset))
                {
                    return waveset;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
