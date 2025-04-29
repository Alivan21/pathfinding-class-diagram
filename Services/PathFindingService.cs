using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using PathFindingClassDiagram.Models;
using PathFindingClassDiagram.Services.Interfaces;

namespace PathFindingClassDiagram.Services
{
    public class PathFindingService : IPathFindingService
    {
        private readonly CollisionDetection _collisionDetection;
        private List<PathSegment> _connections;

        public double Margin { get; set; } = 10;

        public PathFindingService()
        {
            _collisionDetection = new CollisionDetection();
            _connections = new List<PathSegment>();
        }

        public List<PathSegment> CalculateRelationshipPaths(List<ClassDiagram> classDiagrams)
        {
            if (classDiagrams == null || classDiagrams.Count <= 1)
                return new List<PathSegment>();

            // Calculate canvas size based on class diagrams
            double maxWidth = 2000;
            double maxHeight = 1200;

            // Create lead lines for pathfinding
            _connections = CreateLeadLines(classDiagrams, maxWidth, maxHeight);

            // Find intersection points between lead lines
            var intersections = FindIntersections(_connections);

            // Build graph from lead lines and intersections
            ConstructGraph(intersections);

            // Calculate paths for all relationships
            var paths = new List<PathSegment>();
            foreach (var sourceDiagram in classDiagrams)
            {
                if (sourceDiagram.Relationships == null)
                    continue;

                foreach (var relationship in sourceDiagram.Relationships)
                {
                    var targetDiagram = classDiagrams.FirstOrDefault(d => d.ClassName == relationship.TargetClass);
                    if (targetDiagram != null)
                    {
                        var connector = CreateConnector(sourceDiagram, targetDiagram);
                        var path = CalculatePathForConnector(connector);
                        if (path.Any())
                        {
                            paths.AddRange(path);
                        }
                    }
                }
            }

            return paths;
        }

        private Connector CreateConnector(ClassDiagram source, ClassDiagram target)
        {
            return new Connector
            {
                Source = new ClassInputAdapter(source),
                Destination = new ClassInputAdapter(target),
                SourceOrientation = DetermineOrientation(source, target),
                DestinationOrientation = DetermineOrientation(target, source)
            };
        }

        private ConnectorOrientation DetermineOrientation(ClassDiagram source, ClassDiagram target)
        {
            // Determine the best orientation based on relative positions
            double sourceX = source.Points.FirstOrDefault()?.TopLeft.X ?? 0;
            double sourceY = source.Points.FirstOrDefault()?.TopLeft.Y ?? 0;
            double targetX = target.Points.FirstOrDefault()?.TopLeft.X ?? 0;
            double targetY = target.Points.FirstOrDefault()?.TopLeft.Y ?? 0;

            double dx = targetX - sourceX;
            double dy = targetY - sourceY;

            if (Math.Abs(dx) > Math.Abs(dy))
            {
                return dx > 0 ? ConnectorOrientation.Right : ConnectorOrientation.Left;
            }
            else
            {
                return dy > 0 ? ConnectorOrientation.Bottom : ConnectorOrientation.Top;
            }
        }

        private List<PathSegment> CalculatePathForConnector(Connector connector)
        {
            var sourceNode = ConvertToNode(connector.Source, connector.SourceOrientation);
            var destinationNode = ConvertToNode(connector.Destination, connector.DestinationOrientation);

            var shortestPath = FindShortestPath(sourceNode, destinationNode);
            var pathSegments = new List<PathSegment>();

            if (shortestPath != null && shortestPath.Count > 0)
            {
                for (int i = 0; i < shortestPath.Count - 1; i++)
                {
                    pathSegments.Add(new PathSegment
                    {
                        Start = new PointF((float)shortestPath[i].X, (float)shortestPath[i].Y),
                        End = new PointF((float)shortestPath[i + 1].X, (float)shortestPath[i + 1].Y)
                    });
                }
            }
            else
            {
                // Fallback to direct connection if pathfinding fails
                pathSegments.Add(new PathSegment
                {
                    Start = new PointF((float)sourceNode.X, (float)sourceNode.Y),
                    End = new PointF((float)destinationNode.X, (float)destinationNode.Y)
                });
            }

            return pathSegments;
        }

        private Node ConvertToNode(IInput input, ConnectorOrientation orientation)
        {
            switch (orientation)
            {
                case ConnectorOrientation.Left:
                    return new Node(input.X, input.Y + input.Height / 2);
                case ConnectorOrientation.Right:
                    return new Node(input.X + input.Width, input.Y + input.Height / 2);
                case ConnectorOrientation.Top:
                    return new Node(input.X + input.Width / 2, input.Y);
                case ConnectorOrientation.Bottom:
                    return new Node(input.X + input.Width / 2, input.Y + input.Height);
                default:
                    return new Node(input.X + input.Width / 2, input.Y + input.Height / 2);
            }
        }

        private List<Point> FindShortestPath(Node start, Node end)
        {
            // Simple A* pathfinding implementation
            // For a complete implementation, we would use the OrthogonalConnectorRouting DijkstraAlgorithm 
            // or AStarAlgorithm directly

            // Placeholder for now - returns direct path
            return new List<Point>
            {
                new Point((int)start.X, (int)start.Y),
                new Point((int)end.X, (int)end.Y)
            };
        }

        private void ConstructGraph(List<Point> intersections)
        {
            // Placeholder for graph construction
            // In a full implementation, this would build a graph structure for pathfinding
        }

        private List<Point> FindIntersections(List<PathSegment> connections)
        {
            var intersections = new List<Point>();
            for (int i = 0; i < connections.Count; i++)
            {
                for (int j = i + 1; j < connections.Count; j++)
                {
                    var intersection = FindIntersection(
                        new Point((int)connections[i].Start.X, (int)connections[i].Start.Y),
                        new Point((int)connections[i].End.X, (int)connections[i].End.Y),
                        new Point((int)connections[j].Start.X, (int)connections[j].Start.Y),
                        new Point((int)connections[j].End.X, (int)connections[j].End.Y));

                    if (intersection.HasValue)
                    {
                        intersections.Add(intersection.Value);
                    }
                }
            }
            return intersections;
        }

        private Point? FindIntersection(Point a1, Point a2, Point b1, Point b2)
        {
            double A1 = a2.Y - a1.Y;
            double B1 = a1.X - a2.X;
            double C1 = A1 * a1.X + B1 * a1.Y;

            double A2 = b2.Y - b1.Y;
            double B2 = b1.X - b2.X;
            double C2 = A2 * b1.X + B2 * b1.Y;

            double determinant = A1 * B2 - A2 * B1;

            if (Math.Abs(determinant) < 0.0001)
                return null; // Lines are parallel

            double x = (B2 * C1 - B1 * C2) / determinant;
            double y = (A1 * C2 - A2 * C1) / determinant;

            // Check if intersection is on both line segments
            if (IsPointOnSegment(a1, a2, x, y) && IsPointOnSegment(b1, b2, x, y))
                return new Point((int)Math.Round(x), (int)Math.Round(y));

            return null;
        }

        private bool IsPointOnSegment(Point p1, Point p2, double x, double y)
        {
            return (x >= Math.Min(p1.X, p2.X) && x <= Math.Max(p1.X, p2.X) &&
                    y >= Math.Min(p1.Y, p2.Y) && y <= Math.Max(p1.Y, p2.Y));
        }

        private List<PathSegment> CreateLeadLines(List<ClassDiagram> items, double maxWidth, double maxHeight)
        {
            var connections = new List<PathSegment>();

            // In a full implementation, this would create horizontal and vertical lines
            // from each side of each class diagram, detecting collisions with other diagrams

            // For now, we'll create a simple grid of lines
            foreach (var item in items)
            {
                foreach (var point in item.Points)
                {
                    // Horizontal lines from left and right centers
                    connections.Add(new PathSegment
                    {
                        Start = new PointF(0, point.LeftCenter.Y),
                        End = new PointF(point.LeftCenter.X, point.LeftCenter.Y)
                    });

                    connections.Add(new PathSegment
                    {
                        Start = new PointF(point.RightCenter.X, point.RightCenter.Y),
                        End = new PointF((float)maxWidth, point.RightCenter.Y)
                    });

                    // Vertical lines from top and bottom centers
                    connections.Add(new PathSegment
                    {
                        Start = new PointF(point.TopCenter.X, 0),
                        End = new PointF(point.TopCenter.X, point.TopCenter.Y)
                    });

                    connections.Add(new PathSegment
                    {
                        Start = new PointF(point.BottomCenter.X, point.BottomCenter.Y),
                        End = new PointF(point.BottomCenter.X, (float)maxHeight)
                    });
                }
            }

            return connections;
        }
    }
}