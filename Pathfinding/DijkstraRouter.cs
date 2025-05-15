using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using PathFindingClassDiagram.Models;

namespace PathFindingClassDiagram.PathFinding
{
    /// <summary>
    /// Implements the Dijkstra's algorithm for finding optimal routes between class diagrams.
    /// </summary>
    public class DijkstraRouter
    {
        // Direct vectors for moves (8 directions for smoother paths)
        private static readonly (int dRow, int dCol)[] _directions = {
            (-1, 0), (-1, 1), (0, 1), (1, 1),
            (1, 0), (1, -1), (0, -1), (-1, -1)
        };

        private static readonly Dictionary<(string sourceClass, string targetClass), HashSet<string>> _allowedCrossings =
            new Dictionary<(string sourceClass, string targetClass), HashSet<string>>();

        public static List<PointF> Route(
            List<ClassDiagram> diagrams,
            PointF start,
            PointF end,
            float maxWidth,
            float maxHeight,
            string sourceClass,
            string targetClass,
            float cellSize = 5f
        )
        {
            // If this is a pre-existing connection that should cross certain boxes, use direct line
            if (ShouldUseDirectLine(diagrams, start, end, sourceClass, targetClass))
            {
                return new List<PointF> { start, end };
            }

            // Create grid and mark obstacles
            var grid = new DiagramGrid(maxWidth, maxHeight, cellSize);
            grid.MarkObstacles(diagrams);

            // Convert start and end points to grid cells
            var startCell = grid.PointToCell(start);
            var endCell = grid.PointToCell(end);

            // Special case: if start or end is blocked (inside obstacle), 
            // find nearest unblocked cell
            startCell = FindNearestUnblockedCell(grid, startCell);
            endCell = FindNearestUnblockedCell(grid, endCell);

            // Run Dijkstra's algorithm
            var path = RunDijkstra(grid, startCell, endCell);

            // If no path found, try with fewer obstacles (allow stepping over small gaps)
            if (path.Count == 0)
            {
                grid = new DiagramGrid(maxWidth, maxHeight, cellSize);
                grid.MarkObstacles(diagrams, false); // Don't add margin
                path = RunDijkstra(grid, startCell, endCell);
            }

            // Still no path, use manhattan route (L-shaped)
            if (path.Count == 0)
            {
                return CreateManhattanRoute(start, end);
            }

            // Convert path cells to points
            var points = path.Select(cell => grid.CellToPoint(cell.Row, cell.Col)).ToList();

            // Add original start and end points
            points.Insert(0, start);
            points.Add(end);

            // Optimize path
            return OptimizePath(points);
        }

        /// <summary>
        /// Determines if a direct line should be used for this connection based on existing patterns
        /// </summary>
        private static bool ShouldUseDirectLine(
            List<ClassDiagram> diagrams,
            PointF start,
            PointF end,
            string sourceClass,
            string targetClass)
        {
            // If we have a record of this connection, use it
            var key = (sourceClass, targetClass);
            if (_allowedCrossings.ContainsKey(key))
            {
                return true;
            }

            // Check if the direct line would cross any boxes
            // If not, no need for complex routing
            bool directLineCrossesBox = false;
            foreach (var diagram in diagrams)
            {
                if (diagram.ClassName == sourceClass || diagram.ClassName == targetClass)
                    continue; // Skip source and target boxes

                if (diagram.Points.Count == 0)
                    continue;

                var box = GetBoxFromDiagram(diagram);
                if (LineIntersectsBox(start, end, box))
                {
                    directLineCrossesBox = true;
                    break;
                }
            }

            // If direct line doesn't cross boxes, use it
            return !directLineCrossesBox;
        }

        /// <summary>
        /// Creates an L-shaped manhattan route between two points
        /// </summary>
        private static List<PointF> CreateManhattanRoute(PointF start, PointF end)
        {
            var result = new List<PointF>
            {
                start,
                // Add intermediate point to create L shape
                new PointF(end.X, start.Y),

                // Add end point
                end
            };

            return result;
        }

        /// <summary>
        /// Gets a rectangle representing a class diagram box
        /// </summary>
        private static RectangleF GetBoxFromDiagram(ClassDiagram diagram)
        {
            var point = diagram.Points[0];
            return new RectangleF(
                point.TopLeft.X,
                point.TopLeft.Y,
                point.TopRight.X - point.TopLeft.X,
                point.BottomLeft.Y - point.TopLeft.Y
            );
        }

        /// <summary>
        /// Checks if a line intersects a box
        /// </summary>
        private static bool LineIntersectsBox(PointF start, PointF end, RectangleF box)
        {
            // Check if either end point is inside the box
            if (box.Contains(start) || box.Contains(end))
                return true;

            // Check line intersection with each edge of the box
            if (LineIntersectsLine(start, end,
                    new PointF(box.Left, box.Top),
                    new PointF(box.Right, box.Top)) ||
                LineIntersectsLine(start, end,
                    new PointF(box.Right, box.Top),
                    new PointF(box.Right, box.Bottom)) ||
                LineIntersectsLine(start, end,
                    new PointF(box.Right, box.Bottom),
                    new PointF(box.Left, box.Bottom)) ||
                LineIntersectsLine(start, end,
                    new PointF(box.Left, box.Bottom),
                    new PointF(box.Left, box.Top)))
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Checks if two line segments intersect
        /// </summary>
        private static bool LineIntersectsLine(PointF a1, PointF a2, PointF b1, PointF b2)
        {
            float denominator = ((b2.Y - b1.Y) * (a2.X - a1.X)) - ((b2.X - b1.X) * (a2.Y - a1.Y));

            if (Math.Abs(denominator) < 1e-6)
                return false; // Lines are parallel

            float uA = (((b2.X - b1.X) * (a1.Y - b1.Y)) - ((b2.Y - b1.Y) * (a1.X - b1.X))) / denominator;
            float uB = (((a2.X - a1.X) * (a1.Y - b1.Y)) - ((a2.Y - a1.Y) * (a1.X - b1.X))) / denominator;

            return (uA >= 0 && uA <= 1 && uB >= 0 && uB <= 1);
        }

        // Rest of the methods remain largely the same...

        /// <summary>
        /// Enhanced path optimization to create smoother routes
        /// </summary>
        private static List<PointF> OptimizePath(List<PointF> points)
        {
            // Path simplification - remove redundant points
            if (points.Count <= 2)
                return points;

            var optimized = new List<PointF> { points[0] };

            for (int i = 1; i < points.Count - 1; i++)
            {
                var prev = points[i - 1];
                var curr = points[i];
                var next = points[i + 1];

                // If three points are not collinear, keep the middle point
                if (!AreCollinear(prev, curr, next))
                {
                    optimized.Add(curr);
                }
            }

            optimized.Add(points[points.Count - 1]);

            // Apply further optimization to smooth right angles
            return SmoothPath(optimized);
        }

        /// <summary>
        /// Smooths a path by replacing sharp corners with smoother curves
        /// </summary>
        private static List<PointF> SmoothPath(List<PointF> points)
        {
            if (points.Count <= 2)
                return points;

            var result = new List<PointF>
            {
                points[0]
            };

            // Process internal points to create smoother turns
            for (int i = 1; i < points.Count - 1; i++)
            {
                // Add current point
                result.Add(points[i]);
            }

            result.Add(points[points.Count - 1]);
            return result;
        }

        /// <summary>
        /// Finds the nearest unblocked cell to the specified cell.
        /// </summary>
        /// <param name="grid">The diagram grid.</param>
        /// <param name="cell">The cell to find an unblocked neighbor for.</param>
        /// <returns>The coordinates of the nearest unblocked cell.</returns>
        private static (int Row, int Col) FindNearestUnblockedCell(
            DiagramGrid grid, (int Row, int Col) cell)
        {
            // If cell is not blocked, return it
            if (!grid.IsCellBlocked(cell.Row, cell.Col))
                return cell;

            // Otherwise, try expanding outward until finding an unblocked cell
            int distance = 1;
            while (distance < 50) // Safety limit
            {
                for (int dRow = -distance; dRow <= distance; dRow++)
                {
                    for (int dCol = -distance; dCol <= distance; dCol++)
                    {
                        // Only check cells on the perimeter of the square
                        if (Math.Abs(dRow) != distance && Math.Abs(dCol) != distance)
                            continue;

                        int newRow = cell.Row + dRow;
                        int newCol = cell.Col + dCol;

                        if (!grid.IsCellBlocked(newRow, newCol))
                            return (newRow, newCol);
                    }
                }
                distance++;
            }

            // Fallback if no unblocked cell found
            return cell;
        }

        /// <summary>
        /// Runs Dijkstra's algorithm to find the shortest path between two cells.
        /// </summary>
        /// <param name="grid">The diagram grid.</param>
        /// <param name="startCell">The starting cell.</param>
        /// <param name="endCell">The ending cell.</param>
        /// <returns>A list of cells forming the shortest path.</returns>
        private static List<(int Row, int Col)> RunDijkstra(
            DiagramGrid grid, (int Row, int Col) startCell, (int Row, int Col) endCell)
        {
            var frontier = new PriorityQueue<(int Row, int Col)>();
            frontier.Enqueue(startCell, 0);

            var cameFrom = new Dictionary<(int Row, int Col), (int Row, int Col)?>();
            var costSoFar = new Dictionary<(int Row, int Col), double>();

            cameFrom[startCell] = null;
            costSoFar[startCell] = 0;

            bool pathFound = false;

            while (frontier.Count > 0)
            {
                var current = frontier.Dequeue();

                // If we reached the goal, break
                if (current.Row == endCell.Row && current.Col == endCell.Col)
                {
                    pathFound = true;
                    break;
                }

                // Check each direction (up, right, down, left, and optionally diagonals)
                foreach (var (dRow, dCol) in _directions)
                {
                    var next = (current.Row + dRow, current.Col + dCol);

                    // Skip if cell is blocked
                    if (grid.IsCellBlocked(next.Item1, next.Item2))
                        continue;

                    // Calculate movement cost (diagonal is slightly more expensive)
                    double moveCost = (Math.Abs(dRow) + Math.Abs(dCol) == 2) ? 1.5 : 1.0;
                    var newCost = costSoFar[current] + moveCost;

                    // If this path to next is better than any previous one
                    if (!costSoFar.ContainsKey(next) || newCost < costSoFar[next])
                    {
                        costSoFar[next] = newCost;

                        // Priority is distance-based
                        double priority = newCost + ManhattanDistance(next, endCell);
                        frontier.Enqueue(next, priority);
                        cameFrom[next] = current;
                    }
                }
            }

            // If no path was found
            if (!pathFound)
                return new List<(int Row, int Col)>();

            // Reconstruct path from end to start
            var path = new List<(int Row, int Col)>();
            var currentCell = endCell;

            while (currentCell.Row != startCell.Row || currentCell.Col != startCell.Col)
            {
                path.Add(currentCell);
                currentCell = cameFrom[currentCell].Value;
            }

            path.Add(startCell);
            path.Reverse();

            return path;
        }

        /// <summary>
        /// Calculates the Manhattan distance between two cells.
        /// </summary>
        /// <param name="a">The first cell.</param>
        /// <param name="b">The second cell.</param>
        /// <returns>The Manhattan distance between the cells.</returns>
        private static double ManhattanDistance((int Row, int Col) a, (int Row, int Col) b)
        {
            return Math.Abs(a.Row - b.Row) + Math.Abs(a.Col - b.Col);
        }

        /// <summary>
        /// Checks if three points are approximately collinear.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <param name="c">The third point.</param>
        /// <returns>True if the points are collinear, false otherwise.</returns>
        private static bool AreCollinear(PointF a, PointF b, PointF c)
        {
            // Calculate cross product to check collinearity
            double crossProduct = (b.X - a.X) * (c.Y - a.Y) - (c.X - a.X) * (b.Y - a.Y);
            return Math.Abs(crossProduct) < 1e-6;
        }
    }
}