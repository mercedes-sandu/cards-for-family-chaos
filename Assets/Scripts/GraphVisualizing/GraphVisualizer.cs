﻿using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using GraphVisualization;
using GraphVisualizing.GraphVisualization;
using GraphVisualizing.Utilities;
using UnityEngine;

// ReSharper disable once ClassNeverInstantiated.Global
namespace GraphVisualizing
{
    public class GraphVisualizer : Graph {
        private static readonly Dictionary<string, Color> NonStandardColorNames;

        public static GraphVisualizer Current;

        public static bool GraphVisible => Current.nodes.Count == 0;

        // ReSharper disable once CollectionNeverUpdated.Local
        private readonly DictionaryWithDefault<string, EdgeStyle> _edgeStyles;
        // ReSharper disable once CollectionNeverUpdated.Local
        private readonly DictionaryWithDefault<string, NodeStyle> _nodeStyles;

        static GraphVisualizer() => NonStandardColorNames = new Dictionary<string, Color>(
            GraphColors.NamedColors.Select(c => new KeyValuePair<string, Color>(c.Name, c.Color)));
        public GraphVisualizer() {
            _nodeStyles = new DictionaryWithDefault<string, NodeStyle>(MakeNodeStyle);
            _edgeStyles = new DictionaryWithDefault<string, EdgeStyle>(MakeEdgeStyle);
            Current = this;
        }

        private NodeStyle MakeNodeStyle(string colorName) {
            var s = NodeStyles[0].Clone();
            var f = typeof(Color).GetField(colorName, BindingFlags.Public | BindingFlags.Static | BindingFlags.IgnoreCase);
            if (f == null) {
                if (NonStandardColorNames.TryGetValue(colorName, out var namedColor)) s.Color = namedColor;
                else Debug.Log($"No such color as {colorName}");
            } else s.Color = (Color)f.GetValue(null);
            return s;
        }

        private EdgeStyle MakeEdgeStyle(string colorName) {
            var s = EdgeStyles[0].Clone();
            var f = typeof(Color).GetField(colorName, BindingFlags.Static | BindingFlags.IgnoreCase);
            if (f == null) {
                if (NonStandardColorNames.TryGetValue(colorName, out var namedColor)) s.Color = namedColor;
                else Debug.Log($"No such color as {colorName}");
            } else s.Color = (Color)f.GetValue(null);
            return s;
        }

        private void SetGraph<T>(GraphViz<T> g) {
            Clear();
            EnsureEdgeNodesInGraph(g);
            if (g.Nodes.Count == 0) return;

            foreach (var n in g.Nodes) {
                var attrs = g.NodeAttributes[n];
                var nodeStyle = NodeStyles[0];
                if (attrs != null && attrs.TryGetValue("fillcolor", out var attr)) nodeStyle = _nodeStyles[(string)attr];
                if (attrs != null && attrs.TryGetValue("rgbcolor", out var rgbcolor)) nodeStyle = (Color)rgbcolor;
                AddNode(n, n.ToString(), nodeStyle);
            }
            foreach (var e in g.Edges) {
                var attrs = e.Attributes;
                var edgeStyle = EdgeStyles[0];
                if (attrs != null && attrs.TryGetValue("color", out var attr)) edgeStyle = _edgeStyles[(string)attr];
                if (attrs != null && attrs.TryGetValue("rgbcolor", out var rgbcolor)) edgeStyle = (Color)rgbcolor;
                AddEdge(e.StartNode, e.EndNode, e.Label, edgeStyle);
            }
            UpdateTopologyStats();
            PlaceComponents();
            RepopulateMesh();
        }

        private static void EnsureEdgeNodesInGraph<T>(GraphViz<T> g) {
            foreach (var e in g.Edges) {
                if (!g.Nodes.Contains(e.StartNode)) g.AddNode(e.StartNode);
                if (!g.Nodes.Contains(e.EndNode)) g.AddNode(e.EndNode);
            }
        }

        public static void ShowGraph<T>(GraphViz<T> g) => FindObjectOfType<GraphVisualizer>().SetGraph(g);
    }
}