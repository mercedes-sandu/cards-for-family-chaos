using CatSAT;
using CatSAT.SAT;
using Imaginarium;
using UnityEngine;

public class SetupMaster : MonoBehaviour
{
    private void Awake()
    {
        var p = new Problem();
        var graph = new Graph(p, 5);
        graph.Connected();
        graph.WriteDot(p.Solve(), "unity-test.dot");
        Debug.Log("wrote dot file for 5 vertex graph");
    }
}