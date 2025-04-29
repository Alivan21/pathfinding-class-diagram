using System.Collections.Generic;

namespace PathFindingClassDiagram.Models
{
    /// <summary>
    /// Represents a connector between two class diagrams
    /// </summary>
    public class Connector
    {
        /// <summary>
        /// The source class diagram input
        /// </summary>
        public IInput Source { get; set; }

        /// <summary>
        /// The destination class diagram input
        /// </summary>
        public IInput Destination { get; set; }

        /// <summary>
        /// The orientation of the connector on the source class
        /// </summary>
        public ConnectorOrientation SourceOrientation { get; set; }

        /// <summary>
        /// The orientation of the connector on the destination class
        /// </summary>
        public ConnectorOrientation DestinationOrientation { get; set; }

        /// <summary>
        /// The path segments that make up this connector
        /// </summary>
        public List<PathSegment> ConnectorPath { get; set; } = new List<PathSegment>();

        /// <summary>
        /// Creates a new instance of the Connector class
        /// </summary>
        public Connector()
        {
        }
    }
}