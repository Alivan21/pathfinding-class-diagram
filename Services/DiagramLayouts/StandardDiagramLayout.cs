using System;
using System.Collections.Generic;
using System.Drawing;
using PathFindingClassDiagram.Models;
using PathFindingClassDiagram.Services.Interfaces;

namespace PathFindingClassDiagram.Services.DiagramLayouts
{
    public class StandardDiagramLayout : IDiagramLayoutStrategy
    {
        public void DrawRelationships(Graphics g, List<ClassDiagram> classDiagrams,
                                     List<Relationship> relationships, int bitmapWidth, int bitmapHeight, float cellSize)
        {
            // Extract the existing direct-line relationship drawing code from DiagramService
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
                    (PointF source, PointF target) = GetClosestPoints(sourceClassDiagram, targetClassDiagram);
                    sourceClassDiagram.DrawArrow(g, Pens.Red, source.X, source.Y, target.X, target.Y);
                }
            }
        }

        // Include the GetClosestPoints, CalculateDistance, and other helper methods from DiagramService
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
    }
}