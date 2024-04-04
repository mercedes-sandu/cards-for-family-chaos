using System;
using System.Collections.Generic;
using CatSAT.SAT;
using CCSS;
using GameSetup;
using Random = UnityEngine.Random;

namespace Utility
{
    public class AffinityPair
    {
        public float PositiveAffinity;
        public float NegativeAffinity;
        
        public float NetAffinity => PositiveAffinity - NegativeAffinity;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="positiveAffinity"></param>
        /// <param name="negativeAffinity"></param>
        public AffinityPair(float positiveAffinity, float negativeAffinity)
        {
            PositiveAffinity = positiveAffinity;
            NegativeAffinity = negativeAffinity;
        }
    }

    public class EdgeInformation
    {
        public AffinityPair AffinityPair;
        // todo: add relationship type? enum? string?

        public EdgeInformation(AffinityPair affinityPair)
        {
            AffinityPair = affinityPair;
        }
    }

    public class InGameGraph
    {
        public Dictionary<(Character, Character), EdgeInformation> EdgesAndInformation;
        public HashSet<Character> AllCharacters;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="family"></param>
        public InGameGraph(Family family)
        {
            EdgesAndInformation = new Dictionary<(Character, Character), EdgeInformation>();
            AllCharacters = new HashSet<Character>();

            foreach (EdgeProposition edge in family.Edges.Values)
            {
                Character characterOne = family.Characters[edge.SourceVertex];
                Character characterTwo = family.Characters[edge.DestinationVertex];
                // todo: relationship type?
                AffinityPair affinityPair = new AffinityPair((float)Math.Round(Random.Range(-1f, 1f), 2),
                    (float)Math.Round(Random.Range(-1f, 1f), 2));
                EdgesAndInformation.TryAdd((characterOne, characterTwo), new EdgeInformation(affinityPair));
            }

            foreach (Character character in family.Characters.Values)
            {
                AllCharacters.Add(character);
            }
            
            GameEvent.OnChoiceMade += UpdateGraph;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="characterOne"></param>
        /// <param name="characterTwo"></param>
        /// <returns></returns>
        public bool AreConnected(Character characterOne, Character characterTwo)
        {
            return EdgesAndInformation.ContainsKey((characterOne, characterTwo)) ||
                   EdgesAndInformation.ContainsKey((characterTwo, characterOne));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="characterOne"></param>
        /// <param name="characterTwo"></param>
        /// <returns></returns>
        public AffinityPair GetAffinityPair(Character characterOne, Character characterTwo)
        {
            if (!AreConnected(characterOne, characterTwo)) return null;

            EdgesAndInformation.TryGetValue((characterOne, characterTwo), out EdgeInformation edgeInformation);

            if (edgeInformation != null) return edgeInformation.AffinityPair;

            EdgesAndInformation.TryGetValue((characterTwo, characterOne), out edgeInformation);

            return edgeInformation?.AffinityPair;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="choice"></param>
        private void UpdateGraph(Choice choice)
        {
            // todo: fill
        }
        
        /// <summary>
        /// 
        /// </summary>
        ~InGameGraph()
        {
            GameEvent.OnChoiceMade -= UpdateGraph;
        }
    }
}