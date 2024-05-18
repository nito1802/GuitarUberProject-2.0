using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GitarUberProject
{
    public class AppOptions
    {
        [JsonIgnore]
        public static string OptionsPath { get; set; }
        [JsonIgnore]
        public static AppOptions Options { get; set; }

        public string LastUsedFile { get; set; }
        public bool ChordReadMode { get; set; }
        public bool RenderImage { get; set; }


        public static void Load()
        {
            if (!File.Exists(OptionsPath))
            {
                return;
            }

            string jsonContent = File.ReadAllText(OptionsPath);

            try
            {
                Options = JsonConvert.DeserializeObject<AppOptions>(jsonContent);
            }
            catch (Exception ex)
            {
            }

            if (Options == null) Options = new AppOptions();
        }

        public static void Save()
        {
            string serializedText = JsonConvert.SerializeObject(Options, Formatting.Indented);
            File.WriteAllText(OptionsPath, serializedText);
        }
    }
}
