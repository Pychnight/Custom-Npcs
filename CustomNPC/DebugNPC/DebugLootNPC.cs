using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomNPC;

namespace DebugNPC
{
	public sealed class DebugLootNPCDefinition : CustomNPCDefinition
	{
		public DebugLootNPCDefinition ()
            //define base id type for Custom NPC
            : base (3)
		{
			customNPCLoots.Add (new CustomNPCLoot (808, new List<int> { 0 }, 1, 100));
			customNPCLoots.Add (new CustomNPCLoot (806, new List<int> { 83 }, 1, 100));
		}

		public override string customID {
			get { return "debugloot"; }
		}

		//Be sure to disable onlootdrop in the killed args area to test the itemdrop hook functionality!
		public override string customSpawnMessage {
			get {
				return "Loot Should spawn on death";
			}
		}


		public override string customName {
			get { return "Custom Loot Debug NPC"; }
		}

		public override int customHealth {
			get {
				return 120;
			}
		}

		public override void OnDeath (CustomNPCVars vars)
		{
			base.OnDeath (vars);
		}

		//remove default loot
		public override bool overrideBaseNPCLoot {
			get { return true; }
		}
	}
}
