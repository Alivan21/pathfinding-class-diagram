namespace PathFindingClassDiagram.Models
{
    /// <summary>
    /// Interface for objects that can be used as inputs in the pathfinding algorithm
    /// </summary>
    public interface IInput
    {
        /// <summary>
        /// The X coordinate of the item
        /// </summary>
        double X { get; }

        /// <summary>
        /// The Y coordinate of the item
        /// </summary>
        double Y { get; }

        /// <summary>
        /// The right edge X coordinate of the item
        /// </summary>
        double Right { get; }

        /// <summary>
        /// The bottom edge Y coordinate of the item
        /// </summary>
        double Bottom { get; }

        /// <summary>
        /// The width of the item
        /// </summary>
        double Width { get; }

        /// <summary>
        /// The height of the item
        /// </summary>
        double Height { get; }
    }
}