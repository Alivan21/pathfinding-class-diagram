using System.Collections.Generic;
using PathFindingClassDiagram.Models;

namespace PathFindingClassDiagram.Services.Interfaces
{
    public interface IPathFindingService
    {
        /// <summary>
        /// Margin to use around classes when finding paths
        /// </summary>
        double Margin { get; set; }

        /// <summary>
        /// Calculates optimal paths for relationships between class diagrams
        /// </summary>
        /// <param name="classDiagrams">List of class diagrams with relationships</param>
        /// <returns>List of path segments representing the optimal paths</returns>
        List<PathSegment> CalculateRelationshipPaths(List<ClassDiagram> classDiagrams);
    }
}