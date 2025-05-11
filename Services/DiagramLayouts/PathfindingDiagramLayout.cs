using System;
using System.Collections.Generic;
using System.Drawing;
using PathFindingClassDiagram.Models;
using PathFindingClassDiagram.PathFinding;
using PathFindingClassDiagram.Services.Interfaces;

namespace PathFindingClassDiagram.Services.DiagramLayouts
{
    public class PathfindingDiagramLayout : IDiagramLayoutStrategy
    {
        public void DrawRelationships(Graphics g, List<ClassDiagram> classDiagrams,
                                     List<Relationship> relationships, int bitmapWidth, int bitmapHeight)
        {
            // Extract the pathfinding-based relationship drawing code from DiagramService
            Dictionary<string, ClassDiagram> classDiagramDictionary = new Dictionary<string, ClassDiagram>();
            foreach (ClassDiagram classDiagram in classDiagrams)
            {
                if (!string.IsNullOrEmpty(classDiagram.ClassName))
                    classDiagramDictionary[classDiagram.ClassName] = classDiagram;
            }

            foreach (Relationship relationship in relationships)
            {
                if (classDiagramDictionary.TryGetValue(relationship.SourceClass, out var sourceClassDiagram) &&
                    classDiagramDictionary.TryGetValue(relationship.TargetClass, out var targetClassDiagram))
                {
                    // Find the best connection points and snap to edges
                    (PointF source, PointF target) = GetClosestSnappedPoints(sourceClassDiagram, targetClassDiagram);

                    // Route the path using Dijkstra's algorithm
                    List<PointF> routePoints = DijkstraRouter.Route(
                        classDiagrams,
                        source,
                        target,
                        bitmapWidth,
                        bitmapHeight,
                        relationship.SourceClass,
                        relationship.TargetClass,
                        5f);

                    // Draw the path with appropriate styling
                    DrawConnectorPath(g, relationship.Type, routePoints);
                }
            }
        }

        // Include all the supporting methods from the pathfinding implementation
        private (PointF, PointF) GetClosestSnappedPoints(ClassDiagram source, ClassDiagram target)
        {
            // First find the closest points between diagrams
            (PointF sourcePoint, PointF targetPoint) = GetClosestPoints(source, target);

            // Snap source point to nearest edge
            sourcePoint = SnapToNearestEdge(source, sourcePoint);

            // Snap target point to nearest edge
            targetPoint = SnapToNearestEdge(target, targetPoint);

            return (sourcePoint, targetPoint);
        }

        private PointF SnapToNearestEdge(ClassDiagram diagram, PointF point)
        {
            if (diagram.Points.Count == 0)
                return point;

            var diagramPoint = diagram.Points[0];
            var box = new RectangleF(
                diagramPoint.TopLeft.X,
                diagramPoint.TopLeft.Y,
                diagramPoint.TopRight.X - diagramPoint.TopLeft.X,
                diagramPoint.BottomLeft.Y - diagramPoint.TopLeft.Y);

            // Find closest edge
            float distToLeft = Math.Abs(point.X - box.Left);
            float distToRight = Math.Abs(point.X - box.Right);
            float distToTop = Math.Abs(point.Y - box.Top);
            float distToBottom = Math.Abs(point.Y - box.Bottom);

            // Find minimum distance
            float minDist = Math.Min(Math.Min(distToLeft, distToRight), Math.Min(distToTop, distToBottom));

            // Snap to the closest edge
            if (minDist == distToLeft)
                return new PointF(box.Left, Math.Min(Math.Max(box.Top, point.Y), box.Bottom));
            else if (minDist == distToRight)
                return new PointF(box.Right, Math.Min(Math.Max(box.Top, point.Y), box.Bottom));
            else if (minDist == distToTop)
                return new PointF(Math.Min(Math.Max(box.Left, point.X), box.Right), box.Top);
            else
                return new PointF(Math.Min(Math.Max(box.Left, point.X), box.Right), box.Bottom);
        }
        private (PointF, PointF) GetClosestPoints(ClassDiagram source, ClassDiagram target)
        {
            double minDistance = double.MaxValue;
            PointF closestSourcePoint = PointF.Empty;
            PointF closestTargetPoint = PointF.Empty;

            foreach (ClassDiagramPoint sourcePoint in source.Points)
            {
                foreach (ClassDiagramPoint targetPoint in target.Points)
                {
                    // Check all possible point combinations
                    PointF[] sourcePoints = {
                        sourcePoint.TopLeft,
                        sourcePoint.TopRight,
                        sourcePoint.BottomLeft,
                        sourcePoint.BottomRight
                    };

                    PointF[] targetPoints = {
                        targetPoint.TopLeft,
                        targetPoint.TopRight,
                        targetPoint.BottomLeft,
                        targetPoint.BottomRight
                    };

                    for (int i = 0; i < sourcePoints.Length; i++)
                    {
                        for (int j = 0; j < targetPoints.Length; j++)
                        {
                            double distance = CalculateDistance(sourcePoints[i], targetPoints[j]);

                            if (distance < minDistance)
                            {
                                minDistance = distance;
                                closestSourcePoint = sourcePoints[i];
                                closestTargetPoint = targetPoints[j];
                            }
                        }
                    }
                }
            }

            return (closestSourcePoint, closestTargetPoint);
        }

        private double CalculateDistance(PointF point1, PointF point2)
        {
            double deltaX = point2.X - point1.X;
            double deltaY = point2.Y - point1.Y;
            return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }

        private void DrawConnectorPath(Graphics g, RelationshipType relationshipType, List<PointF> points)
        {
            if (points.Count < 2)
                return;

            // Choose style based on relationship type
            Pen pen;
            switch (relationshipType)
            {
                case RelationshipType.Inheritance:
                    pen = new Pen(Color.Blue, 1.5f);
                    break;
                case RelationshipType.Interface:
                    pen = new Pen(Color.Green, 1.5f) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };
                    break;
                case RelationshipType.Composition:
                    pen = new Pen(Color.Red, 1.5f);
                    break;
                case RelationshipType.Association:
                default:
                    pen = new Pen(Color.Black, 1.5f);
                    break;
            }

            // Draw line segments
            for (int i = 0; i < points.Count - 1; i++)
            {
                g.DrawLine(pen, points[i], points[i + 1]);
            }

            // Draw arrow at the end
            float endX = points[points.Count - 1].X;
            float endY = points[points.Count - 1].Y;

            // Calculate direction of the final segment
            float prevX = points[points.Count - 2].X;
            float prevY = points[points.Count - 2].Y;
            float angle = (float)Math.Atan2(endY - prevY, endX - prevX);
            float arrowSize = 10f;

            PointF arrow1 = new PointF(
                endX - arrowSize * (float)Math.Cos(angle - Math.PI / 6.0),
                endY - arrowSize * (float)Math.Sin(angle - Math.PI / 6.0)
            );

            PointF arrow2 = new PointF(
                endX - arrowSize * (float)Math.Cos(angle + Math.PI / 6.0),
                endY - arrowSize * (float)Math.Sin(angle + Math.PI / 6.0)
            );

            g.DrawLine(pen, endX, endY, arrow1.X, arrow1.Y);
            g.DrawLine(pen, endX, endY, arrow2.X, arrow2.Y);

            // Draw specific relationship type indicators
            switch (relationshipType)
            {
                case RelationshipType.Inheritance:
                    // Draw a hollow triangle
                    g.FillPolygon(Brushes.White, new PointF[] {
                        new PointF(endX, endY),
                        arrow1,
                        arrow2
                    });
                    g.DrawPolygon(pen, new PointF[] {
                        new PointF(endX, endY),
                        arrow1,
                        arrow2
                    });
                    break;

                case RelationshipType.Composition:
                    // Draw a filled diamond
                    PointF diamond2 = new PointF(
                        endX - 2 * arrowSize * (float)Math.Cos(angle),
                        endY - 2 * arrowSize * (float)Math.Sin(angle)
                    );

                    g.FillPolygon(Brushes.Black, new PointF[] {
                        new PointF(endX, endY),
                        new PointF(
                            endX - arrowSize * (float)Math.Cos(angle - Math.PI / 4.0),
                            endY - arrowSize * (float)Math.Sin(angle - Math.PI / 4.0)
                        ),
                        diamond2,
                        new PointF(
                            endX - arrowSize * (float)Math.Cos(angle + Math.PI / 4.0),
                            endY - arrowSize * (float)Math.Sin(angle + Math.PI / 4.0)
                        )
                    });
                    break;
            }

            pen.Dispose();
        }
    }
}