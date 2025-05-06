using System;
using System.Collections.Generic;
using System.Drawing;
using PathFindingClassDiagram.Models;

namespace PathFindingClassDiagram.PathFinding
{
    /// <summary>
    /// Represents a grid overlay for the diagram space, marking obstacles and providing
    /// conversions between diagram coordinates and grid cells.
    /// </summary>
    public class DiagramGrid
    {
        private readonly bool[,] _blockedCells;
        private readonly float _cellSize;
        private readonly int _rows;
        private readonly int _cols;

        /// <summary>
        /// Initializes a new instance of the DiagramGrid class.
        /// </summary>
        /// <param name="width">The width of the diagram area.</param>
        /// <param name="height">The height of the diagram area.</param>
        /// <param name="cellSize">The size of each grid cell (smaller = more detailed routing, but slower).</param>
        public DiagramGrid(float width, float height, float cellSize = 10f)
        {
            _cellSize = cellSize;
            _rows = (int)(height / cellSize) + 1;
            _cols = (int)(width / cellSize) + 1;
            _blockedCells = new bool[_rows, _cols];
        }

        /// <summary>
        /// Gets the cell size used by this grid.
        /// </summary>
        public float CellSize => _cellSize;

        /// <summary>
        /// Gets the number of rows in the grid.
        /// </summary>
        public int Rows => _rows;

        /// <summary>
        /// Gets the number of columns in the grid.
        /// </summary>
        public int Columns => _cols;

        /// <summary>
        /// Mark cells occupied by class diagrams as obstacles.
        /// </summary>
        /// <param name="diagrams">The list of class diagrams to mark as obstacles.</param>
        /// <summary>
        /// Mark cells occupied by class diagrams as obstacles with optional buffer
        /// </summary>
        public void MarkObstacles(List<ClassDiagram> diagrams, bool addBuffer = true)
        {
            foreach (var diagram in diagrams)
            {
                if (diagram.Points.Count == 0) continue;

                var point = diagram.Points[0];
                float left = point.TopLeft.X;
                float top = point.TopLeft.Y;
                float right = point.BottomRight.X;
                float bottom = point.BottomRight.Y;

                // Convert rectangle to grid cells and mark them as obstacles
                int startRow = Math.Max(0, (int)(top / _cellSize));
                int endRow = Math.Min(_rows - 1, (int)Math.Ceiling(bottom / _cellSize));
                int startCol = Math.Max(0, (int)(left / _cellSize));
                int endCol = Math.Min(_cols - 1, (int)Math.Ceiling(right / _cellSize));

                for (int row = startRow; row <= endRow; row++)
                {
                    for (int col = startCol; col <= endCol; col++)
                    {
                        _blockedCells[row, col] = true;
                    }
                }

                // Add buffer around obstacle if requested
                if (addBuffer)
                {
                    int buffer = 1; // Buffer size in cells
                    for (int row = Math.Max(0, startRow - buffer); row <= Math.Min(_rows - 1, endRow + buffer); row++)
                    {
                        for (int col = Math.Max(0, startCol - buffer); col <= Math.Min(_cols - 1, endCol + buffer); col++)
                        {
                            // Only mark buffer cells if they're not already part of the obstacle
                            if (row < startRow || row > endRow || col < startCol || col > endCol)
                            {
                                _blockedCells[row, col] = true;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Checks if a cell at the specified row and column is blocked (either by being outside
        /// the grid boundaries or by being marked as an obstacle).
        /// </summary>
        /// <param name="row">The row to check.</param>
        /// <param name="col">The column to check.</param>
        /// <returns>True if the cell is blocked, false otherwise.</returns>
        public bool IsCellBlocked(int row, int col) =>
            row < 0 || row >= _rows || col < 0 || col >= _cols || _blockedCells[row, col];

        /// <summary>
        /// Converts a grid cell to a diagram coordinate point.
        /// </summary>
        /// <param name="row">The row of the cell.</param>
        /// <param name="col">The column of the cell.</param>
        /// <returns>The point at the center of the cell.</returns>
        public PointF CellToPoint(int row, int col) =>
            new PointF(col * _cellSize + _cellSize / 2, row * _cellSize + _cellSize / 2);

        /// <summary>
        /// Converts a diagram coordinate point to a grid cell.
        /// </summary>
        /// <param name="point">The point to convert.</param>
        /// <returns>A tuple containing the row and column of the cell.</returns>
        public (int Row, int Col) PointToCell(PointF point) =>
            ((int)(point.Y / _cellSize), (int)(point.X / _cellSize));
    }
}