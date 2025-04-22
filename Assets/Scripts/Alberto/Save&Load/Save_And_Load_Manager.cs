using UnityEngine;
using System.IO;

namespace SaveLoad_Manager
{
    public static class Save_And_Load_Manager
    {
        private static readonly string saveFolder = Application.dataPath + "/Saves/";

        public static void Init()
        {
            if (!Directory.Exists(saveFolder))
            {
                Directory.CreateDirectory(saveFolder);
            }
        }

        public static void Save(string saveString, string fileName)
        {
            File.WriteAllText(saveFolder + "/" + fileName + ".txt", saveString);
        }

        public static string Load(string fileName)
        {
            if(File.Exists(saveFolder + "/" + fileName + ".txt")) 
            {
                string savedString = File.ReadAllText(saveFolder + "/" + fileName + ".txt");
                return savedString;
            }

            return null;
        }
    }
}
