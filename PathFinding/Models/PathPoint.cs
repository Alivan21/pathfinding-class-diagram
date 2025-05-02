using System.Drawing;

namespace PathFindingClassDiagram.PathFinding.Models
{
    public class PathPoint
    {
        public double X { get; set; }
        public double Y { get; set; }

        public PathPoint() { }

        public PathPoint(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static implicit operator PointF(PathPoint p) => new PointF((float)p.X, (float)p.Y);
        public static implicit operator PathPoint(PointF p) => new PathPoint(p.X, p.Y);
    }
}
