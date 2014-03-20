using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomNPC
{
    internal class CustomNPCProjectiles
    {
        internal int projectileID { get; set; }
        internal List<ShotTile> projectileShotTiles { get; set; }
        internal int projectileDamage { get; set; }
        internal int projectileFireRate { get; set; }
        internal int projectileFireChance { get; set; }
        internal bool projectileCheckCollision { get; set; }
        internal float projectileAIParams1 { get; set; }
        internal float projectileAIParams2 { get; set; }

        internal CustomNPCProjectiles(int projectileid, List<ShotTile> projectileshottiles, int projectiledamage, int projectilefirerate, int projectilefirechance = 100, bool projectilecheckcollision = true, float projectileaiparams1 = 0f, float projectileaiparams2 = 0f)
        {
            projectileID = projectileid;
            projectileShotTiles = projectileshottiles;
            projectileDamage = projectiledamage;
            projectileFireRate = projectilefirerate;
            projectileFireChance = projectilefirechance;
            projectileCheckCollision = projectilecheckcollision;
            projectileAIParams1 = projectileaiparams1;
            projectileAIParams2 = projectileaiparams2;
        }
    }
}
