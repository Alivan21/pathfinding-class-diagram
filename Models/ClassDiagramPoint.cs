using System.Drawing;

namespace PathFindingClassDiagram.Models
{
    public class ClassDiagramPoint
    {
        /// <summary>
        /// The class name
        /// </summary>
        public string ClassName { get; set; }

        /// <summary>
        /// Top-left corner point
        /// </summary>
        public PointF TopLeft { get; set; }

        /// <summary>
        /// Top-right corner point
        /// </summary>
        public PointF TopRight { get; set; }

        /// <summary>
        /// Bottom-left corner point
        /// </summary>
        public PointF BottomLeft { get; set; }

        /// <summary>
        /// Bottom-right corner point
        /// </summary>
        public PointF BottomRight { get; set; }

        /// <summary>
        /// Center of the top edge
        /// </summary>
        public PointF TopCenter { get; set; }

        /// <summary>
        /// Center of the bottom edge
        /// </summary>
        public PointF BottomCenter { get; set; }

        /// <summary>
        /// Center of the left edge
        /// </summary>
        public PointF LeftCenter { get; set; }

        /// <summary>
        /// Center of the right edge
        /// </summary>
        public PointF RightCenter { get; set; }

        /// <summary>
        /// Initializes a new instance of the ClassDiagramPoint class
        /// </summary>
        public ClassDiagramPoint(string className, PointF topLeft, PointF topRight, PointF bottomLeft, PointF bottomRight)
        {
            ClassName = className;
            TopLeft = topLeft;
            TopRight = topRight;
            BottomLeft = bottomLeft;
            BottomRight = bottomRight;
            TopCenter = new PointF((topLeft.X + topRight.X) / 2f, topLeft.Y);
            BottomCenter = new PointF((bottomLeft.X + bottomRight.X) / 2f, bottomLeft.Y);
            LeftCenter = new PointF(topLeft.X, (topLeft.Y + bottomLeft.Y) / 2f);
            RightCenter = new PointF(topRight.X, (topRight.Y + bottomRight.Y) / 2f);
        }
    }
}
}
