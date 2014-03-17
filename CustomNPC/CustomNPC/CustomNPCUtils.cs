using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomNPC
{
    internal class CustomNPCUtils
    {
        internal Random rand = new Random();

        //Below should be in the NPC Class
        //internal void Transform(int id, bool addhealth = false, int additionalhealth = 0)
        //{

        //}

        //internal void SelfHealing(int amount)
        //{

        //}

        //internal void Multiply(int amount, bool sethealth = false, int health = 0)
        //{

        //}

        //internal bool HealthAbove(int Health)
        //{

        //}

        internal bool Chance(double percentage)
        {
            if (rand.Next(101) <= percentage)
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
