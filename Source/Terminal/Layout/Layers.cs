using EpForceDirectedGraph.cs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Xml.Linq;
using Fdg = EpForceDirectedGraph.cs;

namespace TradeWarsData;

public class Layer
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public object? Data { get; set; }
    public List<Node> Nodes { get; set; }
    public List<Edge> Edges { get; set; }

    internal Fdg.Graph fGraph = new();
    internal Fdg.ForceDirected2D? fPhysics;

    private System.Timers.Timer tick = new(200);
    private int ticks = 0;
    private Random rnd = new();

    public NodeMovedEventHandler? NodeMoved;
    public delegate void NodeMovedEventHandler(object sender, NodeMovedEventArgs e);


    public Layer(string name, object? data = null)
    {
        Id = new Guid();
        Name = name;
        Data = data;

        Nodes = new List<Node>();
        Edges = new List<Edge>();

        tick.AutoReset = true;
        tick.Enabled = false;
        tick.Elapsed += OnTick;
    }

    public void Run()
    {
        float stiffness = 81.76f;
        float repulsion = 300000.0f;
        float damping = 0.5f;

        // 2D Force Directed
        fPhysics = new(fGraph, stiffness, repulsion, damping);

        ticks = 20;
        tick.Enabled = true;
    }

    public void UpdateNode(int sector, double x, double y)
    {
        if (fPhysics == null) return;

        Fdg.Node fn = fGraph.GetNode($"{sector}");
        fn.Pinned = true;
        fn.Data.mass = 150.0f;

        fPhysics.GetPoint(fn).position.x = (float)x;
        fPhysics.GetPoint(fn).position.y = (float)y;

        ticks = 5;
        tick.Enabled = true;

    }


    public void Stop()
    {
        fPhysics = null;

        tick.Enabled = false;
    }

    private void OnTick(object? sender, ElapsedEventArgs e)
    {
        if (ticks-- < 1 || fPhysics == null)
        {
            tick.Enabled = false;

            foreach (Node node in Nodes)
            {
                Fdg.Node fn = fGraph.GetNode($"{node.Sector}");
                fn.Pinned = node.Pinned;

            }

            return;
        }

        fPhysics.Calculate(0.2f);
        fPhysics.EachNode((Fdg.Node fNode, Point point) =>
        {
            if (fNode != null && point != null && NodeMoved != null)
            {
                Node? node = Nodes.Find(n => n.Sector.ToString() == fNode.Data.label);
                if (node != null)
                //&& (node.X != point.position.x
                //                 || node.Y != point.position.y))
                {
                    //node.X = point.position.x;
                    //node.Y = point.position.y;

                    NodeMovedEventArgs eArgs = new(node, point.position.x, point.position.y);
                    NodeMoved(this, eArgs);
                }
            }
        });
    }

    public void NewNode(int sector, object userdata, double x = 0, double y = 0)
    {
        // Don't create node if it already exists. 
        if (Nodes.Find(n => n.Sector == sector) != null) 
            return;


        bool pinned = (x > 0 || y > 0);
        if (!pinned)
        {
            x = rnd.NextDouble() * 1500;
            y = rnd.NextDouble() * 1500;
        }



        Nodes.Add(new Node(sector, userdata)
        { Pinned = pinned });


        Fdg.Node fNode = fGraph.CreateNode(new Fdg.NodeData() {
            label = $"{sector}",
            mass = 10.0f,
            initialPostion = new Fdg.FDGVector2((float)x, (float)y) });
        fNode.Pinned = pinned;
        fGraph.AddNode(fNode);
    }

    public void NewEdge(int source, int target, float length = 10.0f)
    {
        // Don't create edge if it already exists. 
        if (Edges.Find(n => n.Source.Sector == source && n.Target.Sector == target) != null)
            return;

        // Search for two-way edges.
        Edge? twoway = Edges.Find(n => n.Source.Sector == target && n.Target.Sector == source);
        if (twoway != null)
        {
            twoway.TwoWay = true;
            return;
        }


            Node? sn = Nodes.Find(n => n.Sector == source);
        Node? tn = Nodes.Find(n => n.Sector == target);
        if (sn == null || tn == null) return;

        Edges.Add(new (sn, tn, length));


        Fdg.Node fsn = fGraph.GetNode($"{source}");
        Fdg.Node ftn = fGraph.GetNode($"{target}");
        if (fsn == null || ftn == null) return;

        Fdg.Edge fEdge = fGraph.CreateEdge(fsn, ftn, new Fdg.EdgeData() {
            label = $"{source} - {target}",
            length = length
        });
        fGraph.AddEdge(fEdge);
    }
}


public class Layers : LinkedList<Layer>
{
    //private LinkedList<Layer> layers = new LinkedList<Layer>();

    public Layers() { }
    public void Calculate(string layerName)
    {
        var layer = this.FirstOrDefault(g => g.Name == layerName);
        if (layer == null) return;



        //forceDirected.EachEdge(delegate (Edge edge, Spring spring)
        //{
        //    drawEdge(edge, spring.point1.position, spring.point2.position);
        //});

        //        InteractiveEdgeRouter portRouter = new InteractiveEdgeRouter(Layers[0],
        //            Nodes.Select(n => n.BoundaryCurve), 3, 0.65 * 3, 0);
        //        portRouter.Run();
    }


    public Layer GetLayer(string layerName)
    {
        var layer = this.FirstOrDefault(g => g.Name == layerName);
        if (layer == null)
        {
            layer = new(layerName);
            AddLast(layer);

            //Graph m_fdgGraph = new();
        }

        //FDGVector2 vec = ScreenToGraph(new Pair<int, int>(e.Location.X, e.Location.Y));
        //clickedNode.Pinned = true;
        //m_fdgPhysics.GetPoint(clickedNode).position = vec;



        return layer;
    }
    public List<Node>? GetNodes(string layerName)
    {
        var layer = this.FirstOrDefault(g => g.Name == layerName);
        if (layer == null) return null;

        return layer.Nodes;
    }

    public List<Edge>? GetEdges(string layerName)
    {
        var layer = this.FirstOrDefault(g => g.Name == layerName);
        if (layer == null) return null;

        return layer.Edges;
    }
}

public class Node
{
    //public Guid Id { get; set; }
    public int Sector { get; set; }
    //public double X { get; set; }
    //public double Y { get; set; }
    public bool Pinned { get; set; }
    public object UserData { get; set; }


    //public Node(string iId, double x = 0, double y = 0, Fdg.NodeData? iData = null) : base(iId, iData)
    //public Node(int sector, double x = 0, double y = 0)
    public Node(int sector, object userdata)
    {
        UserData = userdata;
        //Id = new Guid();
        Sector = sector;
        //X = x;
        //Y = y;
        //base.Data.initialPostion = new Fdg.FDGVector2((int)X, (int)Y);
    }
}

public class Edge
{
    //public Guid Id { get; set; }
    public Node Source { get; set; }
    public Node Target { get; set; }
    public float Length { get; set; }
    public bool TwoWay { get; set; }

    public string? PathData { get; set; }

    public Edge(Node source, Node target, float length = 60f) 
    {
        //Id = new Guid();
        Source = source;
        Target = target;
        Length = length;

    }
}

public class NodeMovedEventArgs : EventArgs
{
    //public Guid Id { get; set; }
    //public int Sector { get; set; }
    public double X { get; set; }
    public double Y { get; set; }
    //public bool Pinned { get; set; }
    public object UserData { get; set; }


    public NodeMovedEventArgs(Node node, double x, double y)
    {
        //Id = node.Id;
        //Sector = node.Sector;
        UserData = node.UserData;
        X = x;
        Y = y;
        //Pinned = node.Pinned;
    }
}




















//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using System.Xml.Linq;
//using Fdg = EpForceDirectedGraph.cs;

//namespace TradeWarsData;

//public class Layer : Fdg.Graph
//{
//    public Guid Id { get; set; }
//    public string Name { get; set; }
//    public object? UserData { get; set; }

//    //public Graph graph = new();

//    public Layer(string name, object? data = null)
//    {
//        Id = new Guid();
//        Name = name;
//        UserData = data;
//    }

//    public void NewNode(int sector, int posX = 0, int posY = 0)
//    {
//        Fdg.Node node = CreateNode(new Fdg.NodeData()
//        {
//            label = $"{sector}",
//            mass = 3.0f,
//            initialPostion = new Fdg.FDGVector2(posX, posY)
//        });
//        node.Pinned = true;
//        if (posX == 0 && posY == 0) node.Pinned = false;
//        AddNode(node);

//    }

//    public void NewEdge(int source, int target)
//    {
//        Fdg.Node sn = GetNode($"{source}");
//        Fdg.Node tn = GetNode($"{target}");
//        if (sn == null || tn == null) return;

//        Fdg.Edge edge = CreateEdge(sn, tn, new Fdg.EdgeData()
//        {
//            label = $"{source} - {target}",
//            length = 60.0f
//        });
//        AddEdge(edge);
//    }
//}


//public class Layers : LinkedList<Layer>
//{
//    //private LinkedList<Layer> layers = new LinkedList<Layer>();

//    public Layers() { }
//    public void Calculate(string layerName)
//    {
//        var layer = this.FirstOrDefault(g => g.Name == layerName);
//        if (layer == null) return;


//        float stiffness = 81.76f;
//        float repulsion = 40000.0f;
//        float damping = 0.5f;

//        // 2D Force Directed
//        Fdg.ForceDirected2D physics = new Fdg.ForceDirected2D(layer, // instance of Graph

//                                                           stiffness, // stiffness of the spring
//                                                           repulsion, // node repulsion rate 
//                                                           damping    // damping rate  
//                                                           );

//        //        InteractiveEdgeRouter portRouter = new InteractiveEdgeRouter(Layers[0],
//        //            Nodes.Select(n => n.BoundaryCurve), 3, 0.65 * 3, 0);
//        //        portRouter.Run();
//    }


//    public Layer GetLayer(string layerName)
//    {
//        var layer = this.FirstOrDefault(g => g.Name == layerName);
//        if (layer == null)
//        {
//            layer = new(layerName);
//            AddLast(layer);

//            //Graph m_fdgGraph = new();
//        }

//        //FDGVector2 vec = ScreenToGraph(new Pair<int, int>(e.Location.X, e.Location.Y));
//        //clickedNode.Pinned = true;
//        //m_fdgPhysics.GetPoint(clickedNode).position = vec;



//        return layer;
//    }
//    public List<Node>? GetNodes(string layerName)
//    {
//        var layer = this.FirstOrDefault(g => g.Name == layerName);
//        if (layer == null) return null;


//        var ln = new List<Node>();

//        foreach (var node in layer.nodes)
//        {
//            ln.Add(new Node((int)node.Data.label, node.X.Center.X, node.BoundingBox.Center.Y));
//        }

//        return ln;
//    }

//}


//public class Node : Fdg.Node
//{
//    public Guid Id { get; set; }
//    public int Sector { get; set; }
//    public double X { get; set; }
//    public double Y { get; set; }


//    //public Node(string iId, double x = 0, double y = 0, Fdg.NodeData? iData = null) : base(iId, iData)
//    public Node(int sector, double x = 0, double y = 0, Fdg.NodeData? iData = null) : base(sector.ToString(), iData)
//    {
//        Id = new Guid();
//        Sector = sector;
//        X = x;
//        Y = y;
//        base.Data.initialPostion = new Fdg.FDGVector2((int)X, (int)Y);
//    }
//}

//public class ModelEdge
//{
//    public Fdg.Node? Source { get; set; }
//    public Fdg.Node? Destination { get; set; }
//    public string? PathData { get; set; }

//    public ModelEdge() { }
//}


