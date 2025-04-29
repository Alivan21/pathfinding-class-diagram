using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.IO;
using PathFindingClassDiagram.Models;
using PathFindingClassDiagram.Services.Interfaces;

namespace PathFindingClassDiagram.Helpers
{
    /// <summary>
    /// Helper class for debugging pathfinding operations
    /// </summary>
    public static class PathFindingDebuggingHelper
    {
        /// <summary>
        /// Logs pathfinding information to a file for debugging
        /// </summary>
        public static void LogPathFindingInfo(
            IPathFindingService pathFindingService,
            List<ClassDiagram> classDiagrams,
            bool isEnabled)
        {
            try
            {
                // Create output directory if it doesn't exist
                string outputDir = Path.Combine(
                    Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location),
                    "Debug");

                if (!Directory.Exists(outputDir))
                {
                    Directory.CreateDirectory(outputDir);
                }

                // Create a log file
                string logFile = Path.Combine(outputDir, $"pathfinding_log_{DateTime.Now:yyyyMMdd_HHmmss}.txt");

                using (StreamWriter writer = new StreamWriter(logFile))
                {
                    writer.WriteLine($"PathFinding Debug Log - {DateTime.Now}");
                    writer.WriteLine($"PathFinding Enabled: {isEnabled}");
                    writer.WriteLine($"PathFinding Service: {(pathFindingService != null ? pathFindingService.GetType().Name : "null")}");
                    writer.WriteLine($"Number of Class Diagrams: {classDiagrams.Count}");
                    writer.WriteLine();

                    writer.WriteLine("Class Diagrams:");
                    foreach (var diagram in classDiagrams)
                    {
                        writer.WriteLine($"  - {diagram.ClassName}");
                        if (diagram.Points.Count > 0)
                        {
                            var point = diagram.Points[0];
                            writer.WriteLine($"    Position: TopLeft({point.TopLeft.X},{point.TopLeft.Y}), " +
                                           $"BottomRight({point.BottomRight.X},{point.BottomRight.Y})");
                        }

                        if (diagram.Relationships != null && diagram.Relationships.Count > 0)
                        {
                            writer.WriteLine($"    Relationships:");
                            foreach (var rel in diagram.Relationships)
                            {
                                writer.WriteLine($"      -> {rel.TargetClass} ({rel.Type})");
                            }
                        }
                    }

                    // If pathfinding is enabled and service is available, calculate paths and log them
                    if (isEnabled && pathFindingService != null)
                    {
                        writer.WriteLine();
                        writer.WriteLine("Calculated Paths:");
                        var paths = pathFindingService.CalculateRelationshipPaths(classDiagrams);
                        writer.WriteLine($"Number of Path Segments: {paths.Count}");

                        foreach (var segment in paths)
                        {
                            writer.WriteLine($"  - From: ({segment.Start.X},{segment.Start.Y}) To: ({segment.End.X},{segment.End.Y})");
                        }
                    }
                }

                Debug.WriteLine($"PathFinding debug log written to {logFile}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error writing pathfinding debug log: {ex.Message}");
            }
        }
    }
}