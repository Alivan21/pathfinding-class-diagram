using System.Collections.Generic;
using System.Drawing;
using PathFindingClassDiagram.Models;

namespace PathFindingClassDiagram.Services.Interfaces
{
    public interface IDiagramLayoutStrategy
    {
        void DrawRelationships(Graphics g, List<ClassDiagram> classDiagrams,
                             List<Relationship> relationships, int bitmapWidth, int bitmapHeight, float cellSize);
    }
}