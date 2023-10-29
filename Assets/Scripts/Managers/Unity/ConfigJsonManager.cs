using System.IO;
using UnityEngine;

namespace TheWarriors
{
    public static class ConfigJsonManager
    {
        [System.Serializable]
        public class Config
        {
            public string wadFileName;

            public string dirFileName;

            public string musicFileName;

            public string soundFileName;
        }

        public static Config configInJson;

        public static bool ReadConfigJson()
        {
            string path = Path.Combine(Application.streamingAssetsPath, "Config.json");

            if (File.Exists(path))
            {
                configInJson = JsonUtility.FromJson<Config>(File.ReadAllText(path));

                return true;
            }

            return false;
        }

        public static string GetValueFromConfig(string key)
        {
            if (configInJson != null)
            {
                switch (key.ToUpper())
                {
                    case "WADFILENAME":
                        return configInJson.wadFileName;
                    case "DIRFILENAME":
                        return configInJson.dirFileName;
                    case "MUSICFILENAME":
                        return configInJson.musicFileName;
                    case "SOUNDFILENAME":
                        return configInJson.soundFileName;
                    default:
                        return string.Empty;
                }
            }
            
            return string.Empty;
        }
    }
}
