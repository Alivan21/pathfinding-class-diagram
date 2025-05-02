using System.Collections.Generic;
using System.Drawing;
using System;
using PathFindingClassDiagram.Services.Interfaces;
using System.Linq;

namespace PathFindingClassDiagram.Services
{
    public class DiagramService: IDiagramService
    {
        /// <summary>
        /// Generates a class diagram image
        /// </summary>
        public Image GenerateClassDiagram(
            List<Models.ClassDiagram> classDiagrams,
            List<Models.Relationship> relationships,
            bool showRelationships)
        {
            float maxDiagramWidth = classDiagrams.Max(diagram => diagram.CalculateTotalWidth(null));
            int bitmapWidth = (int)Math.Max(Math.Max(1600f, maxDiagramWidth + 100f), maxDiagramWidth);

            float totalHeight = CalculateTotalHeight(classDiagrams);
            int minBitmapHeight = 800;
            int additionalHeightPerDiagram = 80;
            int bitmapHeight = (int)Math.Max(minBitmapHeight, totalHeight + (float)(classDiagrams.Count * additionalHeightPerDiagram));

            Bitmap bitmap = new Bitmap(bitmapWidth, bitmapHeight);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.White);

                float xOffset = 50f;
                float yOffset = 50f;
                float currentRowHeight = 0f;

                foreach (Models.ClassDiagram classDiagram in classDiagrams)
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
        /// Calculates the total height of all class diagrams
        /// </summary>
        private float CalculateTotalHeight(List<Models.ClassDiagram> classDiagrams)
        {
            float totalHeight = 0f;
            foreach (Models.ClassDiagram classDiagram in classDiagrams)
            {
                totalHeight += classDiagram.CalculateTotalHeight(null);
            }
            return totalHeight;
        }

        /// <summary>
        /// Draws relationships between classes
        /// </summary>
        private void DrawRelationships(Graphics g, List<Models.ClassDiagram> classDiagrams, List<Models.Relationship> relationships)
        {
            Dictionary<string, Models.ClassDiagram> classDiagramDictionary = new Dictionary<string, Models.ClassDiagram>();
            foreach (Models.ClassDiagram classDiagram in classDiagrams)
            {
                if (!string.IsNullOrEmpty(classDiagram.ClassName))
                    classDiagramDictionary[classDiagram.ClassName] = classDiagram;
            }

            foreach (Models.Relationship relationship in relationships)
            {
                if (classDiagramDictionary.TryGetValue(relationship.SourceClass, out var sourceClassDiagram) &&
                    classDiagramDictionary.TryGetValue(relationship.TargetClass, out var targetClassDiagram))
                {
                    (PointF source, PointF target) = GetClosestPoints(sourceClassDiagram, targetClassDiagram);
                    sourceClassDiagram.DrawArrow(g, Pens.Red, source.X, source.Y, target.X, target.Y);
                }
            }
        }

        /// <summary>
        /// Gets the closest points between two class diagrams
        /// </summary>
        private (PointF, PointF) GetClosestPoints(Models.ClassDiagram source, Models.ClassDiagram target)
        {
            double minDistance = double.MaxValue;
            PointF closestSourcePoint = PointF.Empty;
            PointF closestTargetPoint = PointF.Empty;

            foreach (Models.ClassDiagramPoint sourcePoint in source.Points)
            {
                foreach (Models.ClassDiagramPoint targetPoint in target.Points)
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

        /// <summary>
        /// Calculates the distance between two points
        /// </summary>
        private double CalculateDistance(PointF point1, PointF point2)
        {
            double deltaX = point2.X - point1.X;
            double deltaY = point2.Y - point1.Y;
            return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }
    }
}
