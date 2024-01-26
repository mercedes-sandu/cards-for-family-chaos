using System.Linq;
using CatSAT;
using CatSAT.SAT;
using GraphVisualizing;
using Imaginarium;
using UnityEngine;

public class SetupMaster : MonoBehaviour
{
    private Problem _problem;
    private Graph _graph;
    
    private void Awake()
    {
        _problem = new Problem();
        _graph = new Graph(_problem, 5);
        _graph.Connected();
        var edges = _graph.SATVariableToEdge.Values;
        var solution = _problem.Solve();
        
        // var graphViz = new GraphViz<int>();
        // foreach (var edge in edges)
        // {
        //     graphViz.AddEdge(new GraphViz<int>.Edge(edge.SourceVertex, edge.DestinationVertex));
        // }
        //
        // GraphVisualizer.ShowGraph(graphViz);
        
        Debug.Log(edges.Where(edge => solution[edge]).Aggregate("",
            (current, edge) => current + edge.SourceVertex + "--" + edge.DestinationVertex + "\n"));
    }
}