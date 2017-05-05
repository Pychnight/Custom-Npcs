using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomNPC;
using Microsoft.Xna.Framework;

namespace TestNPC
{
    public sealed class TestNPCDefinition : CustomNPCDefinition
    {
        public TestNPCDefinition()
            //define base id type for Custom NPC
            : base(21)
        {
            //add projectiles to list here
            //(Projectile ID, ShotTiles, Damage, Attack Rate (milliseconds), TargetSearch Detection, Chance, Collision Check, ai0, ai1)
            //Projectile ID = ID of Projectile
            //ShotTiles = Position shot from relative from players centre
            //Damage = how much damage it will do (this is affected by armor)
            //Attack Rate = how fast it will attack again, 1000 = every 1 second
            //Target Search = whether to fire at the first fireable person, or fire at the NPCs current target
            //Chance = 0 - 100
            //Collision check = whether to check if it the projectile will collide with a tile before firing
            //ai0 ai1 = projectile ai's - don't use unless you know what these does (does different things depending on different projectiles)
            //Example of 3 different Shot Tile methods new ShotTile(x, y) - relative to centre of the Player, where positive values for x is down, and postive values for y is right
            customProjectiles.Add(new CustomNPCProjectiles(180, new List<ShotTile>() { ShotTile.Middle }, 10, 250, true, 100));
            customProjectiles.Add(new CustomNPCProjectiles(257, new List<ShotTile>() { new ShotTile(-100, 0) }, 170, 2000, false, 10));
            customProjectiles.Add(new CustomNPCProjectiles(174, new List<ShotTile>() { ShotTile.Middle, ShotTile.MiddleLeft, ShotTile.MiddleRight }, 70, 600, true, 50));

            //Custom loot
            //(Loot ID, Prefix Lists, Stacks, Chance)
            //Loot ID = item id
            //Prefix List = list of prefixs it can drop with - if you set -1 it randomizes all prefixs, if you set a prefix it can't be set it will randomize the prefix as well
            //stacks = how many drops at once
            //chance = 0 - 100
            customNPCLoots.Add(new CustomNPCLoot(808, new List<int> { 0 }, 1, 50));
            customNPCLoots.Add(new CustomNPCLoot(806, new List<int> { 83 }, 1, 100));

            //Custom Spawning
            //(spawnrate, conditions, Terraria Spawn, Biomes, Regions, chance)
            //spawnrate = seconds
            //Conditions = when they can spawn, can define multiple, must all be true for it to run - separate them by |
            //Terraria Spawn = use terraria spawning method or use static spawning method
            //Biomes = BiomeTypes.None for no biome spawning, split by | if you want multiply biomes defined
            //Regions = string.Empty for no regions, or define regions by inputting name of region (allows only 1 region)
            //Chance = 100.0
            //Notes: Can add multiple of spawn definitions
            //customNPCSpawning.Add(new CustomNPCSpawning(2, SpawnConditions.Raining | SpawnConditions.DayTime, true, BiomeTypes.Grass, string.Empty, 100.0));
            //customNPCSpawning.Add(new CustomNPCSpawning(1, SpawnConditions.NightTime, true, BiomeTypes.Grass));
            customNPCSpawning.Add(new CustomNPCSpawning(1, SpawnConditions.Raining | SpawnConditions.DayTime, true, BiomeTypes.None, "test", 100.0));
        }

        //ID of Custom NPC - can be set to anything, this will be what is used to summon the npc in game ie/ c1, c2, c3 etc...
        public override string customID
        {
            get { return "testnpc"; }
        }

        //Name of NPC - will display for namable NPCs
        public override string customName
        {
            get { return "Test NPC"; }
        }

        public override int customAI
        {
            get
            {
                return 14;
            }
        }

        public override bool noGravity
        {
            get
            {
                return true;
            }
        }

        public override int customHealth
        {
            get
            {
                return 1000;
            }
        }

        //On death 35% chance of multiplying into 3
        public override void OnDeath(CustomNPCVars vars)
        {
            if (NPCManager.Chance(35))
            {
                NPCManager.SendPrivateMessageNearbyPlayers("TestNpc: I live three folds more! Muwhahahah!", Color.Black, vars.mainNPC.whoAmI, 50);
                vars.Multiply(vars, 3);
            }
            else
            {
                NPCManager.SendPrivateMessageNearbyPlayers("TestNpc: Nooo! I failed to multiply!", Color.Black, vars.mainNPC.whoAmI, 50);
            }
        }
        //remove default loot
        public override bool overrideBaseNPCLoot
        {
            get { return true; }
        }
    }
}
