using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Good.Core
{
    public struct Overlap
    {
        public Vector2 Depth;
        public Sprite Other;
    }

    public class LayoutGrid
    {
        private struct Cell 
        {
            public HashSet<Sprite> Sprites;
        }

        private readonly Cell[,] cells;

        public LayoutGrid(int numRows, int numColumns, int cellSize) 
        {
            NumRows = numRows;
            NumColumns = numColumns;
            CellSize = cellSize;

            cells = new Cell[numRows, numColumns];

            for (int y = 0; y < numRows; y++)
                for (int x = 0; x < numColumns; x++)
                    cells[y, x].Sprites = new HashSet<Sprite>();
        }

        public int CellSize { get; }
        public int NumRows { get; }
        public int NumColumns { get; }

        public void Update(Sprite sprite)
        {
            if (sprite.BodyInfo.Position != sprite.BodyInfo.PreviousPosition)
            {
                Remove(sprite);
                Add(sprite);
            }
        }

        internal void Add(Sprite sprite)
        {
            foreach (var cellPos in GetOccupyingCells(sprite.BodyInfo.Bounds))
                cells[cellPos.Y, cellPos.X].Sprites.Add(sprite);
        }

        internal void Remove(Sprite sprite)
        {
            foreach (var cellPos in GetOccupyingCells(sprite.BodyInfo.PreviousBounds))
                cells[cellPos.Y, cellPos.X].Sprites.Remove(sprite);
        }

        public bool IsOverlapping(Sprite sprite, out Overlap overlap)
        {
            overlap = new Overlap();
            var overlappedObject = QueryBounds(sprite.BodyInfo.Bounds).FirstOrDefault(other => other != sprite);
            if (overlappedObject != null)
            {
                overlap.Depth = GetIntersectionDepth(sprite.BodyInfo.Bounds, overlappedObject.BodyInfo.Bounds);
                overlap.Other = overlappedObject;
                return true;
            }
            return false;
        }

        public bool IsCollidingAtOffset(Sprite sprite, float xOffset, float yOffset, out Overlap overlap)
        {
            overlap = new Overlap();
            var offsetBounds = sprite.BodyInfo.Bounds;
            offsetBounds.Offset(xOffset, yOffset);
            var overlappedObject = QueryBounds(offsetBounds).FirstOrDefault(other => other != sprite && other.BodyInfo.IsSolid);
            if (overlappedObject != null)
            {
                overlap.Depth = GetIntersectionDepth(offsetBounds, overlappedObject.BodyInfo.Bounds);
                overlap.Other = overlappedObject;
                return true;
            }
            return false;
        }

        public IEnumerable<Sprite> QueryPoint(Point point) 
        {
            if (TryGetCellPosition(point.ToVector2(), out int col, out int row))
                return cells[row, col].Sprites.Where(other => other.BodyInfo.Bounds.Contains(point));

            return Enumerable.Empty<Sprite>();
        }

        public IEnumerable<Sprite> QueryBounds(Rectangle bounds)
        {
            return GetOccupyingCells(bounds)
                .SelectMany(cell => cells[cell.Y, cell.X].Sprites)
                .Distinct()
                .Where(other => bounds.Intersects(other.BodyInfo.Bounds));
        }

        private IEnumerable<Point> GetOccupyingCells(Rectangle bounds)
        {
            return GetBoundPointCells(bounds).Distinct();
        }

        private IEnumerable<Point> GetBoundPointCells(Rectangle bounds)
        {
            if (TryGetCellPosition(new Vector2(bounds.X, bounds.Top), out int topLeftCol, out int topLeftRow))
                yield return new Point(topLeftCol, topLeftRow);

            if (TryGetCellPosition(new Vector2(bounds.Right, bounds.Top), out int topRightCol, out int topRightRow))
                yield return new Point(topRightCol, topRightRow);

            if (TryGetCellPosition(new Vector2(bounds.X, bounds.Bottom), out int bottomLeftCol, out int bottomLeftRow))
                yield return new Point(bottomLeftCol, bottomLeftRow);

            if (TryGetCellPosition(new Vector2(bounds.Right, bounds.Bottom), out int bottomRightCol, out int bottomRightRow))
                yield return new Point(bottomRightCol, bottomRightRow);
        }

        public bool TryGetCellPosition(Vector2 position, out int column, out int row)
        {
            column = (int)(position.X / CellSize);
            row = (int)(position.Y / CellSize);
            return column >= 0 && column < NumColumns && row >= 0 && row < NumRows;
        }

        private static Vector2 GetIntersectionDepth(Rectangle a, Rectangle b)
        {
            float halfWidthA = a.Width / 2f;
            float halfHeightA = a.Height / 2f;
            float halfWidthB = b.Width / 2f;
            float halfHeightB = b.Height / 2f;

            Vector2 centerA = new Vector2(a.Left + halfWidthA, a.Top + halfHeightA);
            Vector2 centerB = new Vector2(b.Left + halfWidthB, b.Top + halfHeightB);

            float distanceX = centerA.X - centerB.X;
            float distanceY = centerA.Y - centerB.Y;
            float minDistanceX = halfWidthA + halfWidthB;
            float minDistanceY = halfHeightA + halfHeightB;

            if (Math.Abs(distanceX) >= minDistanceX || Math.Abs(distanceY) >= minDistanceY)
                return Vector2.Zero;

            float depthX = distanceX > 0 ? minDistanceX - distanceX : -minDistanceX - distanceX;
            float depthY = distanceY > 0 ? minDistanceY - distanceY : -minDistanceY - distanceY;

            return new Vector2(depthX, depthY);
        }
    }
}
