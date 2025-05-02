namespace PathFindingClassDiagram.PathFinding.Models
{
    public class Rectangle
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public double Right => X + Width;
        public double Bottom => Y + Height;

        public Rectangle(double x, double y, double width, double height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public bool Contains(PathPoint point)
        {
            return point.X >= X && point.X <= Right &&
                   point.Y >= Y && point.Y <= Bottom;
        }

        public bool Intersects(Rectangle rect)
        {
            return !(rect.X > Right ||
                    rect.Right < X ||
                    rect.Y > Bottom ||
                    rect.Bottom < Y);
        }
    }
}
