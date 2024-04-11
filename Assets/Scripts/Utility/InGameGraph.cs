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
        /// Initializes the affinity pair with the given values.
        /// </summary>
        /// <param name="positiveAffinity">The positive affinity (how strongly the characters like each other on a scale
        /// of 0 to 1).</param>
        /// <param name="negativeAffinity">The negative affinity (how strongly the characters dislike each other on a
        /// scale of 0 to 1).</param>
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
        /// Initializes the in-game graph representation with the two generated families. Subscribes to game events.
        /// </summary>
        /// <param name="family">The combined family.</param>
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
        /// Adds an edge between two characters with the given affinity pair.
        /// </summary>
        /// <param name="characterOne">The first character.</param>
        /// <param name="characterTwo">The second character.</param>
        /// <param name="affinityPair">The affinity pair between the two characters.</param>
        private void AddEdge(Character characterOne, Character characterTwo, AffinityPair affinityPair)
        {
            bool firstOrderAdd =
                EdgesAndInformation.TryAdd((characterOne, characterTwo), new EdgeInformation(affinityPair));

            if (firstOrderAdd) return;
            EdgesAndInformation.TryAdd((characterTwo, characterOne), new EdgeInformation(affinityPair));
        }

        /// <summary>
        /// Returns whether the two characters are connected by a single edge.
        /// </summary>
        /// <param name="characterOne">The first character.</param>
        /// <param name="characterTwo">The second character.</param>
        /// <returns>True if there exists an edge between the two characters, false otherwise.</returns>
        public bool AreConnected(Character characterOne, Character characterTwo)
        {
            return EdgesAndInformation.ContainsKey((characterOne, characterTwo)) ||
                   EdgesAndInformation.ContainsKey((characterTwo, characterOne));
        }

        /// <summary>
        /// Returns the affinity pair between the two characters, if there is an edge present between them.
        /// </summary>
        /// <param name="characterOne">The first character.</param>
        /// <param name="characterTwo">The second character.</param>
        /// <returns>The affinity pair or null.</returns>
        public AffinityPair GetAffinityPair(Character characterOne, Character characterTwo)
        {
            if (!AreConnected(characterOne, characterTwo)) return null;

            EdgesAndInformation.TryGetValue((characterOne, characterTwo), out EdgeInformation edgeInformation);

            if (edgeInformation != null) return edgeInformation.AffinityPair;

            EdgesAndInformation.TryGetValue((characterTwo, characterOne), out edgeInformation);

            return edgeInformation?.AffinityPair;
        }

        /// <summary>
        /// Updates the current card being displayed when a new one has been selected.
        /// </summary>
        /// <param name="card">The card.</param>
        /// <param name="weekNumber">The new week number.</param>
        private void UpdateCurrentCard(Card card, int weekNumber)
        {
            _currentCard = card;
        }

        /// <summary>
        /// Updates the in-game graph by updating the affinities between characters based on the choice made by the
        /// player. If the characters are not connected by an edge, a new edge is added. Otherwise, the existing edge is
        /// updated.
        /// </summary>
        /// <param name="choice">The choice made by the player.</param>
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
        /// Unsubscribes from game events.
        /// </summary>
        ~InGameGraph()
        {
            GameEvent.OnCardSelected -= UpdateCurrentCard;
            GameEvent.OnChoiceMade -= UpdateGraph;
        }
    }
}