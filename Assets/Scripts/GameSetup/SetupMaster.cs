using System.Collections.Generic;
using System.Linq;
using CatSAT;
using CCSS;
using Imaginarium.Driver;
using Imaginarium.Generator;
using Imaginarium.Ontology;
using UI;
using UnityEngine;
using Utility;
using Random = UnityEngine.Random;

namespace GameSetup
{
    public class SetupMaster : MonoBehaviour
    {
        // character generation
        public static Ontology Ontology;
        
        // player information
        public static Character PlayerCharacter;
        public static bool PlayerCharacterFamilyOne;
        
        // variables for family generation
        [SerializeField] private int minFamilySize;
        [SerializeField] private int maxFamilySize;

        [SerializeField] private float minGraphDensity;
        [SerializeField] private float maxGraphDensity;
        
        // graph generation
        private int _familyOneSize;
        private int _familyTwoSize;
        private string _familyOneSurname;
        private string _familyTwoSurname;
        private static Family _familyOne;
        private static Family _familyTwo;
        public static Family CombinedFamily;
    
        /// <summary>
        /// Initializes the families, loads all possible cards. 
        /// </summary>
        private void Awake()
        {
            DataFiles.DataHome = $"{Application.persistentDataPath}";
            Ontology = new Ontology("Characters", $"{Application.persistentDataPath}");
            
            FamilyPreprocessing();
            
            _familyOne = new Family(_familyOneSize, _familyOneSurname, minGraphDensity, maxGraphDensity, true);
            _familyTwo = new Family(_familyTwoSize, _familyTwoSurname, minGraphDensity, maxGraphDensity, false);
            CombinedFamily = new Family(_familyOneSize + _familyTwoSize, _familyOne, _familyTwo);
            
            FileCopier.CheckForCopies(); // todo: remove when game is complete // todo: Wait why did i write this

            CreateAllCharacters();
            
            CardTemplateLoader.LoadAllCardTemplates();
            
            // CardTemplateLoader.PrintAllCardTemplates();
            
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// 
        /// </summary>
        private void Start()
        {
            SetupSceneUI.Instance.SetupAllButtons(_familyOneSurname, _familyTwoSurname);
            ShowGraph(1);
        }

        /// <summary>
        /// Selects a random size and a random surname for each family.
        /// </summary>
        private void FamilyPreprocessing()
        {
            // set family sizes
            _familyOneSize = Random.Range(minFamilySize, maxFamilySize);
            _familyTwoSize = Random.Range(minFamilySize, maxFamilySize);
            
            // select family surnames
            List<string> surnames = 
                Resources.Load<TextAsset>("Imaginarium/surnames").text.Split('\n').ToList();
            int familyOneSurnameIndex = Random.Range(0, surnames.Count);
            _familyOneSurname = surnames[familyOneSurnameIndex].Replace("\r", "");
            surnames.RemoveAt(familyOneSurnameIndex);
            _familyTwoSurname = surnames[Random.Range(0, surnames.Count)].Replace("\r", "");
        }

        /// <summary>
        /// Creates the characters for the two families, calling MakeNumCharacters.
        /// </summary>
        private void CreateAllCharacters()
        {
            _familyOne.SetCharacters(MakeNumCharacters(_familyOneSize));
            _familyTwo.SetCharacters(MakeNumCharacters(_familyTwoSize));
            CombinedFamily.SetCharacters(_familyOne, _familyTwo); // todo: need to add additional edges between families
        }

        /// <summary>
        /// Makes the specified number of characters using Imaginarium.
        /// </summary>
        /// <param name="numCharacters">The number of characters to generate.</param>
        /// <returns>The invention of characters generated.</returns>
        private (Solution, List<PossibleIndividual>) MakeNumCharacters(int numCharacters)
        {
            CommonNoun character = Ontology.CommonNoun("character");
            Invention invention = character.MakeGenerator(numCharacters).Generate();
            
            return (invention.Model, invention.PossibleIndividuals);
        }

        /// <summary>
        /// Shows the graph corresponding to the selected family, prints the edges, and changes the UI appropriately.
        /// </summary>
        /// <param name="index">1 if family one, 2 if family two, 3 if combined family.</param>
        public void ShowGraph(int index)
        {
            switch (index)
            {
                case 1:
                    _familyOne.ShowGraph();
                    _familyOne.PrintEdges();
                    break;
                case 2:
                    _familyTwo.ShowGraph();
                    _familyTwo.PrintEdges();
                    break;
                case 3:
                    CombinedFamily.ShowGraph();
                    CombinedFamily.PrintEdges();
                    break;
            }
            
            SetupSceneUI.Instance.SetActiveButton(index);
        }
        
        /// <summary>
        /// Returns the family corresponding to the selected index.
        /// </summary>
        /// <param name="familyOne">True if returning family one, false if returning family one.</param>
        /// <returns>The desired Family object.</returns>
        public static Family GetFamily(bool familyOne) => familyOne ? _familyOne : _familyTwo;
    }
}