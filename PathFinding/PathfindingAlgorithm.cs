using System;
using System.Collections.Generic;
using System.Drawing;
using PathFindingClassDiagram.Models;
using PathFindingClassDiagram.PathFinding.Models;
using Rectangle = PathFindingClassDiagram.PathFinding.Models.Rectangle;

namespace PathFindingClassDiagram.PathFinding
{
    public class PathfindingAlgorithm
    {
        private const int GridSize = 4;
        private const double NodeMargin = 25.0;
        private readonly CollisionDetection _collisionDetection;

        public PathfindingAlgorithm()
        {
            _collisionDetection = new CollisionDetection();
        }

        public List<PathPoint> FindPath(PathPoint start, PathPoint end, List<ClassDiagram> classDiagrams, Graphics graphics)
        {
            // Convert class diagrams to obstacles
            var obstacles = ConvertClassDiagramsToObstacles(classDiagrams, graphics);

            // If direct path doesn't collide with any obstacles, return it
            if (!_collisionDetection.CheckCollision(start, end, obstacles))
            {
                return new List<PathPoint> { start, end };
            }

            // Create a grid for pathfinding
            var (grid, nodeMap) = CreateGrid(start, end, obstacles);

            // Find the shortest path using Dijkstra's algorithm
            var path = DijkstraAlgorithm(grid, start, end, nodeMap);

            // Optimize the path
            return OptimizePath(path, obstacles);
        }

        private List<Rectangle> ConvertClassDiagramsToObstacles(List<ClassDiagram> classDiagrams, Graphics graphics)
        {
            var obstacles = new List<Rectangle>();

            foreach (var diagram in classDiagrams)
            {
                if (diagram.Points == null || diagram.Points.Count == 0)
                    continue;

                var firstPoint = diagram.Points[0];
                float width = firstPoint.TopRight.X - firstPoint.TopLeft.X;
                float height = firstPoint.BottomLeft.Y - firstPoint.TopLeft.Y;

                obstacles.Add(new Rectangle(
                    firstPoint.TopLeft.X - NodeMargin,
                    firstPoint.TopLeft.Y - NodeMargin,
                    width + (2 * NodeMargin),
                    height + (2 * NodeMargin)
                ));
            }

            return obstacles;
        }

        private (Dictionary<(int, int), Node> grid, Dictionary<(double, double), (int, int)> nodeMap) CreateGrid(
            PathPoint start, PathPoint end, List<Rectangle> obstacles)
        {
            // Find the bounding box for the grid
            double minX = Math.Min(start.X, end.X) - 100;
            double minY = Math.Min(start.Y, end.Y) - 100;
            double maxX = Math.Max(start.X, end.X) + 100;
            double maxY = Math.Max(start.Y, end.Y) + 100;

            // Ensure the grid covers all obstacles
            foreach (var obstacle in obstacles)
            {
                minX = Math.Min(minX, obstacle.X - 50);
                minY = Math.Min(minY, obstacle.Y - 50);
                maxX = Math.Max(maxX, obstacle.Right + 50);
                maxY = Math.Max(maxY, obstacle.Bottom + 50);
            }

            // Calculate grid dimensions
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

            // Create grid nodes
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

            // Define directions (orthogonal movement)
            int[] dx = { -1, 0, 1, 0 };
            int[] dy = { 0, 1, 0, -1 };

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
                for (int i = 0; i < 4; i++)
                {
                    int nx = x + dx[i];
                    int ny = y + dy[i];

                    if (grid.TryGetValue((nx, ny), out Node neighbor) && !neighbor.Visited)
                    {
                        double distance = current.Distance +
                            Math.Sqrt(Math.Pow(current.X - neighbor.X, 2) + Math.Pow(current.Y - neighbor.Y, 2));

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
            else // No path found, use direct path
            {
                path.Add(start);
                path.Add(end);
            }

            return path;
        }

        private List<PathPoint> OptimizePath(List<PathPoint> path, List<Rectangle> obstacles)
        {
            if (path.Count <= 2)
                return path;

            // Smooth the path by removing unnecessary points
            var optimizedPath = new List<PathPoint> { path[0] };

            for (int i = 1; i < path.Count - 1; i++)
            {
                // Check if we can skip this point
                if (!_collisionDetection.CheckCollision(optimizedPath[optimizedPath.Count - 1], path[i + 1], obstacles))
                {
                    // We can skip this point
                    continue;
                }

                optimizedPath.Add(path[i]);
            }

            optimizedPath.Add(path[path.Count - 1]);

            // Additional smoothing to make the path more orthogonal
            return CreateOrthogonalPath(optimizedPath);
        }

        private List<PathPoint> CreateOrthogonalPath(List<PathPoint> path)
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
                orthogonalPath.Add(new PathPoint(next.X, current.Y));
                orthogonalPath.Add(next);
            }

            return orthogonalPath;
        }
    }
}
