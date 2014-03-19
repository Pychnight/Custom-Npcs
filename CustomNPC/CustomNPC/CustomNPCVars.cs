using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomNPC
{
    public class CustomNPCVars
    {
        internal DateTime lastAttemptedProjectile { get; set; }
        public CustomNPCVars(DateTime lastattemptedprojectile)
        {
            lastAttemptedProjectile = lastattemptedprojectile;
        }
    }
}
