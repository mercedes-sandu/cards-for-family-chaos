using UnityEngine;
using System.IO;

namespace Utility
{
    public static class FileCopier
    {
        private static readonly string PersistentDataPath = $"{Application.persistentDataPath}";
        private const string GenFileIndicator = "// gen";
        private const string CsvFileIndicator = "// csv";
        private const string ImaginariumFolder = "Imaginarium";
        private const string InflectionsFolder = "Inflections";
        private const string ResourcesCardsJsonPath = "cards";
        private const string CardsJson = "cards.json";
        private const string GenExtension = ".gen";
        private const string CsvExtension = ".csv";
        private const string TxtExtension = ".txt";

        /// <summary>
        /// Checks for the existence of Imaginarium files in the player's local data. If they don't exist, they are
        /// copied from the Resources folder.
        /// </summary>
        public static void CheckForCopies()
        {
            TextAsset[] allImaginariumFiles = Resources.LoadAll<TextAsset>(ImaginariumFolder);
            TextAsset[] allInflectionsFiles = Resources.LoadAll<TextAsset>(InflectionsFolder);
            TextAsset cardsJson = Resources.Load<TextAsset>(ResourcesCardsJsonPath);

            foreach (var file in allImaginariumFiles)
            {
                string fileName = $"{file.name}{(file.text.Contains(GenFileIndicator) ? GenExtension : TxtExtension)}";
                string txtFileName = $"{file.name}{TxtExtension}";
                Debug.Log($"Checking for copy of {fileName}.");
                string filePath = Path.Combine(PersistentDataPath, fileName);
                string txtFilePath = Path.Combine(PersistentDataPath, txtFileName);
                if (!File.Exists(filePath) || !File.Exists(txtFilePath))
                {
                    CopyFile(file, fileName);
                }
                else
                {
                    CheckFileDiff(file, File.ReadAllText(txtFilePath), fileName);
                }
            }

            MakeInflectionDirectory();

            foreach (var file in allInflectionsFiles)
            {
                string fileName = $"{file.name}{(file.text.Contains(CsvFileIndicator) ? CsvExtension : TxtExtension)}";
                string txtFileName = $"{file.name}{TxtExtension}";
                Debug.Log($"Checking for copy of {fileName}.");
                string filePath = Path.Combine(PersistentDataPath, InflectionsFolder, fileName);
                string txtFilePath = Path.Combine(PersistentDataPath, InflectionsFolder, txtFileName);
                if (!File.Exists(filePath) || !File.Exists(txtFilePath))
                {
                    CopyFile(file, Path.Combine(InflectionsFolder, fileName));
                }
                else
                {
                    CheckFileDiff(file, File.ReadAllText(txtFilePath),
                        Path.Combine(InflectionsFolder, fileName));
                }
            }

            MakeJson(cardsJson, CardsJson);
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
        /// Checks if the specified Json file exists in the player's local data. If it doesn't, it is copied from the
        /// Resources folder. If it does, it is checked for differences from the version in the Resources folder.
        /// </summary>
        /// <param name="resourcesJson">The text in the Json from the Resources folder.</param>
        /// <param name="fileName">The name of the Json file.</param>
        private static void MakeJson(TextAsset resourcesJson, string fileName)
        {
            string filePath = Path.Combine(PersistentDataPath, fileName);
            if (!File.Exists(filePath))
            {
                CopyFile(resourcesJson, fileName);
            }
            else
            {
                CheckFileDiff(resourcesJson, File.ReadAllText(filePath), fileName);
            }
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
        /// Copies the specified file with the specified filename to the player's local data. If the file is not a
        /// standard txt type, then its corresponding txt file is also copied.
        /// </summary>
        /// <param name="file">The file to be copied.</param>
        /// <param name="fileName">The file's name.</param>
        private static void CopyFile(TextAsset file, string fileName)
        {
            string fileText = fileName.Contains(CsvExtension)
                ? file.text.Replace(CsvFileIndicator, "")
                : file.text;
            File.WriteAllText(Path.Combine(PersistentDataPath, fileName), fileText);
            Debug.Log($"Copied {fileName} to {PersistentDataPath}.");

            if (fileName.Contains(TxtExtension)) return;
            string txtFileName = fileName.Replace(GenExtension, TxtExtension).Replace(CsvExtension, TxtExtension);
            File.WriteAllText(Path.Combine(PersistentDataPath, txtFileName), file.text);
            Debug.Log($"Copied {txtFileName} to {PersistentDataPath}.");
        }
    }
}