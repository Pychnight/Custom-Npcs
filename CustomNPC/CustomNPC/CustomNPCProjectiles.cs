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

        internal CustomNPCProjectiles(int projectileid, List<ShotTile> projectileshottiles, int projectiledamage, int projectilefirerate, int projectilefirechance)
        {
            projectileID = projectileid;
            projectileShotTiles = projectileshottiles;
            projectileDamage = projectiledamage;
            projectileFireRate = projectilefirerate;
            projectileFireChance = projectilefirechance;
        }
    }
}
