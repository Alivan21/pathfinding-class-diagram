using System;

namespace PathFindingClassDiagram.PathFinding.Models
{
    public class Node : IComparable<Node>
    {
        public double X { get; set; }
        public double Y { get; set; }
        public string Key => $"N({X},{Y})";
        public PathPoint Position => new PathPoint(X, Y);

        public double Distance { get; set; } = double.MaxValue;
        public Node Previous { get; set; }
        public bool Visited { get; set; }

        public Node(double x, double y)
        {
            X = x;
            Y = y;
        }

        public int CompareTo(Node other)
        {
            return Distance.CompareTo(other.Distance);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Node other))
                return false;

            return X.Equals(other.X) && Y.Equals(other.Y);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(X, Y);
        }
    }

}
