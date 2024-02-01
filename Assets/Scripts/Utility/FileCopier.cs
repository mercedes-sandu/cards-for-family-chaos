using UnityEngine;
using System.IO;

namespace Utility
{
    public static class FileCopier
    {
        private static readonly string PersistentDataPath = $"{Application.persistentDataPath}";
        private const string GenFileIndicator = "// gen";
        
        /// <summary>
        /// Checks for the existence of Imaginarium files in the player's local data. If they don't exist, they are
        /// copied from the Resources folder.
        /// </summary>
        public static void CheckForCopies()
        {
            var allFiles = Resources.LoadAll<TextAsset>("Imaginarium");
            
            foreach (var file in allFiles)
            {
                var fileName = $"{file.name}{(file.text.Contains(GenFileIndicator) ? ".gen" : ".txt")}";
                if (!File.Exists($"{PersistentDataPath}/{fileName}"))
                {
                    CopyFile(file, fileName);
                }
            }
        }

        /// <summary>
        /// Copies the specified file with the specified filename to the player's local data.
        /// </summary>
        /// <param name="file">The file to be copied.</param>
        /// <param name="fileName">The file's name.</param>
        private static void CopyFile(TextAsset file, string fileName)
        {
            File.WriteAllText($"{PersistentDataPath}/{fileName}", file.text);
        }
    }
}