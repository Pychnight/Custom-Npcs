using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace CustomNPC
{
    public class CustomNPCConfig
    {
        public Dictionary<string, WaveSet> WaveSets { get; set; }
        /// <summary>
        /// Reads a configuration file from a given path
        /// </summary>
        /// <param name="path">string path</param>
        /// <returns>ConfigFile object</returns>
        public static CustomNPCConfig Read(string path)
        {
            if (!File.Exists(path))
                return new CustomNPCConfig();
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                return Read(fs);
            }
        }

        /// <summary>
        /// Reads the configuration file from a stream
        /// </summary>
        /// <param name="stream">stream</param>
        /// <returns>ConfigFile object</returns>
        public static CustomNPCConfig Read(Stream stream)
        {
            using (var sr = new StreamReader(stream))
            {
                var cf = JsonConvert.DeserializeObject<CustomNPCConfig>(sr.ReadToEnd());
                if (ConfigRead != null)
                    ConfigRead(cf);
                return cf;
            }
        }

        /// <summary>
        /// Writes the configuration to a given path
        /// </summary>
        /// <param name="path">string path - Location to put the config file</param>
        public void Write(string path)
        {
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.Write))
            {
                Write(fs);
            }
        }

        /// <summary>
        /// Writes the configuration to a stream
        /// </summary>
        /// <param name="stream">stream</param>
        public void Write(Stream stream)
        {
            var str = JsonConvert.SerializeObject(this, Formatting.Indented);
            using (var sw = new StreamWriter(stream))
            {
                sw.Write(str);
            }
        }

        /// <summary>
        /// On config read hook
        /// </summary>
        public static Action<CustomNPCConfig> ConfigRead;
    }
}
