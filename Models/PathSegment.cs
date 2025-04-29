using System.Drawing;
using System;

namespace PathFindingClassDiagram.Models
{
    public class PathSegment
    {
        /// <summary>
        /// The starting point of the path segment
        /// </summary>
        public PointF Start { get; set; }

        /// <summary>
        /// The ending point of the path segment
        /// </summary>
        public PointF End { get; set; }

        /// <summary>
        /// The weight of this path segment (used in pathfinding algorithms)
        /// </summary>
        public double Weight => CalculateWeight();

        /// <summary>
        /// Initializes a new instance of the PathSegment class
        /// </summary>
        public PathSegment()
        {
        }

        /// <summary>
        /// Initializes a new instance of the PathSegment class with specified start and end points
        /// </summary>
        public PathSegment(PointF start, PointF end)
        {
            Start = start;
            End = end;
        }

        /// <summary>
        /// Calculates the weight of this segment (Euclidean distance)
        /// </summary>
        private double CalculateWeight()
        {
            if (Start == null || End == null)
                return 0;

            double dx = End.X - Start.X;
            double dy = End.Y - Start.Y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        /// <summary>
        /// Determines whether the specified point lies on this segment
        /// </summary>
        public bool Contains(double x, double y)
        {
            if (Start == null || End == null)
                return false;

            return (x >= Math.Min(Start.X, End.X) && x <= Math.Max(Start.X, End.X)) &&
                   (y >= Math.Min(Start.Y, End.Y) && y <= Math.Max(Start.Y, End.Y));
        }
    }
}
