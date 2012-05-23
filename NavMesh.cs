using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Mogre;

namespace Gra
{
    public class NavMesh
    {
        public class Edge
        {
            public Vector3 A, B;
            public List<Triangle> Triangles = new List<Triangle>();
        }

        public class Triangle
        {
            public Edge U, V, W;
            public float Area;

            public Vector3 Center;
            public bool IsVisited;
            public float Cost;
            public Triangle Previous;

            public bool IsPointInside(Vector3 pt)
            {
                float area = 0;
                area += Op2D.TriArea(U.A - pt, U.B - pt);
                area += Op2D.TriArea(V.A - pt, V.B - pt);
                area += Op2D.TriArea(W.A - pt, W.B - pt);
                if (area > Area * 1.001f)
                    return false;
                return true;
            }

            public void CalcArea()
            {
                Center = (U.A + U.B + V.A + V.B + W.A + W.B) / 6.0f;
                Area = Op2D.TriArea(U.B - U.A, V.B - V.A);
            }
        }

        public List<Vector3> Vertices = new List<Vector3>();
        public List<Edge> Edges = new List<Edge>();
        public List<Triangle> Triangles = new List<Triangle>();

        PriorityQueue<Triangle> SearchQueue = new PriorityQueue<Triangle>();
        public List<Triangle> TriPath = new List<Triangle>();
        public Vector3 StartPt, EndPt;

        public List<Pair<Vector3, Vector3>> Portals = new List<Pair<Vector3, Vector3>>();

        public Edge AddEdge(Vector3 a, Vector3 b)
        {
            Edge edge = Edges.Find(e => e.A == a && e.B == b || e.A == b && e.B == a);
            if (edge != null)
                return edge;
            else
            {
                edge = new Edge();
                edge.A = a;
                edge.B = b;
                Edges.Add(edge);
                return edge;
            }
        }

        public void LoadFromOBJ(string fname)
        {
            string[] lines = File.ReadAllLines(fname);
            string[] tokens;
            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i][0] == 'v')
                {
                    tokens = lines[i].Split(' ');
                    Vector3 vertex = new Vector3();
                    vertex.x = Single.Parse(tokens[1], System.Globalization.CultureInfo.InvariantCulture);
                    vertex.y = Single.Parse(tokens[2], System.Globalization.CultureInfo.InvariantCulture);
                    vertex.z = Single.Parse(tokens[3], System.Globalization.CultureInfo.InvariantCulture);
                    Vertices.Add(vertex);
                }

                else if (lines[i][0] == 'f')
                {
                    tokens = lines[i].Split(' ');
                    int v1 = Int32.Parse(tokens[1]) - 1;
                    int v2 = Int32.Parse(tokens[2]) - 1;
                    int v3 = Int32.Parse(tokens[3]) - 1;
                    Triangle tri = new Triangle();
                    tri.U = AddEdge(Vertices[v1], Vertices[v2]);
                    tri.V = AddEdge(Vertices[v2], Vertices[v3]);
                    tri.W = AddEdge(Vertices[v3], Vertices[v1]);
                    tri.U.Triangles.Add(tri);
                    tri.V.Triangles.Add(tri);
                    tri.W.Triangles.Add(tri);
                    Triangles.Add(tri);
                    tri.CalcArea();
                }
            }
        }

        public void AStar(Vector3 start, Vector3 end)
        {
            StartPt = start;
            EndPt = end;
            Triangle startTri = Triangles.Find(tri => tri.IsPointInside(start));
            Triangle endTri = Triangles.Find(tri => tri.IsPointInside(end));
            Triangles.ForEach(tri => tri.IsVisited = false);
            Triangles.ForEach(tri => tri.Cost = 0);
            SearchQueue.Clear();
            TriPath.Clear();

            if (startTri == null || endTri == null)
                return;

            SearchQueue.Push((start - end).Length, startTri);

            while (!SearchQueue.IsEmpty())
            {
                Triangle currTri = SearchQueue.Pop();
                if (currTri == endTri)
                {
                    while (currTri != startTri)
                    {
                        TriPath.Insert(0, currTri);
                        currTri = currTri.Previous;
                    }
                    TriPath.Insert(0, startTri);
                    break;
                }
                if (currTri.IsVisited)
                    continue;

                currTri.IsVisited = true;
                foreach (Triangle neighbour in
                    currTri.U.Triangles.Concat(currTri.V.Triangles.Concat(currTri.W.Triangles)))
                {
                    if (neighbour.IsVisited)
                        continue;
                    float nCost = currTri.Cost + (neighbour.Center - currTri.Center).Length;
                    if (nCost < neighbour.Cost || neighbour.Cost == 0)
                    {
                        neighbour.Cost = nCost;
                        neighbour.Previous = currTri;
                        SearchQueue.Push(nCost + (end - neighbour.Center).Length, neighbour);
                    }
                }
            }
        }

        public void GetPortals()
        {
            Portals.Clear();

            for (int i = 0; i < TriPath.Count - 1; i++)
            {
                Pair<Vector3, Vector3> portal = new Pair<Vector3, Vector3>();

                if (TriPath[i].U == TriPath[i + 1].U
                  || TriPath[i].U == TriPath[i + 1].V || TriPath[i].U == TriPath[i + 1].W)
                    portal = new Pair<Vector3, Vector3>(TriPath[i].U.A, TriPath[i].U.B);
                else if (TriPath[i].V == TriPath[i + 1].U
                  || TriPath[i].V == TriPath[i + 1].V || TriPath[i].V == TriPath[i + 1].W)
                    portal = new Pair<Vector3, Vector3>(TriPath[i].V.A, TriPath[i].V.B);
                else
                    portal = new Pair<Vector3, Vector3>(TriPath[i].W.A, TriPath[i].W.B);

                if (Op2D.Cross(portal.first - TriPath[i].Center, portal.second - TriPath[i].Center) < 0)
                {
                    Vector3 tmp = portal.first;
                    portal.first = portal.second;
                    portal.second = tmp;
                }
                Portals.Add(portal);
            }
            Portals.Add(new Pair<Vector3, Vector3>(EndPt, EndPt));
        }

        public List<Vector3> Funnel()
        {
            List<Vector3> path = new List<Vector3>();
            Vector3 right, left, current;
            int rightId, leftId;

            right = left = current = StartPt;
            rightId = leftId = 0;

            for (int i = 0; i < Portals.Count; i++)
            {
                if (left == current || Op2D.Cross(left - current, Portals[i].first - current) >= 0)
                {
                    if (Op2D.Cross(right - current, Portals[i].first - current) > 0)
                    {
                        path.Add(right);
                        current = left = right;
                        i = leftId = rightId;
                        continue;
                    }
                    else
                    {
                        left = Portals[i].first;
                        leftId = i;
                    }
                }

                if (right == current || Op2D.Cross(right - current, Portals[i].second - current) <= 0)
                {
                    if (Op2D.Cross(left - current, Portals[i].second - current) < 0)
                    {
                        path.Add(left);
                        current = right = left;
                        i = rightId = leftId;
                        continue;
                    }
                    else
                    {
                        right = Portals[i].second;
                        rightId = i;
                    }
                }
            }

            path.Add(EndPt);

			

			Engine.Singleton.SceneManager.DestroyManualObject("line");

			// destroy the SceneNode (or keep it to add other manual objects)
			Engine.Singleton.SceneManager.DestroySceneNode("line_node");  

			ManualObject manOb = Engine.Singleton.SceneManager.CreateManualObject("line");
			manOb.Begin("line_material", RenderOperation.OperationTypes.OT_LINE_LIST);
			
			for (int i = 1; i < path.Count; i++)
			{
				Vector3 adam = path[i-1];
				adam.y = 0.4f;
				manOb.Position(adam);

				Vector3 adam1 = path[i];
				adam1.y = 0.4f;
				manOb.Position(adam1);
				
			}

			manOb.End();
			SceneNode moNode = Engine.Singleton.SceneManager.RootSceneNode.CreateChildSceneNode("line_node");

			moNode.SetPosition(0, 0, 0);
			
			moNode.AttachObject(manOb);

            return path;
        }
    }
}
