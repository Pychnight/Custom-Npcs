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
        public DateTime[] lastAttemptedProjectile { get; set; }
        public bool isDead { get; set; }
        public NPC mainNPC { get; set; }
        public bool droppedLoot { get; set; }
        public bool isClone { get; set; }
        public bool isInvasion { get; set; }
        private Random rand = new Random();
        private IDictionary<string, object> variables = new Dictionary<string, object>();

        public CustomNPCVars(CustomNPCDefinition customnpc, DateTime[] lastattemptedprojectile, NPC mainnpc, bool isclone = false)
        {
            lastAttemptedProjectile = lastattemptedprojectile;
            isDead = false;
            isClone = isclone;
            customNPC = customnpc;
            mainNPC = mainnpc;
            droppedLoot = false;
        }

        /// <summary>
        /// Updates this npc to the skin of the target NPC
        /// </summary>
        /// <param name="target">The NPC to take the skin of</param>
        /// <param name="update">If a netupdate should be enforced.</param>
        private void UpdateSkin(NPC target, bool update = true)
        {
            //Proper type
            mainNPC.type = target.type;
            mainNPC.netID = target.netID;

            //Color and size
            mainNPC.color = target.color;
            mainNPC.width = target.width;
            mainNPC.height = target.height;
            mainNPC.scale = target.scale;

            //Sounds
            mainNPC.soundHit = target.soundHit;
            mainNPC.soundKilled = target.soundKilled;
            mainNPC.soundDelay = target.soundDelay;

            //Enforce update
            if (update) mainNPC.netUpdate = true;
        }

        /// <summary>
        /// Applies a new base type. Effect wise the same as replacing this custom NPC with a new one.
        /// </summary>
        private void CustomTransform()
        {
            UpdateSkin(customNPC.customBase, false);

            mainNPC.name = customNPC.customName;
            mainNPC.displayName = customNPC.customName;
            mainNPC.lifeMax = customNPC.customHealth;
            mainNPC.aiStyle = customNPC.customAI;
            mainNPC.lavaImmune = customNPC.lavaImmune;
            mainNPC.noGravity = customNPC.noGravity;
            mainNPC.noTileCollide = customNPC.noTileCollide;

            mainNPC.damage = customNPC.customDamage;
            mainNPC.defDamage = customNPC.customDamage;

            mainNPC.defense = customNPC.customDefense;
            mainNPC.defDefense = customNPC.customDefense;

            mainNPC.netUpdate = true;
        }

        /// <summary>
        /// Transforms a NPC to another Custom NPC
        /// </summary>
        /// <param name="npcdef">CustomNPC that will be replacing it</param>
        /// <param name="addhealth">Increase monsters Health</param>
        /// <param name="additionalhealth">Amount to Increase by, if 0 - get new monsters health and add that to NPC</param>
        public void Transform(CustomNPCDefinition npcdef, bool addhealth = false, int additionalhealth = 0, bool fullTransform = true)
        {
            customNPC = npcdef;

            if (fullTransform)
            {
                CustomTransform();
            }
            else
            {
                UpdateSkin(customNPC.customBase, true);
            }

            //mainNPC.type = npcdef.customBase.type;

            if (addhealth)
            {
                if (additionalhealth == 0)
                {
                    mainNPC.life += npcdef.customHealth;
                }
                else
                {
                    mainNPC.life += additionalhealth;
                }
            }
        }

        /// <summary>
        /// Transforms a NPC to another Custom NPC
        /// </summary>
        /// <param name="id">CustomNPC that will be replacing it</param>
        /// <param name="addhealth">Increase monsters Health</param>
        /// <param name="additionalhealth">Amount to Increase by, if 0 - get new monsters health and add that to NPC</param>
        /// <param name="fullTransform">When false, only the skin of the NPC changes. When true, also applies some of the given NPC's stats</param>
        public bool Transform(string id, bool addhealth = false, int additionalhealth = 0, bool fullTransform = true)
        {
            CustomNPCDefinition search = NPCManager.Data.GetNPCbyID(id);
            //If not found, return false
            if (search == null) return false;

            Transform(search, addhealth, additionalhealth, fullTransform);
            return true;
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

            //mainNPC.type = obj.netID;

            UpdateSkin(obj, true);

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
            mainNPC.life = Math.Min(customNPC.customHealth, mainNPC.life + amount);
        }

        /// <summary>
        /// Spawns monsters randomly around the current x y position.
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="sethealth"></param>
        /// <param name="health"></param>
        public void Multiply(CustomNPCVars npcvar, int amount, bool sethealth = false, int health = 0)
        {
            //gets the npc
            if (npcvar == null)
                return;

            for (int i = 0; i < amount; i++)
            {
                if (mainNPC == null)
                    continue;

                int npc = NPCManager.SpawnNPCAtLocation((int)mainNPC.position.X + rand.Next(0, 16) - 8, (int)mainNPC.position.Y + rand.Next(0, 16) - 8, customNPC);
                if (npc == -1)
                    continue;

                var spawned = NPCManager.GetCustomNPCByIndex(npc);
                if (spawned != null)
                {
                    if (sethealth)
                    {
                        spawned.mainNPC.life = health;
                    }
                    spawned.isClone = true;
                }
            }
        }

        /// <summary>
        /// Spawns monsters randomly around the current x y position.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="amount"></param>
        /// <param name="sethealth"></param>
        /// <param name="health"></param>
        public void Multiply(string id, int amount, bool sethealth = false, int health = 0)
        {
            var def = NPCManager.Data.GetNPCbyID(id);
            if (def == null)
                return;

            if (mainNPC == null)
                return;

            for (int i = 0; i < amount; i++)
            {
                int npc = NPCManager.SpawnNPCAtLocation((int)mainNPC.position.X + rand.Next(0, 16) - 8, (int)mainNPC.position.Y + rand.Next(0, 16) - 8, def);
                if (npc == -1)
                    continue;

                var spawned = NPCManager.GetCustomNPCByIndex(npc);
                if (spawned != null)
                {
                    if (sethealth)
                    {
                        spawned.mainNPC.life = health;
                    }
                    //TADD this should be here I think
                    spawned.isClone = true;
                }
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
                TShock.Log.ConsoleError("Error: a defined region does not exist on this map \"{0}\"", region);
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
            npcvar.mainNPC.position = pos * 16;
        }

        /// <summary>
        /// Statically set x y based on the world coordinates, and teleport npc there
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void TeleportNPC(CustomNPCVars npcvar, int x, int y)
        {
            Vector2 pos = new Vector2(x, y);
            npcvar.mainNPC.position = pos * 16;
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
            if (TShock.Players.Length == 0) return;

            Vector2 temp = ReturnPos(npcvar);
            foreach (TSPlayer obj in TShock.Players)
            {
                if (obj != null && obj.ConnectionAlive)
                {
                    if (Vector2.Distance(temp, obj.LastNetPosition) <= distance)
                    {
                        obj.SendMessage(message, color);
                    }
                }
            }
        }

        public void SetVariable<T>(string name, T value)
        {
            variables[name] = value;
        }

        public T GetVariable<T>(string name, T defaultValue = default(T))
        {
            object boxed;
            if (!variables.TryGetValue(name, out boxed))
                return defaultValue;

            return (T)boxed;
        }

        public void OnDeath()
        {
            if (customNPC != null)
            {
                customNPC.OnDeath(this);
            }
        }
    }
}