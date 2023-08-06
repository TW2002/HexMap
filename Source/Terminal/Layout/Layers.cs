using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Fdg = EpForceDirectedGraph.cs;

namespace TradeWarsData;

public class Layer : Fdg.Graph
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public object? UserData { get; set; }

    //public Graph graph = new();

    public Layer(string name, object? data = null)
    {
        Id = new Guid();
        Name = name;
        UserData = data;
    }

    public void NewNode(int sector, int posX = 0, int posY = 0)
    {
        Fdg.Node node = CreateNode(new Fdg.NodeData()
        {
            label = $"{sector}",
            mass = 3.0f,
            initialPostion = new Fdg.FDGVector2(posX, posY)
        });
        node.Pinned = true;
        if (posX == 0 && posY == 0) node.Pinned = false;
        AddNode(node);

    }

    public void NewEdge(int source, int target)
    {
        Fdg.Node sn = GetNode($"{source}");
        Fdg.Node tn = GetNode($"{target}");
        if (sn == null || tn == null) return;

        Fdg.Edge edge = CreateEdge(sn, tn, new Fdg.EdgeData()
        {
            label = $"{source} - {target}",
            length = 60.0f
        });
        AddEdge(edge);
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


        float stiffness = 81.76f;
        float repulsion = 40000.0f;
        float damping = 0.5f;

        // 2D Force Directed
        Fdg.ForceDirected2D physics = new Fdg.ForceDirected2D(layer, // instance of Graph

                                                           stiffness, // stiffness of the spring
                                                           repulsion, // node repulsion rate 
                                                           damping    // damping rate  
                                                           );

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


        var nodes = new List<Node>();

        foreach (var node in layer.nodes)
        {
            //nodes.Add(new ModelNode((int)node.Data.label, node.BoundingBox.Center.X, node.BoundingBox.Center.Y));
        }

        return nodes;
    }

}


public class Node : Fdg.Node
{
    public Guid Id { get; set; }
    public int Sector { get; set; }
    public double X { get; set; }
    public double Y { get; set; }


    //public Node(string iId, double x = 0, double y = 0, Fdg.NodeData? iData = null) : base(iId, iData)
    public Node(int sector, double x = 0, double y = 0, Fdg.NodeData? iData = null) : base(sector.ToString(), iData)
    {
        Id = new Guid();
        Sector = sector;
        X = x;
        Y = y;
        base.Data.initialPostion = new Fdg.FDGVector2((int)X, (int)Y);
    }
}

public class ModelEdge
{
    public Fdg.Node? Source { get; set; }
    public Fdg.Node? Destination { get; set; }
    public string? PathData { get; set; }

    public ModelEdge() { }
}


