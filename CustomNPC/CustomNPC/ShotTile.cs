using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomNPC
{
    public class ShotTile
    {
        public float X { get; set; }
        public float Y { get; set; }

        public ShotTile(float x, float y)
        {
            X = x;
            Y = y;
        }

        public ShotTile CustomShotTile(float x, float y)
        {
            return new ShotTile(x, y);
        }

        public static ShotTile TopLeft
        {
            get { return new ShotTile(-20, -20); }
        }

        public static ShotTile Top
        {
            get { return new ShotTile(0, -20); }
        }

        public static ShotTile TopRight
        {
            get { return new ShotTile(20, -20); }
        }

        public static ShotTile MiddleLeft
        {
            get { return new ShotTile(-20, 0); }
        }

        public static ShotTile Middle
        {
            get { return new ShotTile(0, 0); }
        }

        public static ShotTile MiddleRight
        {
            get { return new ShotTile(20, 0); }
        }

        public static ShotTile BottomLeft
        {
            get { return new ShotTile(-20, 20); }
        }

        public static ShotTile Bottom
        {
            get { return new ShotTile(0, 20); }
        }

        public static ShotTile BottomRight
        {
            get { return new ShotTile(20, 20); }
        }
    }
}
