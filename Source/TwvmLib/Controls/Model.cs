using Microsoft.Msagl.Core.Layout;
using Microsoft.Msagl.Core.Geometry.Curves;
using Geometry = Microsoft.Msagl.Core.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Msagl.GraphmapsWithMesh;
using System.Windows.Shapes;
using System.Windows.Navigation;

namespace TwvmLib.Controls;

internal class Model
{
//    public IEnumerable<GeometryGraph> Layers { get; set; }
    public List<GeometryGraph> Layers { get; set; }

    public Model()
    {
        Layers = new List<GeometryGraph>();
    }

    public Layer GetLayer(string layerName)
    {
        var layer = Layers.FirstOrDefault(g => (string)g.UserData == layerName);
        if (layer == null)
        {
            layer = new() { UserData = layerName };
            Layers.Add(layer);
        }

        return new Layer(layer);
    }
    public List<ModelNode>? GetNodes(string layerName)
    {
        var layer = Layers.FirstOrDefault(g => (string)g.UserData == layerName);
        if (layer == null) return null;


        var nodes = new List<ModelNode>();
            
        foreach (var node in layer.Nodes)
        {
            nodes.Add(new ModelNode((int) node.UserData, node.BoundingBox.Center.X, node.BoundingBox.Center.Y));
        }

        return nodes;
    }

}

public class Layer
{
    private GeometryGraph graph;

    public Layer(GeometryGraph g)
    {
        graph = g;
    }

    public void AddNode(int sector, int posX = 0, int posY = 0)
    {
        var rect = CurveFactory.CreateRectangle(100, 100, 
                   new Geometry.Point(posX, posY));
        graph.Nodes.Add(new Node(rect, sector));
    }

    public void AddEdge(int src, int dst)
    {
        var s = graph.FindNodeByUserData(src);
        var d = graph.FindNodeByUserData(dst);
        if (s == null || d == null) return;

        graph.Edges.Add(new(s, d, 0, 0, 1)
        { UserData = $"{src} - {dst}" });
    }


}

public class ModelNode
{
    public int Id { get; set; }
    public double X { get; set; }
    public double Y { get; set; }

    public ModelNode(int id, double x, double y)
    {
        Id = id;
        X = x;
        Y = y;
    }
}

public class ModelEdge
{
    public Node? Source { get; set; }
    public Node? Destination { get; set; }
    public Path? PathData { get; set; }

    public ModelEdge() { }
}
 
