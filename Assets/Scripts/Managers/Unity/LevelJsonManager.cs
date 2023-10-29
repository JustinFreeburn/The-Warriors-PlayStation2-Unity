using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TheWarriors
{
    public static class LevelJsonManager
    {
        [System.Serializable]
        public class Level
        {
            public string name;
            
            public string title;

            public int sectorCount;

            public string levelHash;
            
            public string sector1Hash;
            
            public string sector2Hash;

            public List<string> sector1Hashes;
            
            public List<string> sector2Hashes;
        }

        [System.Serializable]
        public class Levels
        {
            public Level[] levels;
        }

        public static Levels levelsInJson;

        public static bool ReadLevelJson()
        {
            string path = Path.Combine(Application.streamingAssetsPath, "Levels.json");

            if (File.Exists(path))
            {
                levelsInJson = JsonUtility.FromJson<Levels>(File.ReadAllText(path));

                return true;
            }

            return false;
        }

        public static Level FindLevelByName(string levelName)
        {
            if (levelsInJson != null && levelsInJson.levels != null)
            {
                foreach (Level level in levelsInJson.levels)
                {
                    if (level.name == levelName)
                    {
                        return level;
                    }
                }
            }

            return null;
        }
    }
}
