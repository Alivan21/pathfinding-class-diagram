// Services/DiagramService.cs
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using PathFindingClassDiagram.Services.DiagramLayouts;
using PathFindingClassDiagram.Services.Interfaces;

namespace PathFindingClassDiagram.Services
{
    public class DiagramService : IDiagramService
    {
        private readonly IDiagramLayoutStrategy _standardLayout;
        private readonly IDiagramLayoutStrategy _pathfindingLayout;

        // Add properties for cell size and buffer
        public float CellSize { get; set; } = 5f;

        public DiagramService()
        {
            _standardLayout = new StandardDiagramLayout();
            _pathfindingLayout = new PathfindingDiagramLayout();
        }

        /// <summary>
        /// Generates a class diagram image
        /// </summary>
        public Image GenerateClassDiagram
        (
            List<Models.ClassDiagram> classDiagrams,
            List<Models.Relationship> relationships,
            bool showRelationships,
            bool usePathfinding = false,
            float cellSize = 5f
        )
        {
            float maxDiagramWidth = classDiagrams.Max(diagram => diagram.CalculateTotalWidth(null));
            int bitmapWidth = (int)Math.Max(Math.Max(1600f, maxDiagramWidth + 100f), maxDiagramWidth);

            float totalHeight = CalculateTotalHeight(classDiagrams);
            int minBitmapHeight = 640;
            int additionalHeightPerDiagram = 76;
            int bitmapHeight = (int)Math.Max(minBitmapHeight, totalHeight + classDiagrams.Count * additionalHeightPerDiagram);

            Bitmap bitmap = new Bitmap(bitmapWidth, bitmapHeight);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(Color.White);

                float xOffset = 50f;
                float yOffset = 50f;
                float currentRowHeight = 0f;

                foreach (Models.ClassDiagram classDiagram in classDiagrams)
                {
                    if (xOffset + classDiagram.CalculateTotalWidth(g) > bitmapWidth)
                    {
                        xOffset = 50f;
                        yOffset += currentRowHeight + additionalHeightPerDiagram;
                        currentRowHeight = 0f;
                    }

                    classDiagram.Draw(g, xOffset, yOffset, classDiagrams);
                    xOffset += classDiagram.CalculateTotalWidth(g) + 54f;
                    currentRowHeight = Math.Max(currentRowHeight, classDiagram.CalculateTotalHeight(g));
                }

                if (showRelationships)
                {
                    // Choose the strategy based on the usePathfinding flag
                    IDiagramLayoutStrategy layoutStrategy = usePathfinding
                        ? _pathfindingLayout
                        : _standardLayout;

                    // Draw relationships using the selected strategy
                    layoutStrategy.DrawRelationships(g, classDiagrams, relationships, bitmapWidth, bitmapHeight, cellSize);
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
    }
}