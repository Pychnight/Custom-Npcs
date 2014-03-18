using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using TShockAPI;

namespace CustomNPC
{
    internal class CustomNPC
    {
        /// <summary>
        /// NPC Variable Customizations
        /// </summary>
        internal string customName { get; set; }
        internal int customHealth { get; set; }
        internal string customID { get; set; }
        internal int customDefense { get; set; }
        internal int customSpeed { get; set; }
        internal int customAI { get; set; }
        internal bool isBoss { get; set; }
        internal bool noGravity { get; set; }
        internal bool noTileCollide { get; set; }
        internal bool lavaImmune { get; set; }
        internal int customBaseID { get; set; }

        /// <summary>
        /// NPC Restrictions/Conditions
        /// </summary>
        internal List<string> customBiomeSpawn { get; set; }
        internal List<string> customRegionSpawn { get; set; }

        /// <summary>
        /// NPC MISC
        /// </summary>
        internal List<int> customAreaDebuff { get; set; }
        //internal CustomParticle customParticle { get; set; }; 
        internal int customSpawnTimer { get; set; }
        internal List<CustomNPCProjectiles> customProjectiles { get; set; }
        internal int customSpawnChance { get; set; }
        internal NPC mainNPC { get; set; }
        
        /// <summary>
        /// Transforms a NPC to another Custom NPC
        /// </summary>
        /// <param name="id">ID of NPC - Can be Custom</param>
        /// <param name="addhealth">Increase monsters Health</param>
        /// <param name="additionalhealth">Amount to Increase by, if 0 - get new monsters health and add that to NPC</param>
        internal void Transform(string id, bool addhealth = false, int additionalhealth = 0)
        {
            //CustomNPC obj = .GetNPCbyID(id);
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
        internal void Transform(int id, bool addhealth = false, int additionalhealth = 0)
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
        internal void SelfHealing(int amount)
        {
            if (mainNPC.life + amount < customHealth)
            {
                mainNPC.life += amount;
            }
            else
            {
                mainNPC.life = customHealth;
            }
        }

        /// <summary>
        /// Spawns monsters randomly around the current x y position.
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="sethealth"></param>
        /// <param name="health"></param>
        internal void Multiply(int amount, bool sethealth = false, int health = 0)
        {
            
        }
        
        internal bool HealthAbove(int Health)
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

        internal bool HealthBelow(int Health)
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
        
    }
}
