using System;
using System.Collections.Generic;

namespace PathFindingClassDiagram.Models
{
    /// <summary>
    /// Represents a node in the pathfinding graph
    /// </summary>
    public class Node
    {
        /// <summary>
        /// X coordinate of the node
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y coordinate of the node
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// List of edges connected to this node
        /// </summary>
        public List<Edge> Edges { get; set; } = new List<Edge>();

        /// <summary>
        /// Key identifying this node (derived from coordinates)
        /// </summary>
        public string Key => $"N({X},{Y})";

        /// <summary>
        /// Creates a new instance of the Node class
        /// </summary>
        public Node()
        {
        }

        /// <summary>
        /// Creates a new instance of the Node class with specified coordinates
        /// </summary>
        public Node(double x, double y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Determines equality between nodes based on coordinates
        /// </summary>
        public override bool Equals(object obj)
        {
            if (!(obj is Node other))
            {
                return false;
            }

            return Math.Abs(X - other.X) < 0.001 && Math.Abs(Y - other.Y) < 0.001;
        }

        /// <summary>
        /// Gets the hash code for this node
        /// </summary>
        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }
    }

    /// <summary>
    /// Represents an edge between two nodes in the pathfinding graph
    /// </summary>
    public class Edge
    {
        /// <summary>
        /// Source node of the edge
        /// </summary>
        public Node Source { get; set; }

        /// <summary>
        /// Destination node of the edge
        /// </summary>
        public Node Destination { get; set; }

        /// <summary>
        /// Weight of the edge (distance between nodes)
        /// </summary>
        public double Weight { get; set; }

        /// <summary>
        /// Creates a new instance of the Edge class
        /// </summary>
        public Edge()
        {
        }

        /// <summary>
        /// Creates a new instance of the Edge class with specified nodes
        /// </summary>
        public Edge(Node source, Node destination)
        {
            Source = source;
            Destination = destination;
            Weight = CalculateWeight();
        }

        /// <summary>
        /// Calculates the weight (distance) between the source and destination nodes
        /// </summary>
        private double CalculateWeight()
        {
            if (Source == null || Destination == null)
                return 0;

            double dx = Destination.X - Source.X;
            double dy = Destination.Y - Source.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}