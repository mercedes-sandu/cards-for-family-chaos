using System.Collections.Generic;
using System.Linq;
using CatSAT;
using CatSAT.SAT;
using GraphVisualizing;
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
        private GraphViz<int> _graphViz;

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
            
            _graphViz = new GraphViz<int>();
            // todo: is there a way to specify which node styles to use for which nodes?
            foreach (var (index, edge) in _edges.Where(edge => _solution[edge.Value]))
            {
                _graphViz.AddEdge(new GraphViz<int>.Edge(edge.SourceVertex, edge.DestinationVertex));
            }
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
            
            _graphViz = new GraphViz<int>(); // change to pair of int and family number
            
            // todo: pick some number of edges to add between nodes in family one and nodes in family two
            // should i do this with catsat ?? ^^
            
            // separate set of edge predicates linking the two graphs
            // separate cardinality constraint on those edge predicates
            
            // to address relationship types, use generation numbers
            
            _edges = new Dictionary<ushort, EdgeProposition>();
            foreach (var (index, edge) in familyOne._edges)
            {
                _edges.TryAdd(index, edge);
                if (familyOne._solution[edge])
                {
                    _graphViz.AddEdge(new GraphViz<int>.Edge(edge.SourceVertex, edge.DestinationVertex));
                }
            }
            
            ushort indexOffset = (ushort) familyOne._edges.Count;
            foreach (var (index, edge) in familyTwo._edges)
            {
                _edges.TryAdd((ushort) (index + indexOffset), edge);
                if (familyTwo._solution[edge])
                {
                    _graphViz.AddEdge(new GraphViz<int>.Edge(edge.SourceVertex + familyOne._size,
                        edge.DestinationVertex + familyOne._size));
                }
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