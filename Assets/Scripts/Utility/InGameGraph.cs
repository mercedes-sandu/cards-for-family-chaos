using System;
using System.Collections.Generic;
using System.Linq;
using CatSAT.SAT;
using CCSS;
using GameSetup;
using UnityEngine;
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

        private Card _currentCard;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="family"></param>
        public InGameGraph(Family family)
        {
            EdgesAndInformation = new Dictionary<(Character, Character), EdgeInformation>();
            AllCharacters = new HashSet<Character>();

            foreach (EdgeProposition edge in family.EdgesInSolution.Values)
            {
                Character characterOne = family.Characters[edge.SourceVertex];
                Character characterTwo = family.Characters[edge.DestinationVertex];
                // todo: relationship type?
                AffinityPair affinityPair = new AffinityPair((float)Math.Round(Random.Range(-1f, 1f), 2),
                    (float)Math.Round(Random.Range(-1f, 1f), 2));
                AddEdge(characterOne, characterTwo, affinityPair);
            }
            
            foreach (var character in family.Characters.Values.Where(character => character != Player.PlayerCharacter))
            {
                AllCharacters.Add(character);
            }

            GameEvent.OnCardSelected += UpdateCurrentCard;
            GameEvent.OnChoiceMade += UpdateGraph;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="characterOne"></param>
        /// <param name="characterTwo"></param>
        /// <param name="affinityPair"></param>
        private void AddEdge(Character characterOne, Character characterTwo, AffinityPair affinityPair)
        {
            bool firstOrderAdd =
                EdgesAndInformation.TryAdd((characterOne, characterTwo), new EdgeInformation(affinityPair));

            if (firstOrderAdd) return;
            EdgesAndInformation.TryAdd((characterTwo, characterOne), new EdgeInformation(affinityPair));
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
        /// <param name="card"></param>
        /// <param name="weekNumber"></param>
        private void UpdateCurrentCard(Card card, int weekNumber)
        {
            _currentCard = card;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="choice"></param>
        private void UpdateGraph(Choice choice)
        {
            foreach (EdgeModifier edgeModifier in choice.EdgeModifiers)
            {
                Character characterOne = _currentCard.GetRoleCharacter(edgeModifier.RoleOne);
                Character characterTwo = _currentCard.GetRoleCharacter(edgeModifier.RoleTwo);
                AffinityPair potentialAffinityPair = GetAffinityPair(characterOne, characterTwo);

                if (potentialAffinityPair == null)
                {
                    AddEdge(characterOne, characterTwo, new AffinityPair(edgeModifier.PositiveModifier,
                        edgeModifier.NegativeModifier));
                }
                else
                {
                    potentialAffinityPair.PositiveAffinity += Mathf.Clamp(edgeModifier.PositiveModifier, 0f, 1f);
                    potentialAffinityPair.NegativeAffinity += Mathf.Clamp(edgeModifier.NegativeModifier, 0f, 1f);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        ~InGameGraph()
        {
            GameEvent.OnCardSelected -= UpdateCurrentCard;
            GameEvent.OnChoiceMade -= UpdateGraph;
        }
    }
}