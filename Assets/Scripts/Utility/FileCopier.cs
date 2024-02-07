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
            TextAsset[] allImaginariumFiles = Resources.LoadAll<TextAsset>("Imaginarium");
            TextAsset[] allInflectionsFiles = Resources.LoadAll<TextAsset>("Inflections");
            
            foreach (var file in allImaginariumFiles)
            {
                string fileName = $"{file.name}{(file.text.Contains(GenFileIndicator) ? ".gen" : ".txt")}";
                Debug.Log($"Checking for copy of {fileName}.");
                string filePath = Path.Combine(PersistentDataPath, fileName);
                if (!File.Exists(filePath))
                {
                    CopyFile(file, fileName);
                }
                else
                {
                    CheckFileDiff(file, File.ReadAllText(filePath), fileName);
                }
            }
            
            MakeInflectionDirectory();
            
            foreach (var file in allInflectionsFiles)
            {
                string fileName = $"{file.name}{(file.text.Contains(CsvFileIndicator) ? ".csv" : ".txt")}";
                Debug.Log($"Checking for copy of {fileName}.");
                string filePath = Path.Combine(PersistentDataPath, InflectionsFolder, fileName);
                if (!File.Exists(filePath))
                {
                    CopyFile(file, Path.Combine(InflectionsFolder, fileName));
                }
                else
                {
                    CheckFileDiff(file, File.ReadAllText(filePath), Path.Combine(InflectionsFolder, fileName));
                }
            }
        }

        /// <summary>
        /// Checks if the newer file is different from the older file. If it is, the newer file is copied to the
        /// player's local files, replacing the older file since it is copied under the same name.
        /// </summary>
        /// <param name="newerFile">The newer file as a TextAsset.</param>
        /// <param name="olderFileContents">The older file's string contents.</param>
        /// <param name="fileName">The file's name that is being compared.</param>
        private static void CheckFileDiff(TextAsset newerFile, string olderFileContents, string fileName)
        {
            // todo: more efficient/better way to do this than Equals()?
            if (newerFile.text.Equals(olderFileContents)) return;
            Debug.Log($"Found difference for {fileName}.");
            CopyFile(newerFile, fileName);
        }
        
        /// <summary>
        /// Creates a folder for inflections in the player's local data if it doesn't already exist.
        /// </summary>
        private static void MakeInflectionDirectory()
        {
            if (Directory.Exists(Path.Combine(PersistentDataPath, InflectionsFolder))) return;
            Debug.Log($"Creating Inflections folder.");
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
            Debug.Log($"Copied {fileName} to {PersistentDataPath}.");
        }
    }
}