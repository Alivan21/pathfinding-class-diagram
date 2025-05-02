using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using PathFindingClassDiagram.Models;
using PathFindingClassDiagram.Services.Interfaces;
using PathFindingClassDiagram.PathFinding.Models;
using PathFindingClassDiagram.PathFinding;

namespace PathFindingClassDiagram.Services
{
    public class DiagramService : IDiagramService
    {
        private readonly PathfindingAlgorithm _pathfindingAlgorithm;
        private bool _usePathfinding;

        public DiagramService(bool usePathfinding = true)
        {
            _pathfindingAlgorithm = new PathfindingAlgorithm();
            _usePathfinding = usePathfinding;
        }

        /// <summary>
        /// Generates a class diagram image
        /// </summary>
        public Image GenerateClassDiagram(
            List<ClassDiagram> classDiagrams,
            List<Relationship> relationships,
            bool showRelationships)
        {
            float maxDiagramWidth = classDiagrams.Max(diagram => diagram.CalculateTotalWidth(null));
            int bitmapWidth = (int)Math.Max(Math.Max(1600f, maxDiagramWidth + 100f), maxDiagramWidth);

            float totalHeight = CalculateTotalHeight(classDiagrams);
            int minBitmapHeight = 800;
            int additionalHeightPerDiagram = 80;
            int bitmapHeight = (int)Math.Max(minBitmapHeight, totalHeight + (float)(classDiagrams.Count * additionalHeightPerDiagram));

            // Create larger bitmap for complex diagrams
            if (classDiagrams.Count > 20 || relationships.Count > 30)
            {
                bitmapWidth = Math.Max(bitmapWidth, 2400);
                bitmapHeight = Math.Max(bitmapHeight, 1800);
            }

            Bitmap bitmap = new Bitmap(bitmapWidth, bitmapHeight);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.White);
                g.SmoothingMode = SmoothingMode.AntiAlias;  // Enable anti-aliasing for smoother lines

                // Improve layout for diagrams with many classes
                ImproveLayout(classDiagrams, bitmapWidth, bitmapHeight);

                float xOffset = 50f;
                float yOffset = 50f;
                float currentRowHeight = 0f;

                foreach (ClassDiagram classDiagram in classDiagrams)
                {
                    if (xOffset + classDiagram.CalculateTotalWidth(g) > (float)bitmapWidth)
                    {
                        xOffset = 50f;
                        yOffset += currentRowHeight + (float)additionalHeightPerDiagram;
                        currentRowHeight = 0f;
                    }

                    classDiagram.Draw(g, xOffset, yOffset, classDiagrams);
                    xOffset += classDiagram.CalculateTotalWidth(g) + 50f;
                    currentRowHeight = Math.Max(currentRowHeight, classDiagram.CalculateTotalHeight(g));
                }

                if (showRelationships)
                {
                    DrawRelationships(g, classDiagrams, relationships);
                }
            }

            return bitmap;
        }

        /// <summary>
        /// Improve layout to minimize crossings
        /// </summary>
        private void ImproveLayout(List<ClassDiagram> classDiagrams, int width, int height)
        {
            // For diagrams with many classes, arrange them in a grid pattern
            if (classDiagrams.Count > 12)
            {
                int cols = (int)Math.Ceiling(Math.Sqrt(classDiagrams.Count));
                int rows = (int)Math.Ceiling((double)classDiagrams.Count / cols);

                float cellWidth = width / (cols + 1);
                float cellHeight = height / (rows + 1);

                // Group related classes closer together if possible
                // Simple strategy: sort by class name similarity
                classDiagrams.Sort((a, b) => string.Compare(a.ClassName, b.ClassName, StringComparison.Ordinal));

                for (int i = 0; i < classDiagrams.Count; i++)
                {
                    int row = i / cols;
                    int col = i % cols;

                    if (classDiagrams[i].Points.Count > 0)
                    {
                        // Adjust the first point's position - this will be used for drawing
                        float x = (col + 0.5f) * cellWidth;
                        float y = (row + 0.5f) * cellHeight;

                        // Tweak position for better spacing
                        x += (float)(Math.Sin(i * 0.1) * 20);  // Add slight variation
                        y += (float)(Math.Cos(i * 0.1) * 20);

                        // Create new point with updated position
                        var origPoint = classDiagrams[i].Points[0];
                        classDiagrams[i].Points[0] = new ClassDiagramPoint(
                            origPoint.ClassName,
                            new PointF(x, y),
                            new PointF(x + origPoint.TopRight.X - origPoint.TopLeft.X, y),
                            new PointF(x, y + origPoint.BottomLeft.Y - origPoint.TopLeft.Y),
                            new PointF(x + origPoint.TopRight.X - origPoint.TopLeft.X,
                                     y + origPoint.BottomLeft.Y - origPoint.TopLeft.Y)
                        );
                    }
                }
            }
        }

        /// <summary>
        /// Calculates the total height of all class diagrams
        /// </summary>
        private float CalculateTotalHeight(List<ClassDiagram> classDiagrams)
        {
            float totalHeight = 0f;
            foreach (ClassDiagram classDiagram in classDiagrams)
            {
                totalHeight += classDiagram.CalculateTotalHeight(null);
            }
            return totalHeight;
        }

        /// <summary>
        /// Draws relationships between classes
        /// </summary>
        private void DrawRelationships(Graphics g, List<ClassDiagram> classDiagrams, List<Relationship> relationships)
        {
            Dictionary<string, ClassDiagram> classDiagramDictionary = new Dictionary<string, ClassDiagram>();
            foreach (ClassDiagram classDiagram in classDiagrams)
            {
                if (!string.IsNullOrEmpty(classDiagram.ClassName))
                    classDiagramDictionary[classDiagram.ClassName] = classDiagram;
            }

            // Sort relationships to prioritize more important ones
            var sortedRelationships = relationships
                .OrderByDescending(r => r.Type == RelationshipType.Inheritance)
                .ThenByDescending(r => r.Type == RelationshipType.Interface)
                .ThenByDescending(r => r.Type == RelationshipType.Composition)
                .ThenBy(r => r.Type == RelationshipType.Association)
                .ToList();

            foreach (Relationship relationship in sortedRelationships)
            {
                if (classDiagramDictionary.TryGetValue(relationship.SourceClass, out var sourceClassDiagram) &&
                    classDiagramDictionary.TryGetValue(relationship.TargetClass, out var targetClassDiagram))
                {
                    if (_usePathfinding)
                    {
                        DrawPathfindingRelationship(g, sourceClassDiagram, targetClassDiagram, relationship, classDiagrams);
                    }
                    else
                    {
                        DrawDirectRelationship(g, sourceClassDiagram, targetClassDiagram, relationship);
                    }
                }
            }
        }

        /// <summary>
        /// Draws a relationship using pathfinding to avoid obstacles
        /// </summary>
        private void DrawPathfindingRelationship(
            Graphics g,
            ClassDiagram source,
            ClassDiagram target,
            Relationship relationship,
            List<ClassDiagram> allDiagrams)
        {
            if (source.Points.Count == 0 || target.Points.Count == 0)
                return;

            ClassDiagramPoint sourcePoint = source.Points[0];
            ClassDiagramPoint targetPoint = target.Points[0];

            // Get the closest connection points between the classes
            PointF closestSourcePoint = GetOptimalConnectionPoint(sourcePoint, targetPoint);
            PointF closestTargetPoint = GetOptimalConnectionPoint(targetPoint, sourcePoint);

            // Convert to pathfinding points
            var startPoint = new PathPoint(closestSourcePoint.X, closestSourcePoint.Y);
            var endPoint = new PathPoint(closestTargetPoint.X, closestTargetPoint.Y);

            // Find path using pathfinding algorithm
            List<PathPoint> path = _pathfindingAlgorithm.FindPath(startPoint, endPoint, allDiagrams, g);

            // Draw the path
            if (path.Count >= 2)
            {
                // Create the pen based on relationship type
                Pen pen = GetRelationshipPen(relationship.Type);

                // Draw path with GraphicsPath for smoother rendering
                using (GraphicsPath graphicsPath = new GraphicsPath())
                {
                    for (int i = 0; i < path.Count - 1; i++)
                    {
                        graphicsPath.AddLine(
                            (float)path[i].X,
                            (float)path[i].Y,
                            (float)path[i + 1].X,
                            (float)path[i + 1].Y
                        );
                    }

                    g.DrawPath(pen, graphicsPath);
                }

                // Draw arrowhead
                DrawArrowhead(g, pen, (PointF)path[path.Count - 2], (PointF)path[path.Count - 1], relationship.Type);
            }
        }

        /// <summary>
        /// Gets the optimal connection point to minimize path length and crossing
        /// </summary>
        private PointF GetOptimalConnectionPoint(ClassDiagramPoint source, ClassDiagramPoint target)
        {
            PointF[] connectionPoints = {
                source.TopCenter,    // Top
                source.RightCenter,  // Right
                source.BottomCenter, // Bottom
                source.LeftCenter    // Left
            };

            // Get the center of the target class
            PointF targetCenter = new PointF(
                (target.TopLeft.X + target.TopRight.X) / 2,
                (target.TopLeft.Y + target.BottomLeft.Y) / 2
            );

            // Find the closest connection point
            double minDistance = double.MaxValue;
            PointF bestPoint = connectionPoints[0];

            foreach (PointF point in connectionPoints)
            {
                double distance = CalculateDistance(point, targetCenter);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    bestPoint = point;
                }
            }

            return bestPoint;
        }

        /// <summary>
        /// Draws a direct relationship without pathfinding
        /// </summary>
        private void DrawDirectRelationship(Graphics g, ClassDiagram source, ClassDiagram target, Relationship relationship)
        {
            (PointF sourcePoint, PointF targetPoint) = GetClosestPoints(source, target);

            // Create the pen based on relationship type
            Pen pen = GetRelationshipPen(relationship.Type);

            // Draw the line
            g.DrawLine(pen, sourcePoint, targetPoint);

            // Draw arrowhead
            DrawArrowhead(g, pen, sourcePoint, targetPoint, relationship.Type);
        }

        /// <summary>
        /// Gets the closest points between two class diagrams
        /// </summary>
        private (PointF, PointF) GetClosestPoints(ClassDiagram source, ClassDiagram target)
        {
            double minDistance = double.MaxValue;
            PointF closestSourcePoint = PointF.Empty;
            PointF closestTargetPoint = PointF.Empty;

            foreach (ClassDiagramPoint sourcePoint in source.Points)
            {
                foreach (ClassDiagramPoint targetPoint in target.Points)
                {
                    // Check each connection point pair
                    PointF[] sourcePoints = {
                        sourcePoint.TopCenter,
                        sourcePoint.RightCenter,
                        sourcePoint.BottomCenter,
                        sourcePoint.LeftCenter
                    };

                    PointF[] targetPoints = {
                        targetPoint.TopCenter,
                        targetPoint.RightCenter,
                        targetPoint.BottomCenter,
                        targetPoint.LeftCenter
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

        /// <summary>
        /// Calculates the distance between two points
        /// </summary>
        private double CalculateDistance(PointF point1, PointF point2)
        {
            double deltaX = point2.X - point1.X;
            double deltaY = point2.Y - point1.Y;
            return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }

        /// <summary>
        /// Gets the appropriate pen for a relationship type
        /// </summary>
        private Pen GetRelationshipPen(RelationshipType relationshipType)
        {
            Pen pen;
            switch (relationshipType)
            {
                case RelationshipType.Inheritance:
                    pen = new Pen(Color.Blue, 1.5f);
                    break;
                case RelationshipType.Interface:
                    pen = new Pen(Color.Green, 1.5f);
                    pen.DashStyle = DashStyle.Dash;
                    break;
                case RelationshipType.Composition:
                    pen = new Pen(Color.Red, 1.5f);
                    break;
                case RelationshipType.Association:
                default:
                    pen = new Pen(Color.DarkBlue, 1.5f);
                    break;
            }

            // Use rounded line joins for better appearance
            pen.LineJoin = LineJoin.Round;
            pen.EndCap = LineCap.ArrowAnchor;

            return pen;
        }

        /// <summary>
        /// Draws an arrowhead at the end of a line
        /// </summary>
        private void DrawArrowhead(Graphics g, Pen pen, PointF start, PointF end, RelationshipType relationshipType)
        {
            // Direction vector
            float dx = end.X - start.X;
            float dy = end.Y - start.Y;

            // Normalize
            float length = (float)Math.Sqrt(dx * dx + dy * dy);
            if (length < 0.0001f) return; // Avoid division by zero

            dx /= length;
            dy /= length;

            // Arrowhead parameters
            float arrowSize = 10f;
            float angle = 30f * (float)(Math.PI / 180f); // 30 degrees in radians

            // Calculate arrowhead points
            PointF arrowEnd = end;
            PointF arrow1 = new PointF(
                arrowEnd.X - arrowSize * ((float)Math.Cos(Math.Atan2(dy, dx) + angle)),
                arrowEnd.Y - arrowSize * ((float)Math.Sin(Math.Atan2(dy, dx) + angle))
            );
            PointF arrow2 = new PointF(
                arrowEnd.X - arrowSize * ((float)Math.Cos(Math.Atan2(dy, dx) - angle)),
                arrowEnd.Y - arrowSize * ((float)Math.Sin(Math.Atan2(dy, dx) - angle))
            );

            // For Inheritance and Interface, draw a triangle
            if (relationshipType == RelationshipType.Inheritance || relationshipType == RelationshipType.Interface)
            {
                // Fill the triangle for Inheritance, leave it empty for Interface
                using (Brush brush = relationshipType == RelationshipType.Inheritance
                    ? new SolidBrush(pen.Color)
                    : new SolidBrush(Color.White))
                {
                    PointF[] trianglePoints = { arrowEnd, arrow1, arrow2 };
                    g.FillPolygon(brush, trianglePoints);
                    g.DrawPolygon(pen, trianglePoints);
                }
            }
            // For Composition, draw a filled diamond
            else if (relationshipType == RelationshipType.Composition)
            {
                // Diamond size
                float diamondSize = 8f;

                // Calculate diamond points
                PointF diamondTip = end;
                PointF diamondBase = new PointF(
                    end.X - 2 * diamondSize * dx,
                    end.Y - 2 * diamondSize * dy
                );
                PointF diamondLeft = new PointF(
                    end.X - diamondSize * dx - diamondSize * dy,
                    end.Y - diamondSize * dy + diamondSize * dx
                );
                PointF diamondRight = new PointF(
                    end.X - diamondSize * dx + diamondSize * dy,
                    end.Y - diamondSize * dy - diamondSize * dx
                );

                // Draw diamond
                PointF[] diamondPoints = { diamondTip, diamondLeft, diamondBase, diamondRight };
                g.FillPolygon(new SolidBrush(pen.Color), diamondPoints);
                g.DrawPolygon(pen, diamondPoints);
            }
            else
            {
                // Simple arrow for association
                g.DrawLine(pen, arrowEnd, arrow1);
                g.DrawLine(pen, arrowEnd, arrow2);
            }
        }
    }
}