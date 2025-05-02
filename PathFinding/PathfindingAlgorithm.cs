using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using PathFindingClassDiagram.Models;
using PathFindingClassDiagram.PathFinding.Models;
using Rectangle = PathFindingClassDiagram.PathFinding.Models.Rectangle;

namespace PathFindingClassDiagram.PathFinding
{
    public class PathfindingAlgorithm
    {
        // Increase grid resolution for more precise paths
        private const int GridSize = 5; // Smaller grid size for finer resolution

        // Increase margin around classes to ensure paths stay far from them
        private const double NodeMargin = 40.0; // Larger margin around class boxes

        private readonly CollisionDetection _collisionDetection;

        public PathfindingAlgorithm()
        {
            _collisionDetection = new CollisionDetection();
        }

        public List<PathPoint> FindPath(PathPoint start, PathPoint end, List<ClassDiagram> classDiagrams, Graphics graphics)
        {
            // Convert class diagrams to obstacles with larger margins
            var obstacles = ConvertClassDiagramsToObstacles(classDiagrams, graphics);

            // If direct path doesn't collide with any obstacles, return it
            if (!_collisionDetection.CheckCollision(start, end, obstacles))
            {
                return new List<PathPoint> { start, end };
            }

            // Create a grid for pathfinding with finer resolution
            var (grid, nodeMap) = CreateGrid(start, end, obstacles);

            // Find the shortest path using Dijkstra's algorithm
            var path = DijkstraAlgorithm(grid, start, end, nodeMap);

            // Optimize the path 
            path = OptimizePath(path, obstacles);

            // Add final validation to ensure no segment crosses an obstacle
            path = ValidatePath(path, obstacles);

            return path;
        }

        private List<Rectangle> ConvertClassDiagramsToObstacles(List<ClassDiagram> classDiagrams, Graphics graphics)
        {
            var obstacles = new List<Rectangle>();

            foreach (var diagram in classDiagrams)
            {
                if (diagram.Points == null || diagram.Points.Count == 0)
                    continue;

                // Compute a more accurate bounding box
                var firstPoint = diagram.Points[0];
                float minX = firstPoint.TopLeft.X;
                float minY = firstPoint.TopLeft.Y;
                float maxX = firstPoint.TopRight.X;
                float maxY = firstPoint.BottomLeft.Y;

                // Add margin around the class diagram
                obstacles.Add(new Rectangle(
                    minX - NodeMargin,
                    minY - NodeMargin,
                    (maxX - minX) + (2 * NodeMargin),
                    (maxY - minY) + (2 * NodeMargin)
                ));
            }

            return obstacles;
        }

        private (Dictionary<(int, int), Node> grid, Dictionary<(double, double), (int, int)> nodeMap) CreateGrid(
            PathPoint start, PathPoint end, List<Rectangle> obstacles)
        {
            // Find the bounding box for the grid, with more padding
            double minX = Math.Min(start.X, end.X) - 150; // More padding
            double minY = Math.Min(start.Y, end.Y) - 150; // More padding
            double maxX = Math.Max(start.X, end.X) + 150; // More padding
            double maxY = Math.Max(start.Y, end.Y) + 150; // More padding

            // Ensure the grid covers all obstacles with padding
            foreach (var obstacle in obstacles)
            {
                minX = Math.Min(minX, obstacle.X - 100);
                minY = Math.Min(minY, obstacle.Y - 100);
                maxX = Math.Max(maxX, obstacle.Right + 100);
                maxY = Math.Max(maxY, obstacle.Bottom + 100);
            }

            // Calculate grid dimensions with finer grid
            int gridWidth = (int)((maxX - minX) / GridSize) + 1;
            int gridHeight = (int)((maxY - minY) / GridSize) + 1;

            // Create the grid and mapping
            var grid = new Dictionary<(int, int), Node>();
            var nodeMap = new Dictionary<(double, double), (int, int)>();

            // Add start and end nodes to the grid
            int startX = (int)((start.X - minX) / GridSize);
            int startY = (int)((start.Y - minY) / GridSize);
            int endX = (int)((end.X - minX) / GridSize);
            int endY = (int)((end.Y - minY) / GridSize);

            // Create grid nodes - skip nodes inside obstacles
            for (int x = 0; x < gridWidth; x++)
            {
                for (int y = 0; y < gridHeight; y++)
                {
                    double realX = minX + (x * GridSize);
                    double realY = minY + (y * GridSize);

                    // Check if this point is inside an obstacle
                    bool isObstacle = false;
                    foreach (var obstacle in obstacles)
                    {
                        if (obstacle.Contains(new PathPoint(realX, realY)))
                        {
                            isObstacle = true;
                            break;
                        }
                    }

                    if (!isObstacle)
                    {
                        var node = new Node(realX, realY);
                        grid[(x, y)] = node;
                        nodeMap[(realX, realY)] = (x, y);
                    }
                }
            }

            // Ensure start and end points are in the grid
            if (!grid.ContainsKey((startX, startY)))
            {
                var node = new Node(start.X, start.Y);
                grid[(startX, startY)] = node;
                nodeMap[(start.X, start.Y)] = (startX, startY);
            }

            if (!grid.ContainsKey((endX, endY)))
            {
                var node = new Node(end.X, end.Y);
                grid[(endX, endY)] = node;
                nodeMap[(end.X, end.Y)] = (endX, endY);
            }

            return (grid, nodeMap);
        }

        private List<PathPoint> DijkstraAlgorithm(
            Dictionary<(int, int), Node> grid,
            PathPoint start,
            PathPoint end,
            Dictionary<(double, double), (int, int)> nodeMap)
        {
            // Map start and end to grid coordinates
            (int startX, int startY) = nodeMap[(start.X, start.Y)];
            (int endX, int endY) = nodeMap[(end.X, end.Y)];

            // Initialize Dijkstra
            var queue = new PriorityQueue<Node>();
            var startNode = grid[(startX, startY)];
            startNode.Distance = 0;
            queue.Enqueue(startNode);

            // Define directions (for fine-grained control, include diagonal movements)
            int[] dx = { -1, 0, 1, 0, -1, -1, 1, 1 };
            int[] dy = { 0, 1, 0, -1, -1, 1, -1, 1 };

            // Dijkstra's algorithm
            while (queue.Count > 0)
            {
                var current = queue.Dequeue();

                if (current.Visited)
                    continue;

                current.Visited = true;

                // Get grid coordinates for current node
                (int x, int y) = nodeMap[(current.X, current.Y)];

                // If we reached the end node
                if (x == endX && y == endY)
                    break;

                // Check all directions
                for (int i = 0; i < dx.Length; i++)
                {
                    int nx = x + dx[i];
                    int ny = y + dy[i];

                    if (grid.TryGetValue((nx, ny), out Node neighbor) && !neighbor.Visited)
                    {
                        // Diagonal movements cost more (sqrt(2) ≈ 1.414)
                        double moveCost = (i < 4) ? 1.0 : 1.414;
                        double distance = current.Distance +
                            moveCost * Math.Sqrt(Math.Pow(current.X - neighbor.X, 2) +
                                               Math.Pow(current.Y - neighbor.Y, 2));

                        if (distance < neighbor.Distance)
                        {
                            neighbor.Distance = distance;
                            neighbor.Previous = current;
                            queue.Enqueue(neighbor);
                        }
                    }
                }
            }

            // Reconstruct path
            var path = new List<PathPoint>();
            var endNode = grid[(endX, endY)];

            if (endNode.Previous != null) // Path found
            {
                for (Node node = endNode; node != null; node = node.Previous)
                {
                    path.Add(new PathPoint(node.X, node.Y));
                }

                path.Reverse();
            }
            else // No path found, use direct path with extra points to avoid obstacles
            {
                // Create a path that goes around obstacles
                path = CreateAlternativePath(start, end, grid.Values.ToList());
            }

            return path;
        }

        private List<PathPoint> CreateAlternativePath(PathPoint start, PathPoint end, List<Node> allNodes)
        {
            // Try to find some intermediate points to go around obstacles
            var path = new List<PathPoint>();
            path.Add(start);

            // Find the "center" of all nodes as an intermediate point
            double avgX = allNodes.Average(n => n.X);
            double avgY = allNodes.Average(n => n.Y);

            // Use four points to create a path that avoids obstacles
            path.Add(new PathPoint(start.X, avgY));
            path.Add(new PathPoint(avgX, avgY));
            path.Add(new PathPoint(end.X, avgY));
            path.Add(end);

            return path;
        }

        private List<PathPoint> OptimizePath(List<PathPoint> path, List<Rectangle> obstacles)
        {
            if (path.Count <= 2)
                return path;

            // First, reduce path points by removing unnecessary zigzags
            path = ReducePathPoints(path);

            // Make the path orthogonal (horizontal and vertical segments only)
            return CreateOrthogonalPath(path, obstacles);
        }

        private List<PathPoint> ReducePathPoints(List<PathPoint> path)
        {
            if (path.Count <= 3)
                return path;

            var result = new List<PathPoint> { path[0] };
            for (int i = 1; i < path.Count - 1; i++)
            {
                var prev = path[i - 1];
                var curr = path[i];
                var next = path[i + 1];

                // If the point is not in a straight line, keep it
                if (!IsInStraightLine(prev, curr, next))
                {
                    result.Add(curr);
                }
            }
            result.Add(path[path.Count - 1]);

            return result;
        }

        private bool IsInStraightLine(PathPoint a, PathPoint b, PathPoint c)
        {
            // Check if three points are approximately in a straight line
            double crossProduct = Math.Abs((b.Y - a.Y) * (c.X - b.X) - (b.X - a.X) * (c.Y - b.Y));
            return crossProduct < 0.0001; // Allow for floating-point error
        }

        private List<PathPoint> CreateOrthogonalPath(List<PathPoint> path, List<Rectangle> obstacles)
        {
            if (path.Count <= 2)
                return path;

            var orthogonalPath = new List<PathPoint> { path[0] };

            for (int i = 0; i < path.Count - 1; i++)
            {
                var current = path[i];
                var next = path[i + 1];

                // If the line is already horizontal or vertical, just add the next point
                if (Math.Abs(current.X - next.X) < 0.1 || Math.Abs(current.Y - next.Y) < 0.1)
                {
                    if (i == path.Count - 2)
                        orthogonalPath.Add(next);
                    continue;
                }

                // Add corner points to make the line orthogonal
                var cornerPoint = new PathPoint(next.X, current.Y);

                // Check if the corner point and horizontal segment avoid obstacles
                bool segmentClear = !_collisionDetection.CheckCollision(current, cornerPoint, obstacles);
                if (segmentClear)
                {
                    orthogonalPath.Add(cornerPoint);
                }
                else
                {
                    // Try vertical then horizontal
                    cornerPoint = new PathPoint(current.X, next.Y);
                    segmentClear = !_collisionDetection.CheckCollision(current, cornerPoint, obstacles);

                    if (segmentClear)
                    {
                        orthogonalPath.Add(cornerPoint);
                    }
                    else
                    {
                        // Try a two-segment path with a different intermediate point
                        double midX = (current.X + next.X) / 2;
                        PathPoint midPoint = new PathPoint(midX, current.Y);
                        PathPoint midPoint2 = new PathPoint(midX, next.Y);

                        if (!_collisionDetection.CheckCollision(current, midPoint, obstacles) &&
                            !_collisionDetection.CheckCollision(midPoint, midPoint2, obstacles) &&
                            !_collisionDetection.CheckCollision(midPoint2, next, obstacles))
                        {
                            orthogonalPath.Add(midPoint);
                            orthogonalPath.Add(midPoint2);
                        }
                        else
                        {
                            // If all else fails, try more intermediate points
                            orthogonalPath.Add(new PathPoint(current.X + (next.X - current.X) / 3, current.Y));
                            orthogonalPath.Add(new PathPoint(current.X + (next.X - current.X) / 3,
                                                          current.Y + (next.Y - current.Y) / 2));
                            orthogonalPath.Add(new PathPoint(current.X + 2 * (next.X - current.X) / 3,
                                                          current.Y + (next.Y - current.Y) / 2));
                            orthogonalPath.Add(new PathPoint(current.X + 2 * (next.X - current.X) / 3, next.Y));
                        }
                    }
                }

                orthogonalPath.Add(next);
            }

            return orthogonalPath;
        }

        private List<PathPoint> ValidatePath(List<PathPoint> path, List<Rectangle> obstacles)
        {
            if (path.Count <= 2)
                return path;

            var validatedPath = new List<PathPoint> { path[0] };

            for (int i = 0; i < path.Count - 1; i++)
            {
                var currentPoint = path[i];
                var nextPoint = path[i + 1];

                // Check if this segment crosses any obstacle
                bool segmentCrossesObstacle = false;
                foreach (var obstacle in obstacles)
                {
                    if (_collisionDetection.CheckCollision(currentPoint, nextPoint, new List<Rectangle> { obstacle }))
                    {
                        segmentCrossesObstacle = true;
                        break;
                    }
                }

                if (segmentCrossesObstacle)
                {
                    // Find intermediate points to go around the obstacle
                    // First try going horizontally then vertically
                    PathPoint intermediatePoint = new PathPoint(nextPoint.X, currentPoint.Y);
                    bool horizontalThenVertical = true;

                    foreach (var obstacle in obstacles)
                    {
                        if (_collisionDetection.CheckCollision(currentPoint, intermediatePoint, new List<Rectangle> { obstacle }) ||
                            _collisionDetection.CheckCollision(intermediatePoint, nextPoint, new List<Rectangle> { obstacle }))
                        {
                            horizontalThenVertical = false;
                            break;
                        }
                    }

                    if (horizontalThenVertical)
                    {
                        validatedPath.Add(intermediatePoint);
                    }
                    else
                    {
                        // Try vertically then horizontally
                        intermediatePoint = new PathPoint(currentPoint.X, nextPoint.Y);
                        bool verticalThenHorizontal = true;

                        foreach (var obstacle in obstacles)
                        {
                            if (_collisionDetection.CheckCollision(currentPoint, intermediatePoint, new List<Rectangle> { obstacle }) ||
                                _collisionDetection.CheckCollision(intermediatePoint, nextPoint, new List<Rectangle> { obstacle }))
                            {
                                verticalThenHorizontal = false;
                                break;
                            }
                        }

                        if (verticalThenHorizontal)
                        {
                            validatedPath.Add(intermediatePoint);
                        }
                        else
                        {
                            // Try a more complex path with multiple intermediate points
                            // Find a midpoint between the current and next points
                            double midX = (currentPoint.X + nextPoint.X) / 2;
                            double midY = (currentPoint.Y + nextPoint.Y) / 2;

                            // Try to find a path by going out perpendicular to the direct path
                            double dx = nextPoint.X - currentPoint.X;
                            double dy = nextPoint.Y - currentPoint.Y;
                            double length = Math.Sqrt(dx * dx + dy * dy);

                            // Normalize and get perpendicular vector
                            double perpX = -dy / length * 100; // 100 units perpendicular distance
                            double perpY = dx / length * 100;

                            // Create middle point that's offset perpendicular to the direct path
                            PathPoint detourPoint = new PathPoint(midX + perpX, midY + perpY);

                            // Check if this detour is clear
                            bool detourClear = true;
                            foreach (var obstacle in obstacles)
                            {
                                if (_collisionDetection.CheckCollision(currentPoint, detourPoint, new List<Rectangle> { obstacle }) ||
                                    _collisionDetection.CheckCollision(detourPoint, nextPoint, new List<Rectangle> { obstacle }))
                                {
                                    detourClear = false;
                                    break;
                                }
                            }

                            if (detourClear)
                            {
                                validatedPath.Add(detourPoint);
                            }
                            else
                            {
                                // Try the opposite direction
                                detourPoint = new PathPoint(midX - perpX, midY - perpY);
                                detourClear = true;

                                foreach (var obstacle in obstacles)
                                {
                                    if (_collisionDetection.CheckCollision(currentPoint, detourPoint, new List<Rectangle> { obstacle }) ||
                                        _collisionDetection.CheckCollision(detourPoint, nextPoint, new List<Rectangle> { obstacle }))
                                    {
                                        detourClear = false;
                                        break;
                                    }
                                }

                                if (detourClear)
                                {
                                    validatedPath.Add(detourPoint);
                                }
                                else
                                {
                                    // Last resort - add multiple points to navigate around obstacles
                                    // Go to nearest grid edge, go around, and then to destination
                                    // This is a complex case that would need more analysis of the specific obstacles
                                    validatedPath.Add(new PathPoint(currentPoint.X, currentPoint.Y - 100));
                                    validatedPath.Add(new PathPoint(nextPoint.X, currentPoint.Y - 100));
                                }
                            }
                        }
                    }
                }

                validatedPath.Add(nextPoint);
            }

            return validatedPath;
        }
    }
}
