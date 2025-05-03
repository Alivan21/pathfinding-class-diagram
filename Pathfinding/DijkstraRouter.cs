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

        private static Dictionary<(string sourceClass, string targetClass), HashSet<string>> _allowedCrossings =
            new Dictionary<(string sourceClass, string targetClass), HashSet<string>>();

        /// <summary>
        /// Calculate a route between two points, avoiding class diagram obstacles.
        /// </summary>
        /// <param name="diagrams">The list of class diagrams that act as obstacles.</param>
        /// <param name="start">The starting point of the connector.</param>
        /// <param name="end">The ending point of the connector.</param>
        /// <param name="maxWidth">The maximum width of the diagram area.</param>
        /// <param name="maxHeight">The maximum height of the diagram area.</param>
        /// <param name="cellSize">The size of each grid cell. Smaller values provide more detailed routing.</param>
        /// <returns>A list of points forming the connector path.</returns>
        public static List<PointF> Route(
            List<ClassDiagram> diagrams,
            PointF start,
            PointF end,
            float maxWidth,
            float maxHeight,
            float cellSize = 10f)
        {
            // Create grid and mark obstacles
            var grid = new DiagramGrid(maxWidth, maxHeight, cellSize);
            grid.MarkObstacles(diagrams);
            grid.AddMarginToObstacles(1); // Add a small buffer around obstacles

            // Convert start and end points to grid cells
            var startCell = grid.PointToCell(start);
            var endCell = grid.PointToCell(end);

            // Special case: if start or end is blocked (inside obstacle), 
            // find nearest unblocked cell
            startCell = FindNearestUnblockedCell(grid, startCell);
            endCell = FindNearestUnblockedCell(grid, endCell);

            // Run Dijkstra's algorithm
            var path = RunDijkstra(grid, startCell, endCell);

            // No path found, return direct line
            if (path.Count == 0)
            {
                return new List<PointF> { start, end };
            }

            // Convert path cells to points
            var points = path.Select(cell => grid.CellToPoint(cell.Row, cell.Col)).ToList();

            // Add original start and end points to create complete path
            points.Insert(0, start);
            points.Add(end);

            // Optimize path by removing unnecessary points
            return OptimizePath(points);
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
                    double moveCost = (Math.Abs(dRow) + Math.Abs(dCol) == 2) ? 1.414 : 1.0;
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
        /// Optimizes the path by removing unnecessary points that are collinear.
        /// </summary>
        /// <param name="points">The list of points forming the path.</param>
        /// <returns>An optimized list of points.</returns>
        private static List<PointF> OptimizePath(List<PointF> points)
        {
            // Path simplification - remove collinear points
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

            // Secondary optimization: try to eliminate 90-degree bends when possible
            return OptimizeRightAngles(optimized);
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

        /// <summary>
        /// Optimizes right angles in the path to create fewer bends.
        /// </summary>
        /// <param name="points">The list of points forming the path.</param>
        /// <returns>An optimized list of points with fewer right angles.</returns>
        private static List<PointF> OptimizeRightAngles(List<PointF> points)
        {
            if (points.Count <= 3)
                return points;

            var result = new List<PointF>(points);

            for (int i = 0; i < result.Count - 3; i++)
            {
                // Only optimize certain patterns of right angles
                if (IsHorizontalOrVertical(result[i], result[i + 1]) &&
                    IsHorizontalOrVertical(result[i + 1], result[i + 2]) &&
                    IsHorizontalOrVertical(result[i + 2], result[i + 3]))
                {
                    // Check if we can simplify this pattern
                    bool horizontal1 = Math.Abs(result[i].Y - result[i + 1].Y) < 1e-6;
                    bool horizontal2 = Math.Abs(result[i + 1].Y - result[i + 2].Y) < 1e-6;
                    bool horizontal3 = Math.Abs(result[i + 2].Y - result[i + 3].Y) < 1e-6;

                    // If this forms a zigzag with one segment in the middle
                    if ((horizontal1 && !horizontal2 && horizontal3) ||
                        (!horizontal1 && horizontal2 && !horizontal3))
                    {
                        // Check if we can eliminate the two middle points
                        result.RemoveRange(i + 1, 2);

                        // Re-process this segment to catch further optimizations
                        i--;
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Checks if the line segment between two points is horizontal or vertical.
        /// </summary>
        /// <param name="a">The first point.</param>
        /// <param name="b">The second point.</param>
        /// <returns>True if the segment is horizontal or vertical, false otherwise.</returns>
        private static bool IsHorizontalOrVertical(PointF a, PointF b)
        {
            return Math.Abs(a.X - b.X) < 1e-6 || Math.Abs(a.Y - b.Y) < 1e-6;
        }
    }
}