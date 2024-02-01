using UnityEngine;
using System.IO;

namespace Utility
{
    public static class FileCopier
    {
        private static readonly string PersistentDataPath = $"{Application.persistentDataPath}";
        private const string GenFileIndicator = "// gen";
        private const string CsvFileIndicator = "// csv";
        private const string InflectionsFolder = "Inflections";
        
        /// <summary>
        /// Checks for the existence of Imaginarium files in the player's local data. If they don't exist, they are
        /// copied from the Resources folder.
        /// </summary>
        public static void CheckForCopies()
        {
            var allImaginariumFiles = Resources.LoadAll<TextAsset>("Imaginarium");
            var allInflectionsFiles = Resources.LoadAll<TextAsset>("Inflections");
            
            foreach (var file in allImaginariumFiles)
            {
                var fileName = $"{file.name}{(file.text.Contains(GenFileIndicator) ? ".gen" : ".txt")}";
                Debug.Log($"checking file {fileName}");
                if (!File.Exists(Path.Combine(PersistentDataPath, fileName)))
                {
                    CopyFile(file, fileName);
                }
            }
            
            MakeInflectionDirectory();
            
            foreach (var file in allInflectionsFiles)
            {
                var fileName = $"{file.name}{(file.text.Contains(CsvFileIndicator) ? ".csv" : ".txt")}";
                Debug.Log($"checking file {fileName}");
                if (!File.Exists(Path.Combine(PersistentDataPath, InflectionsFolder, fileName)))
                {
                    CopyFile(file, Path.Combine(InflectionsFolder, fileName));
                }
            }
        }

        // todo write something that checks file dif

        private static void MakeInflectionDirectory()
        {
            if (Directory.Exists(Path.Combine(PersistentDataPath, InflectionsFolder))) return;
            Directory.CreateDirectory(Path.Combine(PersistentDataPath, InflectionsFolder));
        }
        
        /// <summary>
        /// Copies the specified file with the specified filename to the player's local data.
        /// </summary>
        /// <param name="file">The file to be copied.</param>
        /// <param name="fileName">The file's name.</param>
        private static void CopyFile(TextAsset file, string fileName)
        {
            string fileText = fileName.Contains(".csv") ? file.text.Replace(CsvFileIndicator, "") : file.text;
            File.WriteAllText(Path.Combine(PersistentDataPath, fileName), fileText);
        }
    }
}