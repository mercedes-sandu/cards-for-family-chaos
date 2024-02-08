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
        private Dictionary<ushort, EdgeProposition> _edges;
        private Solution _solution;
        private GraphViz<Character> _graphViz; // todo: change to character
        private Dictionary<int, Character> _characters;

        /// <summary>
        /// Initializes a family based off of a given number of members and surname. Creates a graph that is passed to
        /// CatSAT to generate a solution. Creates a GraphViz object to visualize the graph.
        /// </summary>
        /// <param name="size">The number of members in the family (the number of nodes in the graph).</param>
        /// <param name="surname">The surname of the family.</param>
        /// <param name="minDensity"></param>
        /// <param name="maxDensity"></param>
        public Family(int size, string surname, float minDensity, float maxDensity)
        {
            _size = size;
            _surname = surname;

            _problem = new Problem();
            _graph = new Graph(_problem, _size);
            _graph.Connected();
            _graph.Density(minDensity, maxDensity);
            _edges = _graph.SATVariableToEdge;
            _solution = _problem.Solve();
            _characters = new Dictionary<int, Character>();

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
            _characters = new Dictionary<int, Character>();

            _graphViz = new GraphViz<Character>(); // change to pair of int and family number

            // todo: pick some number of edges to add between nodes in family one and nodes in family two
            // should i do this with catsat ?? ^^

            // separate set of edge predicates linking the two graphs
            // separate cardinality constraint on those edge predicates

            // to address relationship types, use generation numbers

            _edges = new Dictionary<ushort, EdgeProposition>();
            foreach (var (index, edge) in familyOne._edges)
            {
                _edges.TryAdd(index, edge);
            }

            ushort indexOffset = (ushort)familyOne._edges.Count;
            foreach (var (index, edge) in familyTwo._edges)
            {
                _edges.TryAdd((ushort)(index + indexOffset), edge);
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
                int index = _characters.Count;
                _characters.Add(index, new Character(character, _surname, generatedCharacters.Item1));
            }

            // todo: is there a way to specify which node styles to use for which nodes?
            foreach (var (index, edge) in _edges.Where(edge => _solution[edge.Value]))
            {
                _graphViz.AddEdge(new GraphViz<Character>.Edge(_characters[edge.SourceVertex],
                    _characters[edge.DestinationVertex]));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="familyOne"></param>
        /// <param name="familyTwo"></param>
        public void SetCharacters(Family familyOne, Family familyTwo)
        {
            foreach (var (index, character) in familyOne._characters)
            {
                _characters.Add(index, character);
            }

            foreach (var (index, character) in familyTwo._characters)
            {
                _characters.Add(index + familyOne._size, character);
            }

            var edges = _edges.Values.ToList();
            
            foreach (var edge in edges.GetRange(0, familyOne._size).Where(edge => familyOne._solution[edge]))
            {
                _graphViz.AddEdge(new GraphViz<Character>.Edge(_characters[edge.SourceVertex],
                    _characters[edge.DestinationVertex]));
            }
            
            foreach (var edge in edges.GetRange(familyOne._size, familyTwo._size).Where(edge => familyTwo._solution[edge]))
            {
                _graphViz.AddEdge(new GraphViz<Character>.Edge(_characters[edge.SourceVertex],
                    _characters[edge.DestinationVertex]));
            }
        }

        /// <summary>
        /// Returns the dictionary of edges in the graph representing the family.
        /// NOTE: If this is of a single family, then all edges (whether actually present in the solution or not) are
        /// included in the dictionary. If this is of a combined family, then only the edges present in the solution are
        /// included in the dictionary.
        /// </summary>
        /// <returns></returns>
        public Dictionary<ushort, EdgeProposition> GetEdges() => _edges;

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
                Debug.Log(_surname + " Families: \n" + _edges.Aggregate("", (current, edge) => current + edge.Key +
                    ": " +
                    edge.Value.SourceVertex + "--" + edge.Value.DestinationVertex + "\n"));
            }
            else
            {
                Debug.Log(_surname + " Family: \n" + _edges.Where(edge => _solution[edge.Value]).Aggregate("",
                    (current, edge) => current + edge.Key + ": " + edge.Value.SourceVertex + "--" +
                                       edge.Value.DestinationVertex + "\n"));
            }
        }
    }
}