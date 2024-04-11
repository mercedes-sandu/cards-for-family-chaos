using System.Collections.Generic;
using System.Linq;
using CatSAT;
using CatSAT.SAT;
using GraphVisualizing;
using Imaginarium.Generator;
using Imaginarium.Ontology;
using UnityEngine;

namespace GameSetup
{
    public class Family
    {
        private readonly int _size;
        private readonly string _surname;

        private Problem _problem;
        private Graph _graph;
        private Dictionary<ushort, EdgeProposition> _allPossibleEdges;
        public Dictionary<ushort, EdgeProposition> EdgesInSolution;
        private Solution _solution;
        private GraphViz<Character> _graphViz;
        public Dictionary<int, Character> Characters;

        private bool _isFamilyOne;

        /// <summary>
        /// Initializes a family based off of a given number of members and surname. Creates a graph that is passed to
        /// CatSAT to generate a solution. Creates a GraphViz object to visualize the graph.
        /// </summary>
        /// <param name="size">The number of members in the family (the number of nodes in the graph).</param>
        /// <param name="surname">The surname of the family.</param>
        /// <param name="minDensity"></param>
        /// <param name="maxDensity"></param>
        /// <param name="isFamilyOne"></param>
        public Family(int size, string surname, float minDensity, float maxDensity, bool isFamilyOne)
        {
            _size = size;
            _surname = surname;

            _problem = new Problem();
            _graph = new Graph(_problem, _size);
            _graph.Connected();
            _graph.Density(minDensity, maxDensity);
            _allPossibleEdges = _graph.SATVariableToEdge;
            _solution = _problem.Solve();

            EdgesInSolution = new Dictionary<ushort, EdgeProposition>();
            foreach ((ushort index, EdgeProposition edge) in _graph.SATVariableToEdge)
            {
                if (_solution[edge]) EdgesInSolution.TryAdd(index, edge);
            }

            Characters = new Dictionary<int, Character>();

            _isFamilyOne = isFamilyOne;

            _graphViz = new GraphViz<Character>();
        }

        /// <summary>
        /// Initializes a family based off of two other families. Used to create the "combined" family. Incorporates an
        /// offset to the nodes as well as the indices of the edges to avoid overlap.
        /// </summary>
        /// <param name="size">The size of the combined family (the number of nodes in the first family's graph added
        /// to the number of nodes in the second family's graph).</param>
        /// <param name="familyOne">The first family.</param>
        /// <param name="familyTwo">The second family.</param>
        public Family(int size, Family familyOne, Family familyTwo)
        {
            _size = size;
            _surname = $"{familyOne._surname} and {familyTwo._surname}";
            Characters = new Dictionary<int, Character>();

            _graphViz = new GraphViz<Character>(); // change to pair of int and family number

            // todo: pick some number of edges to add between nodes in family one and nodes in family two
            // should i do this with catsat ?? ^^

            // separate set of edge predicates linking the two graphs
            // separate cardinality constraint on those edge predicates

            // to address relationship types, use generation numbers

            _allPossibleEdges = new Dictionary<ushort, EdgeProposition>();
            EdgesInSolution = new Dictionary<ushort, EdgeProposition>();
            foreach ((ushort index, EdgeProposition edge) in familyOne._allPossibleEdges)
            {
                _allPossibleEdges.TryAdd(index, edge);
                if (familyOne._solution[edge]) EdgesInSolution.TryAdd(index, edge);
            }

            ushort indexOffset = (ushort)familyOne._allPossibleEdges.Count;
            foreach ((ushort index, EdgeProposition edge) in familyTwo._allPossibleEdges)
            {
                _allPossibleEdges.TryAdd((ushort)(index + indexOffset), edge);
                if (familyTwo._solution[edge]) EdgesInSolution.TryAdd((ushort)(index + indexOffset), edge);
            }
        }

        /// <summary>
        /// Sets the characters in the family. Assigns them to the nodes in the graph.
        /// </summary>
        /// <param name="generatedCharacters">A pair (model, list of generated characters).</param>
        public void SetCharacters((Solution, List<PossibleIndividual>) generatedCharacters)
        {
            foreach (PossibleIndividual character in generatedCharacters.Item2)
            {
                int index = Characters.Count;
                Characters.Add(index, new Character(character, _surname, generatedCharacters.Item1, _isFamilyOne));
            }

            // todo: is there a way to specify which node styles to use for which nodes?
            foreach ((ushort index, EdgeProposition edge) in EdgesInSolution)
            {
                _graphViz.AddEdge(new GraphViz<Character>.Edge(Characters[edge.SourceVertex],
                    Characters[edge.DestinationVertex]));
            }
        }

        /// <summary>
        /// Sets the characters in the combined family graph. Assigns them to the nodes in the graph.
        /// </summary>
        /// <param name="familyOne">The first family which was generated.</param>
        /// <param name="familyTwo">The second family which was generated.</param>
        public void SetCharacters(Family familyOne, Family familyTwo)
        {
            foreach ((int index, Character character) in familyOne.Characters)
            {
                Characters.Add(index, character);
            }

            foreach ((int index, Character character) in familyTwo.Characters)
            {
                Characters.Add(index + familyOne._size, character);
            }

            foreach (EdgeProposition edge in familyOne.EdgesInSolution.Values)
            {
                _graphViz.AddEdge(new GraphViz<Character>.Edge(Characters[edge.SourceVertex],
                    Characters[edge.DestinationVertex]));
            }

            foreach (EdgeProposition edge in familyTwo.EdgesInSolution.Values)
            {
                _graphViz.AddEdge(new GraphViz<Character>.Edge(Characters[edge.SourceVertex + familyOne._size],
                    Characters[edge.DestinationVertex + familyOne._size]));
            }
        }

        /// <summary>
        /// Shows the graph in the GraphVisualizer canvas.
        /// </summary>
        public void ShowGraph()
        {
            GraphVisualizer.ShowGraph(_graphViz);
        }

        /// <summary>
        /// Prints the edges present in the family's graph.
        /// </summary>
        public void PrintEdges()
        {
            if (_solution == null)
            {
                Debug.Log(_surname + " Families: \n" + EdgesInSolution.Aggregate("", (current, edge) => current +
                    edge.Key +
                    ": " +
                    edge.Value.SourceVertex + "--" + edge.Value.DestinationVertex + "\n"));
            }
            else
            {
                Debug.Log(_surname + " Family: \n" + EdgesInSolution.Aggregate("",
                    (current, edge) => current + edge.Key + ": " + edge.Value.SourceVertex + "--" +
                                       edge.Value.DestinationVertex + "\n"));
            }
        }
    }
}