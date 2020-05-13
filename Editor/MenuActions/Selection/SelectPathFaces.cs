﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.ProBuilder.MeshOperations;
using System.Linq;

namespace UnityEditor.ProBuilder.Actions
{
    public static class SelectPathFaces
    {
        private static List<int> visited = new List<int>();
        private static List<int> unvisited = new List<int>();

        public static List<int> GetPath(int start, int end, ProBuilderMesh mesh)
        {
           var path = GetMinimalPath(Dijkstra(start, end, mesh), start, end);
           return path;
        }

        private static int[] Dijkstra(int start, int end, ProBuilderMesh mesh)
        {
            visited.Clear();
            unvisited.Clear();
            int faceCount = mesh.faceCount;
         
            double[] weights = new double[faceCount];
            int[] P = new int[faceCount];
            List<Node> nodes = GetNodes(start, mesh);

            for (int i = 0; i < faceCount; i++)
            {
                weights[i] = double.MaxValue;
                unvisited.Add(i);
            }

          
            int u = start;
            weights[u] = 0;
            visited.Add(u);
            unvisited.Remove(u);

            do
            {
                for (int v = 0; v < unvisited.Count; v++)
                {
                    var node = nodes.Where(x => x.Start == u && x.End == unvisited[v]).FirstOrDefault();
                    if (node.Weight == 0)
                        continue;
                    if (weights[u] + node.Weight < weights[unvisited[v]])
                    {
                        var idx = unvisited[v];
                        var idxU = u;
                        weights[unvisited[v]] = weights[u] + node.Weight;
                        P[unvisited[v]] = u;
                    }
                    else
                    {
                        var idx = unvisited[v];
                        var idxU = u;
                    }
                }

                double min = double.MaxValue;

                for (int i = 0; i < visited.Count; i++)
                {
                    for (int j = 0; j < unvisited.Count; j++)
                    {
                        var node = nodes.Where(x => x.Start == visited[i] && x.End == unvisited[j]).FirstOrDefault();
                        if (node.Weight == 0)
                            continue;
                        if (node.Weight < min)
                        {
                            min = node.Weight;
                            u = unvisited[j];
                        }
                    }
                }

                visited.Add(u);
                unvisited.Remove(u);

            } while (visited.Count < faceCount && !visited.Contains(end));

            return P;
        }

        private static List<Node> GetNodes(int start, ProBuilderMesh mesh)
        {
            List<Node> list = new List<Node>();
            foreach (var face in mesh.faces)
            {
                foreach (var other in mesh.faces)
                {
                    if (face == other)
                        continue;
                    foreach (var edge in face.edges)
                    {
                        var neighbors = ElementSelection.GetNeighborFaces(mesh, edge);
                        foreach (var neighbor in neighbors)
                        {
                            if (neighbor.item1 != other)
                                continue;
                            Node node = new Node(mesh.faces.IndexOf(face), mesh.faces.IndexOf(neighbor.item1), 1);
                            list.Add(node);
                        }
                    }
                }
            }

            return list;
        }

        private static double GetWeight(Face face1, Face face2)
        {
            double baseCost = 10.0;
            double normalCost = 2.0;
            double distCost = 3.0;

            return 0.0;
        }

        private static List<int> GetMinimalPath(int[] P, int start, int end)
        {
            List<int> list = new List<int>();
            list.Add(end);
            int a = end;
            while (a != start)
            {
                a = P[a];
                list.Add(a);
            }
            for (int i = 0, j = list.Count - 1; i < j; i++)
            {
                var item = list[j];
                list.Remove(item);
                list.Insert(i, item);
            }
            return list;

        }

        private struct Node
        {
            public int Start { get; private set; }
            public int End { get; private set; }
            public double Weight { get; private set; }

            public Node(int start, int end, int weight)
            {
                Start = start;
                End = end;
                Weight = weight;
            }
        }
    }
}
