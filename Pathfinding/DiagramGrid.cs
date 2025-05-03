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
        public void MarkObstacles(List<ClassDiagram> diagrams)
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

        /// <summary>
        /// Creates a margin around obstacles to prevent connectors from being drawn too close to diagram boxes.
        /// </summary>
        /// <param name="margin">The number of cells to add as margin.</param>
        public void AddMarginToObstacles(int margin = 1)
        {
            // Create a copy of the current blocked state
            bool[,] blockedCopy = new bool[_rows, _cols];
            Array.Copy(_blockedCells, blockedCopy, _blockedCells.Length);

            // Extend obstacles by adding margin
            for (int row = 0; row < _rows; row++)
            {
                for (int col = 0; col < _cols; col++)
                {
                    if (blockedCopy[row, col])
                    {
                        // Mark surrounding cells within margin as blocked
                        for (int dRow = -margin; dRow <= margin; dRow++)
                        {
                            for (int dCol = -margin; dCol <= margin; dCol++)
                            {
                                int newRow = row + dRow;
                                int newCol = col + dCol;

                                if (newRow >= 0 && newRow < _rows && newCol >= 0 && newCol < _cols)
                                {
                                    _blockedCells[newRow, newCol] = true;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}