﻿using System.Collections.Generic;
using System.Drawing;

namespace PathFindingClassDiagram.Services.Interfaces
{
    public interface IDiagramService
    {
        Image GenerateClassDiagram(
            List<Models.ClassDiagram> classDiagrams,
            List<Models.Relationship> relationships,
            bool showRelationships,
            bool usePathfinding = false,
            float cellSize = 5f
            );
    }
}