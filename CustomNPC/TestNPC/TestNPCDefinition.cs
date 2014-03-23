using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomNPC;

namespace TestNPC
{
    public sealed class TestNPCDefinition : CustomNPCDefinition
    {
        private List<CustomNPCProjectiles> ProjectilesList = new List<CustomNPCProjectiles>();

        public TestNPCDefinition()
            : base(21)
        {
            ProjectilesList.Add(new CustomNPCProjectiles(180, new List<ShotTile>() { ShotTile.Middle }, 100, 250, 80, true));
        }

        public override string customID
        {
            get { return "testnpc"; }
        }

        public override string customName
        {
            get { return "Test NPC"; }
        }

        public override IList<CustomNPCLoot> customNPCLoots
        {
            get
            {
                return new[]
                {
                    new CustomNPCLoot(1553, new List<int> { 83 }, 1, 50), 
                };
            }
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

        public override IList<CustomNPCProjectiles> customProjectiles
        {
            get
            {
                return this.ProjectilesList;
            }
        }

        protected override List<int> customAreaDebuff
        {
            get { throw new NotImplementedException(); }
        }
    }
}
