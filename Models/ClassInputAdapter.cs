using System.Linq;

namespace PathFindingClassDiagram.Models
{
    /// <summary>
    /// Adapter that converts a ClassDiagram to an IInput for pathfinding
    /// </summary>
    public class ClassInputAdapter : IInput
    {
        private readonly ClassDiagram _classDiagram;

        /// <summary>
        /// Creates a new instance of the ClassInputAdapter
        /// </summary>
        /// <param name="classDiagram">The class diagram to adapt</param>
        public ClassInputAdapter(ClassDiagram classDiagram)
        {
            _classDiagram = classDiagram;
        }

        /// <summary>
        /// Gets the X coordinate of the class diagram
        /// </summary>
        public double X => _classDiagram.Points.FirstOrDefault()?.TopLeft.X ?? 0;

        /// <summary>
        /// Gets the Y coordinate of the class diagram
        /// </summary>
        public double Y => _classDiagram.Points.FirstOrDefault()?.TopLeft.Y ?? 0;

        /// <summary>
        /// Gets the right edge X coordinate of the class diagram
        /// </summary>
        public double Right => _classDiagram.Points.FirstOrDefault()?.BottomRight.X ?? 0;

        /// <summary>
        /// Gets the bottom edge Y coordinate of the class diagram
        /// </summary>
        public double Bottom => _classDiagram.Points.FirstOrDefault()?.BottomRight.Y ?? 0;

        /// <summary>
        /// Gets the width of the class diagram
        /// </summary>
        public double Width => Right - X;

        /// <summary>
        /// Gets the height of the class diagram
        /// </summary>
        public double Height => Bottom - Y;
    }
}