using System;
using System.Collections.Generic;

namespace PathFindingClassDiagram.PathFinding.Models
{
    public class CollisionDetection
    {
        /// <summary>
        /// Checks if a line segment from start to end collides with any of the obstacles
        /// </summary>
        public bool CheckCollision(PathPoint start, PathPoint end, List<Rectangle> obstacles)
        {
            foreach (var obstacle in obstacles)
            {
                // Skip if both points are inside the same obstacle (this allows connections inside a class)
                if (obstacle.Contains(start) && obstacle.Contains(end))
                    continue;

                // Check if either endpoint is inside the obstacle but not both
                if ((obstacle.Contains(start) && !obstacle.Contains(end)) ||
                    (!obstacle.Contains(start) && obstacle.Contains(end)))
                    return true;

                // Check if the line segment intersects any of the rectangle's edges
                if (LineIntersectsRectangle(start, end, obstacle))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if a line segment intersects a rectangle
        /// </summary>
        private bool LineIntersectsRectangle(PathPoint start, PathPoint end, Rectangle rect)
        {
            // Create rectangle corner points
            PathPoint topLeft = new PathPoint(rect.X, rect.Y);
            PathPoint topRight = new PathPoint(rect.Right, rect.Y);
            PathPoint bottomLeft = new PathPoint(rect.X, rect.Bottom);
            PathPoint bottomRight = new PathPoint(rect.Right, rect.Bottom);

            // Check if the line intersects any of the rectangle's sides
            return LineIntersectsLine(start, end, topLeft, topRight) ||    // Top edge
                   LineIntersectsLine(start, end, topRight, bottomRight) || // Right edge
                   LineIntersectsLine(start, end, bottomRight, bottomLeft) || // Bottom edge
                   LineIntersectsLine(start, end, bottomLeft, topLeft);    // Left edge
        }

        /// <summary>
        /// Checks if two line segments intersect
        /// </summary>
        private bool LineIntersectsLine(PathPoint a1, PathPoint a2, PathPoint b1, PathPoint b2)
        {
            // Using the algorithm described in:
            // https://en.wikipedia.org/wiki/Line–line_intersection

            double x1 = a1.X;
            double y1 = a1.Y;
            double x2 = a2.X;
            double y2 = a2.Y;
            double x3 = b1.X;
            double y3 = b1.Y;
            double x4 = b2.X;
            double y4 = b2.Y;

            // Calculate the denominator
            double denominator = ((y4 - y3) * (x2 - x1)) - ((x4 - x3) * (y2 - y1));

            // Lines are parallel or coincident
            if (Math.Abs(denominator) < 0.0001)
            {
                // Check if they are collinear
                double d1 = ((y3 - y1) * (x2 - x1)) - ((x3 - x1) * (y2 - y1));
                if (Math.Abs(d1) < 0.0001)
                {
                    // Lines are collinear, check for overlap
                    return RangesOverlap(x1, x2, x3, x4) && RangesOverlap(y1, y2, y3, y4);
                }

                return false; // Parallel but not collinear
            }

            // Calculate ua and ub
            double ua = (((x4 - x3) * (y1 - y3)) - ((y4 - y3) * (x1 - x3))) / denominator;
            double ub = (((x2 - x1) * (y1 - y3)) - ((y2 - y1) * (x1 - x3))) / denominator;

            // Check if intersection is within both line segments
            return (ua >= 0 && ua <= 1) && (ub >= 0 && ub <= 1);
        }

        /// <summary>
        /// Checks if two ranges overlap
        /// </summary>
        private bool RangesOverlap(double a1, double a2, double b1, double b2)
        {
            // Ensure a1 <= a2 and b1 <= b2
            if (a1 > a2)
            {
                double temp = a1;
                a1 = a2;
                a2 = temp;
            }

            if (b1 > b2)
            {
                double temp = b1;
                b1 = b2;
                b2 = temp;
            }

            // Check for overlap
            return Math.Max(a1, b1) <= Math.Min(a2, b2);
        }

        /// <summary>
        /// Calculates the distance from a point to a line segment
        /// </summary>
        public double DistanceToLineSegment(PathPoint point, PathPoint lineStart, PathPoint lineEnd)
        {
            double lineLength = Math.Sqrt(
                Math.Pow(lineEnd.X - lineStart.X, 2) +
                Math.Pow(lineEnd.Y - lineStart.Y, 2));

            if (lineLength < 0.0001)
                return Math.Sqrt(
                    Math.Pow(point.X - lineStart.X, 2) +
                    Math.Pow(point.Y - lineStart.Y, 2));

            // Calculate projection of point onto line
            double t = ((point.X - lineStart.X) * (lineEnd.X - lineStart.X) +
                       (point.Y - lineStart.Y) * (lineEnd.Y - lineStart.Y)) /
                      (lineLength * lineLength);

            t = Math.Max(0, Math.Min(1, t)); // Clamp t to [0,1]

            // Calculate the closest point on the line segment
            double closestX = lineStart.X + t * (lineEnd.X - lineStart.X);
            double closestY = lineStart.Y + t * (lineEnd.Y - lineStart.Y);

            // Return the distance from the point to the closest point on the line segment
            return Math.Sqrt(
                Math.Pow(point.X - closestX, 2) +
                Math.Pow(point.Y - closestY, 2));
        }
    }
}
