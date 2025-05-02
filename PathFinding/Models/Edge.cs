namespace PathFindingClassDiagram.PathFinding.Models
{
    public class Edge
    {
        public Node Source { get; set; }
        public Node Destination { get; set; }
        public double Weight { get; set; }

        public Edge(Node source, Node destination, double weight)
        {
            Source = source;
            Destination = destination;
            Weight = weight;
        }
    }
}
