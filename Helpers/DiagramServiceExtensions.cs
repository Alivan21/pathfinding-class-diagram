using System;
using System.Reflection;
using PathFindingClassDiagram.Services;
using PathFindingClassDiagram.Services.Interfaces;

// This is a helper extension that you can add to your project to ensure 
// the pathfinding service is properly disabled when the toggle is unchecked

namespace PathFindingClassDiagram.Helpers
{
    public static class DiagramServiceExtensions
    {
        public static void SetPathfindingEnabled(this IDiagramService diagramService, bool enabled)
        {
            if (diagramService is DiagramService ds)
            {
                // Get private field using reflection
                FieldInfo field = ds.GetType().GetField("_pathFindingService",
                    BindingFlags.NonPublic | BindingFlags.Instance);

                if (field != null)
                {
                    // Store the original service if we haven't yet
                    if (!_hasStoredOriginal)
                    {
                        _originalService = field.GetValue(ds) as IPathFindingService;
                        _hasStoredOriginal = true;
                    }

                    // Enable or disable pathfinding by setting or clearing the service
                    if (enabled)
                    {
                        field.SetValue(ds, _originalService);
                    }
                    else
                    {
                        field.SetValue(ds, null);
                    }
                }
            }
        }

        // Static fields to store the original pathfinding service
        private static IPathFindingService _originalService;
        private static bool _hasStoredOriginal = false;
    }
}