using System.Collections.Generic;

namespace CustomNPC.Invasions
{
    public class WaveSet
    {
        public string WaveName { get; set; }

        public List<Waves> Waves { get; set; }

        public WaveSet(string wavename, List<Waves> waves)
        {
            WaveName = wavename;
            Waves = waves;
        }
    }
}
