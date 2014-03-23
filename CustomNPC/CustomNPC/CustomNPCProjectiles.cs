using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomNPC
{
    public class CustomNPCProjectiles
    {
        public int projectileID { get; set; }
        public List<ShotTile> projectileShotTiles { get; set; }
        public int projectileDamage { get; set; }
        public int projectileFireRate { get; set; }
        public int projectileFireChance { get; set; }
        public bool projectileCheckCollision { get; set; }
        public float projectileAIParams1 { get; set; }
        public float projectileAIParams2 { get; set; }
        public bool projectileLookForTarget { get; set; }

        public CustomNPCProjectiles(int projectileid, List<ShotTile> projectileshottiles, int projectiledamage, int projectilefirerate, bool projectilelookfortarget = false, int projectilefirechance = 100, bool projectilecheckcollision = true, float projectileaiparams1 = 0f, float projectileaiparams2 = 0f)
        {
            projectileID = projectileid;
            projectileShotTiles = projectileshottiles;
            projectileDamage = projectiledamage;
            projectileFireRate = projectilefirerate;
            projectileFireChance = projectilefirechance;
            projectileCheckCollision = projectilecheckcollision;
            projectileAIParams1 = projectileaiparams1;
            projectileAIParams2 = projectileaiparams2;
            projectileLookForTarget = projectilelookfortarget;
        }
    }
}
