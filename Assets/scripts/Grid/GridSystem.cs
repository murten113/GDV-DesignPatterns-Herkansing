using System;
using Items;  

namespace Grid
{
    public class GridSystem
    {
        private readonly int _width;
        private readonly int _height;
        private readonly bool[,] _cells;

        public GridSystem(int width, int height)
        {
            _width = width;
            _height = height;
            _cells = new bool[width, height];
        }

        public bool CanPlaceItem(Item item, int startX, int startY)
        {
            // Check bounds
            if (startX < 0 || startY < 0 ||
                startX + item.Width > _width ||
                startY + item.Height > _height)
                return false;

            // Check if grid cells are free
            for (int x = startX; x < startX + item.Width; x++)
            {
                for (int y = startY; y < startY + item.Height; y++)
                {
                    if (IsCellOccupied(x, y))
                        return false;
                }
            }

            return true;
        }

        public void PlaceItem(Item item, int x, int y)
        {
            for (int i = x; i < x + item.Width; i++)
            {
                for (int j = y; j < y + item.Height; j++)
                {
                    MarkCellOccupied(i, j, true);
                }
            }
            // Store reference to item for potential removal
        }

        public void RemoveItem(Item item, int startX, int startY)
        {
            for (int x = 0; x < item.Width; x++)
            {
                for (int y = 0; y < item.Height; y++)
                {
                    int gridX = startX + x;
                    int gridY = startY + y;

                    if (IsInsideGrid(gridX, gridY))
                        MarkCellOccupied(gridX, gridY, false);
                }
            }
        }

        private void MarkCellOccupied(int x, int y, bool occupied)
        {
            if (IsInsideGrid(x, y))
            {
                _cells[x, y] = occupied;
            }
        }

        public bool IsInsideGrid(int x, int y)
        {
            return x >= 0 && x < _width && y >= 0 && y < _height;
        }

        public bool IsCellOccupied(int x, int y)
        {
            return IsInsideGrid(x, y) && _cells[x, y];
        }
    }
}
