using System.Collections.Generic;

namespace PathFindingClassDiagram.PathFinding.Models
{
    public class Connection
    {
        public List<PathPoint> Points { get; set; }

        public PathPoint Start => Points[0];
        public PathPoint End => Points[1];

        public Connection(PathPoint start, PathPoint end)
        {
            Points = new List<PathPoint>() { start, end };
        }
    }
}
