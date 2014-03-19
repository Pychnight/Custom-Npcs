using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using TShockAPI.DB;
using TShockAPI;
using Terraria;

namespace CustomNPC
{
    public class CustomNPCVars
    {
        public CustomNPCDefinition customNPC { get; set; }
        public DateTime lastAttemptedProjectile { get; set; }
        public bool isDead { get; set; }
        public NPC mainNPC { get; set; } 

        public CustomNPCVars(CustomNPCDefinition customnpc, DateTime lastattemptedprojectile, NPC mainnpc, bool isdead = false)
        {
            lastAttemptedProjectile = lastattemptedprojectile;
            isDead = isdead;
            customNPC = customnpc;
            mainNPC = mainnpc;
        }

        /// <summary>
        /// Transforms a NPC to another Custom NPC
        /// </summary>
        /// <param name="obj">CustomNPC that will be replacing it</param>
        /// <param name="addhealth">Increase monsters Health</param>
        /// <param name="additionalhealth">Amount to Increase by, if 0 - get new monsters health and add that to NPC</param>
        public void Transform(CustomNPCDefinition obj, bool addhealth = false, int additionalhealth = 0)
        {
            mainNPC.type = obj.customBaseID;
            if (addhealth)
            {
                if (additionalhealth == 0)
                {
                    mainNPC.life += obj.customHealth;
                }
                else
                {
                    mainNPC.life += additionalhealth;
                }
            }
        }

        /// <summary>
        /// Transform Overload for non-custom NPCs
        /// </summary>
        /// <param name="id">ID of NPC - Can be Custom</param>
        /// <param name="addhealth">Increase monsters Health</param>
        /// <param name="additionalhealth">Amount to Increase by, if 0 - get new monsters health and add that to NPC</param>
        public void Transform(int id, bool addhealth = false, int additionalhealth = 0)
        {
            NPC obj = TShock.Utils.GetNPCById(id);
            mainNPC.type = obj.netID;
            if (addhealth)
            {
                if (additionalhealth == 0)
                {
                    mainNPC.life += obj.lifeMax;
                }
                else
                {
                    mainNPC.life += additionalhealth;
                }
            }
        }

        /// <summary>
        /// Heals NPC by amount, cannot heal more then MaxHP set
        /// </summary>
        /// <param name="amount"></param>
        public void SelfHealing(int amount)
        {
            if (mainNPC.life + amount < customNPC.customHealth)
            {
                mainNPC.life += amount;
            }
            else
            {
                mainNPC.life = customNPC.customHealth;
            }
        }

        /// <summary>
        /// Spawns monsters randomly around the current x y position.
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="sethealth"></param>
        /// <param name="health"></param>
        public void Multiply(int amount, bool sethealth = false, int health = 0)
        {

        }

        /// <summary>
        /// Checks if the NPC's current health is above the passed amount
        /// </summary>
        /// <param name="Health"></param>
        /// <returns></returns>
        public bool HealthAbove(int Health)
        {
            if (mainNPC.life >= Health)
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
        public bool HealthBelow(int Health)
        {
            if (mainNPC.life <= Health)
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
        public bool HasBuff(int buffid)
        {
            if (mainNPC.buffType.Contains(buffid))
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
        public void TeleportNPC(string region, bool randompos = true, int x = 0, int y = 0)
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
            mainNPC.position = pos;
        }

        /// <summary>
        /// Statically set x y based on the world coordinates, and teleport npc there
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void TeleportNPC(int x, int y)
        {
            Vector2 pos = new Vector2(x, y);
            mainNPC.position = pos;
        }

        /// <summary>
        /// Returns the current position of the monster
        /// </summary>
        /// <returns></returns>
        public Vector2 ReturnPos()
        {
            return new Vector2(mainNPC.position.X, mainNPC.position.Y);
        }

        /// <summary>
        /// Sends a message to all nearby players, distance define-able
        /// </summary>
        /// <param name="distance"></param>
        /// <param name="message"></param>
        /// <param name="color"></param>
        public void MessageNearByPlayers(int distance, string message, Color color)
        {
            foreach (TSPlayer obj in TShock.Players)
            {
                if (obj != null && obj.ConnectionAlive)
                {

                    if (Vector2.Distance(ReturnPos(), obj.LastNetPosition) <= distance)
                    {
                        obj.SendMessage(message, color);
                    }
                }
            }
        }
    }
}
