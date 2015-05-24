using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomNPC
{
    class TaeirUtil
    {
        public static string Ignore = "";

        public static string GetNPCName(int type)
        {
            if (type < 0)
            {
                switch (type)
                {
                    case -1: return "Slimeling";
                    case -2: return "Slimer2";
                    case -3: return "Green Slime";
                    case -4: return "Pinky";
                    case -5: return "Baby Slime";
                    case -6: return "Black Slime";
                    case -7: return "Purple Slime";
                    case -8: return "Red Slime";
                    case -9: return "Yellow Slime";
                    case -10: return "Jungle Slime";
                    case -11: return "Little Eater";
                    case -12: return "Big Eater";
                    case -13: return "Short Bones";
                    case -14: return "Big Boned";
                    case -15: return "Heavy Skeleton";
                    case -16: return "Little Stinger";
                    case -17: return "Big Stinger";
                    case -18: return "Tiny Moss Hornet";
                    case -19: return "Little Moss Hornet";
                    case -20: return "Big Moss Hornet";
                    case -21: return "Giant Moss Hornet";
                    case -22: return "Little Crimera";
                    case -23: return "Big Crimera";
                    case -24: return "Little Crimslime";
                    case -25: return "Big Crimslime";
                    case -26: return "Small Zombie";
                    case -27: return "Big Zombie";
                    case -28: return "Small Bald Zombie";
                    case -29: return "Big Bald Zombie";
                    case -30: return "Small Pincushion Zombie";
                    case -31: return "Big Pincushion Zombie";
                    case -32: return "Small Slimed Zombie";
                    case -33: return "Big Slimed Zombie";
                    case -34: return "Small Swamp Zombie";
                    case -35: return "Big Swamp Zombie";
                    case -36: return "Small Twiggy Zombie";
                    case -37: return "Big Twiggy Zombie";
                    case -38: return "Cataract Eye 2";
                    case -39: return "Sleepy Eye 2";
                    case -40: return "Dialated Eye 2";
                    case -41: return "Green Eye 2";
                    case -42: return "Purple Eye 2";
                    case -43: return "Demon Eye 2";
                    case -44: return "Small Female Zombie";
                    case -45: return "Big Female Zombie";
                    case -46: return "Small Skeleton";
                    case -47: return "Big Skeleton";
                    case -48: return "Small Headache Skeleton";
                    case -49: return "Big Headache Skeleton";
                    case -50: return "Small Misassembled Skeleton";
                    case -51: return "Big Misassembled Skeleton";
                    case -52: return "Small Pantless Skeleton";
                    case -53: return "Big Pantless Skeleton";
                    case -54: return "Small Rain Zombie";
                    case -55: return "Big Rain Zombie";
                    case -56: return "Little Hornet Fatty";
                    case -57: return "Big Hornet Fatty";
                    case -58: return "Little Hornet Honey";
                    case -59: return "Big Hornet Honey";
                    case -60: return "Little Hornet Leafy";
                    case -61: return "Big Hornet Leafy";
                    case -62: return "Little Hornet Spikey";
                    case -63: return "Big Hornet Spikey";
                    case -64: return "Little Hornet Stingy";
                    case -65: return "Big Hornet Stingy";

                    default: return "Unknown";
                }
            }
            else
            {
                switch (type)
                {
                    case 1: return "Blue Slime";
                    case 2: return "Demon Eye";
                    case 3: return "Zombie";
                    case 4: return "Eye of Cthulhu";
                    case 5: return "Servant of Cthulhu";
                    case 6: return "Eater of Souls";
                    case 7: return "Devourer Head";
                    case 8: return "Devourer Body";
                    case 9: return "Devourer Tail";
                    case 10: return "Giant Worm Head";
                    case 11: return "Giant Worm Body";
                    case 12: return "Giant Worm Tail";
                    case 13: return "Eater of Worlds Head";
                    case 14: return "Eater of Worlds Body";
                    case 15: return "Eater of Worlds Tail";
                    case 16: return "Mother Slime";
                    case 17: return "Merchant";
                    case 18: return "Nurse";
                    case 19: return "Arms Dealer";
                    case 20: return "Dryad";
                    case 21: return "Skeleton";
                    case 22: return "Guide";
                    case 23: return "Meteor Head";
                    case 24: return "Fire Imp";
                    case 25: return "Burning Sphere";
                    case 26: return "Goblin Peon";
                    case 27: return "Goblin Thief";
                    case 28: return "Goblin Warrior";
                    case 29: return "Goblin Sorcerer";
                    case 30: return "Chaos Ball";
                    case 31: return "Angry Bones";
                    case 32: return "Dark Caster";
                    case 33: return "Water Sphere";
                    case 34: return "Cursed Skull";
                    case 35: return "Skeletron Head";
                    case 36: return "Skeletron Hand";
                    case 37: return "Old Man";
                    case 38: return "Demolitionist";
                    case 39: return "Bone Serpent Head";
                    case 40: return "Bone Serpent Body";
                    case 41: return "Bone Serpent Tail";
                    case 42: return "Hornet";
                    case 43: return "Man Eater";
                    case 44: return "Undead Miner";
                    case 45: return "Tim";
                    case 46: case 303: case 337: return "Bunny";
                    case 47: return "Corrupt Bunny";
                    case 48: return "Harpy";
                    case 49: return "Cave Bat";
                    case 50: return "King Slime";
                    case 51: return "Jungle Bat";
                    case 52: return "Doctor Bones";
                    case 53: return "The Groom";
                    case 54: return "Clothier";
                    case 55: return "Goldfish";
                    case 56: return "Snatcher";
                    case 57: return "Corrupt Goldfish";
                    case 58: return "Piranha";
                    case 59: return "Lava Slime";
                    case 60: return "Hellbat";
                    case 61: return "Vulture";
                    case 62: return "Demon";
                    case 63: return "Blue Jellyfish";
                    case 64: return "Pink Jellyfish";
                    case 65: return "Shark";
                    case 66: return "Voodoo Demon";
                    case 67: return "Crab";
                    case 68: return "Dungeon Guardian";
                    case 69: return "Antlion";
                    case 70: return "Spike Ball";
                    case 71: return "Dungeon Slime";
                    case 72: return "Blazing Wheel";
                    case 73: return "Goblin Scout";
                    case 74: case 297: case 298: return "Bird";
                    case 75: return "Pixie";
                    case 77: return "Armored Skeleton";
                    case 78: return "Mummy";
                    case 79: return "Dark Mummy";
                    case 80: return "Light Mummy";
                    case 81: return "Corrupt Slime";
                    case 82: return "Wraith";
                    case 83: return "Cursed Hammer";
                    case 84: return "Enchanted Sword";
                    case 85: return "Mimic";
                    case 86: return "Unicorn";
                    case 87: return "Wyvern Head";
                    case 88: return "Wyvern Legs";
                    case 89: return "Wyvern Body";
                    case 90: return "Wyvern Body 2";
                    case 91: return "Wyvern Body 3";
                    case 92: return "Wyvern Tail";
                    case 93: return "Giant Bat";
                    case 94: return "Corruptor";
                    case 95: return "Digger Head";
                    case 96: return "Digger Body";
                    case 97: return "Digger Tail";
                    case 98: return "Seeker Head";
                    case 99: return "Seeker Body";
                    case 100: return "Seeker Tail";
                    case 101: return "Clinger";
                    case 102: return "Angler Fish";
                    case 103: return "Green Jellyfish";
                    case 104: return "Werewolf";
                    case 105: return "Bound Goblin";
                    case 106: return "Bound Wizard";
                    case 107: return "Goblin Tinkerer";
                    case 108: return "Wizard";
                    case 109: return "Clown";
                    case 110: return "Skeleton Archer";
                    case 111: return "Goblin Archer";
                    case 112: return "Vile Spit";
                    case 113: return "Wall of Flesh";
                    case 114: return "Wall of Flesh Eye";
                    case 115: return "The Hungry";
                    case 116: return "The Hungry II";
                    case 117: return "Leech Head";
                    case 118: return "Leech Body";
                    case 119: return "Leech Tail";
                    case 120: return "Chaos Elemental";
                    case 121: return "Slimer";
                    case 122: return "Gastropod";
                    case 123: return "Bound Mechanic";
                    case 124: return "Mechanic";
                    case 125: return "Retinazer";
                    case 126: return "Spazmatism";
                    case 127: return "Skeletron Prime";
                    case 128: return "Prime Cannon";
                    case 129: return "Prime Saw";
                    case 130: return "Prime Vice";
                    case 131: return "Prime Laser";
                    case 132: return "Bald Zombie";
                    case 133: return "Wandering Eye";
                    case 134: return "The Destroyer";
                    case 135: return "The Destroyer Body";
                    case 136: return "The Destroyer Tail";
                    case 139: return "Probe";
                    case 137: return "Illuminant Bat";
                    case 138: return "Illuminant Slime";
                    case 140: return "Possessed Armor";
                    case 141: return "Toxic Sludge";
                    case 142: return "Santa Claus";
                    case 143: return "Snowman Gangsta";
                    case 144: return "Mister Stabby";
                    case 145: return "Snow Balla";
                    case 147: return "Ice Slime";
                    case 148: return "Penguin";
                    case 149: return "Penguin";
                    case 150: return "Ice Bat";
                    case 151: return "Lava bat";
                    case 152: return "Giant Flying Fox";
                    case 153: return "Giant Tortoise";
                    case 154: return "Ice Tortoise";
                    case 155: return "Wolf";
                    case 156: return "Red Devil";
                    case 157: return "Arapaima";
                    case 158: return "Vampire";
                    case 159: return "Vampire";
                    case 160: return "Truffle";
                    case 161: return "Zombie Eskimo";
                    case 162: return "Frankenstein";
                    case 163: return "Black Recluse";
                    case 238: return "Black Recluse";
                    case 164: return "Wall Creeper";
                    case 165: return "Wall Creeper";
                    case 166: return "Swamp Thing";
                    case 167: return "Undead Viking";
                    case 168: return "Corrupt Penguin";
                    case 169: return "Ice Elemental";
                    case 170: return "Pigron";
                    case 171: return "Pigron";
                    case 172: return "Rune Wizard";
                    case 173: return "Crimera";
                    case 174: return "Herpling";
                    case 175: return "Angry Trapper";
                    case 176: return "Moss Hornet";
                    case 177: return "Derpling";
                    case 178: return "Steampunker";
                    case 179: return "Crimson Axe";
                    case 180: return "Pigron";
                    case 181: return "Face Monster";
                    case 182: return "Floaty Gross";
                    case 183: return "Crimslime";
                    case 184: return "Spiked Ice Slime";
                    case 185: return "Snow Flinx";
                    case 186: return "Pincushion Zombie";
                    case 187: return "Slimed Zombie";
                    case 188: return "Swamp Zombie";
                    case 189: return "Twiggy Zombie";
                    case 190: return "Cataract Eye";
                    case 191: return "Sleepy Eye";
                    case 192: return "Dialated Eye";
                    case 193: return "Green Eye";
                    case 194: return "Purple Eye";
                    case 195: return "Lost Girl";
                    case 196: return "Nymph";
                    case 197: return "Armored Viking";
                    case 198: return "Lihzahrd";
                    case 199: return "Lihzahrd";
                    case 200: return "Female Zombie";
                    case 201: return "Headache Skeleton";
                    case 202: return "Misassembled Skeleton";
                    case 203: return "Pantless Skeleton";
                    case 204: return "Spiked Jungle Slime";
                    case 205: return "Moth";
                    case 206: return "Icy Merman";
                    case 207: return "Dye Trader";
                    case 208: return "Party Girl";
                    case 209: return "Cyborg";
                    case 210: return "Bee";
                    case 211: return "Bee";
                    case 212: return "Pirate Deckhand";
                    case 213: return "Pirate Corsair";
                    case 214: return "Pirate Deadeye";
                    case 215: return "Pirate Crossbower";
                    case 216: return "Pirate Captain";
                    case 217: return "Cochineal Beetle";
                    case 218: return "Cyan Beetle";
                    case 219: return "Lac Beetle";
                    case 220: return "Sea Snail";
                    case 221: return "Squid";
                    case 222: return "Queen Bee";
                    case 223: return "Zombie";
                    case 224: return "Flying Fish";
                    case 225: return "Umbrella Slime";
                    case 226: return "Flying Snake";
                    case 227: return "Painter";
                    case 228: return "Witch Doctor";
                    case 229: return "Pirate";
                    case 230: return "Goldfish";
                    case 231: return "Hornet Fatty";
                    case 232: return "Hornet Honey";
                    case 233: return "Hornet Leafy";
                    case 234: return "Hornet Spikey";
                    case 235: return "Hornet Stingy";
                    case 236: return "Jungle Creeper";
                    case 237: return "Jungle Creeper";
                    case 239: return "Blood Crawler";
                    case 240: return "Blood Crawler";
                    case 241: return "Blood Feeder";
                    case 242: return "Blood Jelly";
                    case 243: return "Ice Golem";
                    case 244: return "Rainbow Slime";
                    case 245: return "Golem";
                    case 246: return "Golem Head";
                    case 247:
                    case 248: return "Golem Fist";
                    case 249: return "Golem Head";
                    case 250: return "Angry Nimbus";
                    case 251: return "Eyezor";
                    case 252: return "Parrot";
                    case 253: return "Reaper";
                    case 254: return "Zombie";
                    case 255: return "Zombie";
                    case 256: return "Fungo Fish";
                    case 257: return "Anomura Fungus";
                    case 258: return "Mushi Ladybug";
                    case 259: return "Fungi Bulb";
                    case 260: return "Giant Fungi Bulb";
                    case 261: return "Fungi Spore";
                    case 262: return "Plantera";
                    case 263: return "Plantera's Hook";
                    case 264: return "Plantera's Tentacle";
                    case 265: return "Spore";
                    case 266: return "Brain of Cthulhu";
                    case 267: return "Creeper";
                    case 268: return "Ichor Sticker";
                    case 269: return "Rusty Armored Bones";
                    case 270: return "Rusty Armored Bones";
                    case 271: return "Rusty Armored Bones";
                    case 272: return "Rusty Armored Bones";
                    case 273: return "Blue Armored Bones";
                    case 274: return "Blue Armored Bones";
                    case 275: return "Blue Armored Bones";
                    case 276: return "Blue Armored Bones";
                    case 277: return "Hell Armored Bones";
                    case 278: return "Hell Armored Bones";
                    case 279: return "Hell Armored Bones";
                    case 280: return "Hell Armored Bones";
                    case 281: return "Ragged Caster";
                    case 282: return "Ragged Caster";
                    case 283: return "Necromancer";
                    case 284: return "Necromancer";
                    case 285: return "Diabolist";
                    case 286: return "Diabolist";
                    case 287: return "Bone Lee";
                    case 288: return "Dungeon Spirit";
                    case 289: return "Giant Cursed Skull";
                    case 290: return "Paladin";
                    case 291: return "Skeleton Sniper";
                    case 292: return "Tactical Skeleton";
                    case 293: return "Skeleton Commando";
                    case 294: return "Angry Bones";
                    case 295: return "Angry Bones";
                    case 296: return "Angry Bones";
                    case 299: return "Squirrel";
                    case 300: return "Mouse";
                    case 301: return "Raven";
                    case 302: return "Slime";
                    case 304: return "Hoppin' Jack";
                    case 305:
                    case 306:
                    case 307:
                    case 308:
                    case 309:
                    case 310:
                    case 311:
                    case 312:
                    case 313:
                    case 314: return "Scarecrow";
                    case 315: return "Headless Horseman";
                    case 316: return "Ghost";
                    case 317: return "Demon Eye";
                    case 318: return "Demon Eye";
                    case 319: return "Zombie";
                    case 320: return "Zombie";
                    case 321: return "Zombie";
                    case 322: return "Skeleton";
                    case 323: return "Skeleton";
                    case 324: return "Skeleton";
                    case 325: return "Mourning Wood";
                    case 326: return "Splinterling";
                    case 327: return "Pumpking";
                    case 328: return "Pumpking";
                    case 329: return "Hellhound";
                    case 330: return "Poltergeist";
                    case 331: return "Zombie";
                    case 332: return "Zombie";
                    case 333: return "Slime";
                    case 334: return "Slime";
                    case 335: return "Slime";
                    case 336: return "Slime";
                    case 338:
                    case 339:
                    case 340: return "Zombie Elf";
                    case 341: return "Present Mimic";
                    case 342: return "Gingerbread Man";
                    case 343: return "Yeti";
                    case 344: return "Everscream";
                    case 345: return "Ice Queen";
                    case 346: return "Santa-NK1";
                    case 347: return "Elf Copter";
                    case 348: return "Nutcracker";
                    case 349: return "Nutcracker";
                    case 350: return "Elf Archer";
                    case 351: return "Krampus";
                    case 352: return "Flocko";
                    case 353: return "Stylist";
                    case 354: return "Webbed Stylist";
                    case 355: return "Firefly";
                    case 356: return "Butterfly";
                    case 357: return "Worm";
                    case 358: return "Lightning Bug";
                    case 359: return "Snail";
                    case 360: return "Glowing Snail";
                    case 361: return "Frog";
                    case 362: return "Duck";
                    case 363: return "Duck";
                    case 364: return "Duck";
                    case 365: return "Duck";
                    case 366:
                    case 367: return "Scorpion";
                    case 368: return "Travelling Merchant";
                    case 369: return "Angler";
                    case 370: return "Duke Fishron";
                    case 371: return "Detonating Bubble";
                    case 372: return "Sharkron";
                    case 373: return "Sharkron2";
                    case 374: return "Truffle Worm";
                    case 375: return "Truffle Worm Digger";
                    case 376: return "Sleeping Angler";
                    case 377: return "Grasshopper";
                    default: return "Unknown";
                }
            }
        }

        public static string YesNo(bool value)
        {
            return value ? "Yes" : "No";
        }

        public static object ValOrNo(object value)
        {
            return ValOrNo(value, null, "No", false);
        }

        public static object ValOrNo(object value, object illegal)
        {
            return ValOrNo(value, illegal, "No", false);
        }

        /// <summary>
        /// If value == illegal, then return novalue, otherwise return value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="illegal"></param>
        /// <param name="novalue"></param>
        /// <returns></returns>
        public static object ValOrNo(object value, object illegal, object novalue, bool invert = false)
        {
            if (illegal != null && illegal is string)
            {
                return ((string) illegal).Equals(value) ? (invert ? value : novalue) : (invert ? novalue : value);
            }
            return value == illegal ? (invert ? value : novalue) : (invert ? novalue : value);
        }

        /// <summary>
        /// If value == illegal, then novalue is used in the format string. Otherwise value is used.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="value"></param>
        /// <param name="illegal"></param>
        /// <param name="novalue"></param>
        /// <returns></returns>
        public static string FormatValue(string format, object value, object illegal, object novalue)
        {
            return string.Format(format, value == illegal ? novalue : value);
        }

        /// <summary>
        /// If value is in illegals, then novalue is used in the format string. Otherwise value is used.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="value"></param>
        /// <param name="illegals"></param>
        /// <param name="novalue"></param>
        /// <returns></returns>
        public static string FormatValue(string format, object value, object[] illegals, object novalue)
        {
            return string.Format(format, illegals.Contains(value) ? novalue : value);
        }

        /// <summary>
        /// If value[x] == illegal[x], then novalue is used in the format string. Otherwise value[x] is used.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="value"></param>
        /// <param name="illegal"></param>
        /// <param name="novalue"></param>
        /// <returns></returns>
        public static string FormatValue(string format, object[] value, object[] illegal, object novalue)
        {
            string[] args = new string[value.Length];

            for (int i = 0; i < value.Length; i++)
            {
                if (illegal[i] == Ignore)
                    args[i] = value[i].ToString();
                else if (value[i] == illegal[i])
                    args[i] = novalue.ToString();
                else
                    args[i] = value[i].ToString();
            }

            return string.Format(format, args);
        }

        /// <summary>
        /// If value[x] == illegal[x], then novalue[x] is used in the format string. Otherwise value[x] is used.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="value"></param>
        /// <param name="illegal"></param>
        /// <param name="novalue"></param>
        /// <returns></returns>
        public static string FormatValue(string format, object[] value, object[] illegal, object[] novalue)
        {
            string[] args = new string[value.Length];

            for (int i = 0; i < value.Length; i++)
            {
                if (illegal[i] == Ignore)
                    args[i] = value[i].ToString();
                else if (value[i] == illegal[i])
                    args[i] = novalue[i].ToString();
                else
                    args[i] = value[i].ToString();
            }

            return string.Format(format, args);
        }

        /// <summary>
        /// If value[x] is in illegals[x], then novalue[x] is used in the format string. Otherwise value[x] is used.
        /// </summary>
        /// <param name="format"></param>
        /// <param name="value"></param>
        /// <param name="illegals"></param>
        /// <param name="novalue"></param>
        /// <returns></returns>
        public static string FormatValue(string format, object[] value, object[][] illegals, object[] novalue)
        {
            string[] args = new string[value.Length];

            for (int i = 0; i < value.Length; i++)
            {
                if (illegals[i] == null)
                    args[i] = value[i].ToString();
                else if (illegals[i].Contains(value[i]))
                    args[i] = novalue[i].ToString();
                else
                    args[i] = value[i].ToString();
            }

            return string.Format(format, args);
        }
    }
}
