using System.Collections.Generic;
using System.Drawing;
using PathFindingClassDiagram.Models;

namespace PathFindingClassDiagram.Services
{
    /// <summary>
    /// Handles collision detection for pathfinding
    /// </summary>
    public class CollisionDetection
    {
        /// <summary>
        /// Detects collisions between line segments and rectangular areas
        /// </summary>
        public List<PathSegment> DetectCollisions(List<PathSegment> segments, List<ClassDiagram> obstacles)
        {
            var result = new List<PathSegment>();

            foreach (var segment in segments)
            {
                bool collision = false;

                // Check for collisions with any class diagram
                foreach (var obstacle in obstacles)
                {
                    var points = obstacle.Points;
                    if (points.Count == 0) continue;

                    foreach (var point in points)
                    {
                        // Create a rectangle from class diagram point
                        var rect = new Rectangle(
                            point.TopLeft.X,
                            point.TopLeft.Y,
                            point.BottomRight.X - point.TopLeft.X,
                            point.BottomRight.Y - point.TopLeft.Y
                        );

                        if (DoesLineIntersectRectangle(segment, rect))
                        {
                            collision = true;
                            break;
                        }
                    }

                    if (collision) break;
                }

                if (!collision)
                {
                    result.Add(segment);
                }
            }

            return result;
        }

        /// <summary>
        /// Determines if a line segment intersects with a rectangle
        /// </summary>
        private bool DoesLineIntersectRectangle(PathSegment segment, Rectangle rect)
        {
            // Check if either endpoint is inside the rectangle
            if (IsPointInRectangle(segment.Start, rect) ||
                IsPointInRectangle(segment.End, rect))
            {
                return true;
            }

            // Check if the line intersects any of the rectangle's edges
            var rectSegments = new List<PathSegment>
            {
                new PathSegment(
                    new PointF((float)rect.X, (float)rect.Y),
                    new PointF((float)(rect.X + rect.Width), (float)rect.Y)
                ),
                new PathSegment(
                    new PointF((float)(rect.X + rect.Width), (float)rect.Y),
                    new PointF((float)(rect.X + rect.Width), (float)(rect.Y + rect.Height))
                ),
                new PathSegment(
                    new PointF((float)(rect.X + rect.Width), (float)(rect.Y + rect.Height)),
                    new PointF((float)rect.X, (float)(rect.Y + rect.Height))
                ),
                new PathSegment(
                    new PointF((float)rect.X, (float)(rect.Y + rect.Height)),
                    new PointF((float)rect.X, (float)rect.Y)
                )
            };

            foreach (var rectSegment in rectSegments)
            {
                if (DoSegmentsIntersect(segment, rectSegment))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines if a point is inside a rectangle
        /// </summary>
        private bool IsPointInRectangle(PointF point, Rectangle rect)
        {
            return point.X >= rect.X && point.X <= rect.X + rect.Width &&
                   point.Y >= rect.Y && point.Y <= rect.Y + rect.Height;
        }

        /// <summary>
        /// Determines if two line segments intersect
        /// </summary>
        private bool DoSegmentsIntersect(PathSegment segment1, PathSegment segment2)
        {
            double a1 = segment1.End.Y - segment1.Start.Y;
            double b1 = segment1.Start.X - segment1.End.X;
            double c1 = a1 * segment1.Start.X + b1 * segment1.Start.Y;

            double a2 = segment2.End.Y - segment2.Start.Y;
            double b2 = segment2.Start.X - segment2.End.X;
            double c2 = a2 * segment2.Start.X + b2 * segment2.Start.Y;

            double determinant = a1 * b2 - a2 * b1;

            if (determinant == 0)
            {
                // Lines are parallel
                return false;
            }

            double x = (b2 * c1 - b1 * c2) / determinant;
            double y = (a1 * c2 - a2 * c1) / determinant;

            // Check if intersection point is on both line segments
            return segment1.Contains(x, y) && segment2.Contains(x, y);
        }
    }

    /// <summary>
    /// Simple rectangle representation
    /// </summary>
    public class Rectangle
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }

        public Rectangle(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }
    }
}