using System.Collections.Generic;

namespace CustomNPC.Invasions
{
    public class WaveSet
    {
        public string WaveSetName { get; set; }
        public List<Waves> Waves { get; set; }
        
        public WaveSet(string wavesetname, List<Waves> waves)
        {
            WaveSetName = wavesetname;
            Waves = waves;

        }
    }
}
