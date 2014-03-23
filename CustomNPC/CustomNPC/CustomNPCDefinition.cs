using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TShockAPI;
using TShockAPI.DB;

namespace CustomNPC
{
    public abstract class CustomNPCDefinition
    {
        private static readonly Color SummonColor = new Color(175, 75, 255);

        private NPC baseNPC;

        protected CustomNPCDefinition(int id)
        {
            baseNPC = TShock.Utils.GetNPCById(id);

            // check to make sure this npc is valid
            if (baseNPC == null || baseNPC.netID == 0)
                throw new ArgumentException("Invalid BaseNPC id specified: " + id, "id");
        }

        /// <summary>
        /// NPC Variable Customizations
        /// </summary>
        public virtual NPC customBase
        {
            get { return baseNPC; }
        }
        public virtual bool isReplacement
        {
            get { return false; }
        }
        public abstract string customID { get; }
        public abstract string customName { get; }
        public virtual int customHealth
        {
            get { return customBase.lifeMax; }
        }
        public virtual int customDefense
        {
            get { return customBase.defense; }
        }
        protected virtual int customSpeed
        {
            get { throw new NotImplementedException(); }
        }
        public virtual int customAI
        {
            get { return customBase.aiStyle; }
        }
        public virtual bool isBoss
        {
            get { return customBase.boss; }
        }
        public virtual bool noGravity
        {
            get { return customBase.noGravity; }
        }
        public virtual bool noTileCollide
        {
            get { return customBase.noTileCollide; }
        }
        public virtual bool lavaImmune
        {
            get { return customBase.lavaImmune; }
        }

        /// <summary>
        /// NPC Restrictions/Conditions
        /// </summary>
        public virtual BiomeTypes customBiomeSpawn
        {
            get { return BiomeTypes.Grass; }
        }
        public virtual IList<string> customRegionSpawn
        {
            get { return null; }
        }

        // NPC Projectile Variables

        /// <summary>
        /// Gets a list of projectiles this NPC can fire.
        /// </summary>
        public virtual IList<CustomNPCProjectiles> customProjectiles
        {
            get { return null; }
        }

        // NPC Loot Variables

        /// <summary>
        /// Gets a list of items this NPC can drop.
        /// </summary>
        public virtual IList<CustomNPCLoot> customNPCLoots
        {
            get { return null; }
        }

        /// <summary>
        /// Gets a boolean that determines whether this NPC should drop its base NPC loot.
        /// </summary>
        public virtual bool overrideBaseNPCLoot
        {
            get { return false; }
        }

        // NPC spawning variables

        /// <summary>
        /// Gets the number of seconds between spawn attempts.
        /// </summary>
        public virtual int customSpawnTimer
        {
            get { return 5; }
        }

        /// <summary>
        /// Gets the chance (out of 100) to spawn this NPC.
        /// </summary>
        public virtual double customSpawnChance
        {
            get { return 100.0; }
        }

        /// <summary>
        /// Gets the message displayed when this NPC is spawned.
        /// </summary>
        public virtual string customSpawnMessage
        {
            get { return null; }
        }

        /// <summary>
        /// Gets the color of the custom message displayed when this NPC is spawned.
        /// </summary>
        public virtual Color customSpawnMessageColor
        {
            get { return SummonColor; }
        }

        /// <summary>
        /// NPC MISC
        /// </summary>
        protected abstract List<int> customAreaDebuff { get; }
        //internal CustomParticle customParticle { get; set; }; 

        /// <summary>
        /// Transforms a NPC to another Custom NPC
        /// </summary>
        /// <param name="npcdef">CustomNPC that will be replacing it</param>
        /// <param name="addhealth">Increase monsters Health</param>
        /// <param name="additionalhealth">Amount to Increase by, if 0 - get new monsters health and add that to NPC</param>
        public void Transform(CustomNPCVars npcvar, bool addhealth = false, int additionalhealth = 0)
        {
            npcvar.mainNPC.type = npcvar.customNPC.customBase.type;
            if (addhealth)
            {
                if (additionalhealth == 0)
                {
                    npcvar.mainNPC.life += npcvar.customNPC.customHealth;
                }
                else
                {
                    npcvar.mainNPC.life += additionalhealth;
                }
            }
        }

        /// <summary>
        /// Transform Overload for non-custom NPCs
        /// </summary>
        /// <param name="id">ID of NPC - Can be Custom</param>
        /// <param name="addhealth">Increase monsters Health</param>
        /// <param name="additionalhealth">Amount to Increase by, if 0 - get new monsters health and add that to NPC</param>
        public void Transform(CustomNPCVars npcvar, int id, bool addhealth = false, int additionalhealth = 0)
        {
            NPC obj = TShock.Utils.GetNPCById(id);
            npcvar.mainNPC.type = obj.netID;
            if (addhealth)
            {
                if (additionalhealth == 0)
                {
                    npcvar.mainNPC.life += obj.lifeMax;
                }
                else
                {
                    npcvar.mainNPC.life += additionalhealth;
                }
            }
        }

        /// <summary>
        /// Heals NPC by amount, cannot heal more then MaxHP set
        /// </summary>
        /// <param name="amount"></param>
        public void SelfHealing(CustomNPCVars npcvar, int amount)
        {
            if (npcvar.mainNPC.life + amount < npcvar.customNPC.customHealth)
            {
                npcvar.mainNPC.life += amount;
            }
            else
            {
                npcvar.mainNPC.life = npcvar.customNPC.customHealth;
            }
        }

        /// <summary>
        /// Spawns monsters randomly around the current x y position.
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="sethealth"></param>
        /// <param name="health"></param>
        public void Multiply(CustomNPCVars npcvar, int amount, bool sethealth = false, int health = 0)
        {

        }

        /// <summary>
        /// Checks if the NPC's current health is above the passed amount
        /// </summary>
        /// <param name="Health"></param>
        /// <returns></returns>
        public bool HealthAbove(CustomNPCVars npcvar, int Health)
        {
            if (npcvar.mainNPC.life >= Health)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the NPC's current health is below the passed amount
        /// </summary>
        /// <param name="Health"></param>
        /// <returns></returns>
        public bool HealthBelow(CustomNPCVars npcvar, int Health)
        {
            if (npcvar.mainNPC.life <= Health)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the NPC currently has a buff placed on them
        /// </summary>
        /// <param name="buffid"></param>
        /// <returns></returns>
        public bool HasBuff(CustomNPCVars npcvar, int buffid)
        {
            if (npcvar.mainNPC.buffType.Contains(buffid))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Teleports a NPC to a specific location in a region
        /// </summary>
        /// <param name="region"></param>
        /// <param name="randompos"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void TeleportNPC(CustomNPCVars npcvar, string region, bool randompos = true, int x = 0, int y = 0)
        {
            Region obj = null;
            try
            {
                obj = TShock.Regions.GetRegionByName(region);
            }
            catch
            {
                Log.ConsoleError("Error: a defined region does not exist on this map \"{0}\"", region);
                return;
            }
            if (randompos)
            {
                TShock.Utils.GetRandomClearTileWithInRange(obj.Area.Left, obj.Area.Top, obj.Area.Width, obj.Area.Height, out x, out y);
            }
            else
            {
                x += obj.Area.Left;
                y += obj.Area.Top;
            }
            Vector2 pos = new Vector2(x, y);
            npcvar.mainNPC.position = pos;
        }

        /// <summary>
        /// Statically set x y based on the world coordinates, and teleport npc there
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void TeleportNPC(CustomNPCVars npcvar, int x, int y)
        {
            Vector2 pos = new Vector2(x, y);
            npcvar.mainNPC.position = pos;
        }

        /// <summary>
        /// Returns the current position of the monster
        /// </summary>
        /// <returns></returns>
        public Vector2 ReturnPos(CustomNPCVars npcvar)
        {
            return new Vector2(npcvar.mainNPC.position.X, npcvar.mainNPC.position.Y);
        }

        /// <summary>
        /// Sends a message to all nearby players, distance define-able
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="message"></param>
        /// <param name="color"></param>
        public void MessageNearByPlayers(CustomNPCVars npcvar, int distance, string message, Color color)
        {
            foreach (TSPlayer obj in TShock.Players)
            {
                if (obj != null && obj.ConnectionAlive)
                {

                    if (Vector2.Distance(ReturnPos(npcvar), obj.LastNetPosition) <= distance)
                    {
                        obj.SendMessage(message, color);
                    }
                }
            }
        }

        public virtual void OnDeath(CustomNPCVars vars)
        {
        }
    }
}
