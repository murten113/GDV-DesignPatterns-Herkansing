using System;
using System.Collections.Generic;

namespace Grid
{
    public class GridSystem
    {
        private readonly int _width;
        private readonly int _height;
        private readonly bool[,] _occupied;

        public GridSystem(int width, int height)
        {
            _width = width;
            _height = height;
            _occupied = new bool[width, height];
        }

        public bool IsInsideGrid(int x, int y)
        {
            return x >= 0 && y >= 0 && x < _width && y < _height;
        }

        public bool CanPlaceItem(Item item, int startX, int startY)
        {
            // Check every cell the item would occupy
            for (int x = 0; x < item.Width; x++)
            {
                for (int y = 0; y < item.Height; y++)
                {
                    int checkX = startX + x;
                    int checkY = startY + y;

                    // Check if out of grid
                    if (!IsInsideGrid(checkX, checkY))
                        return false;

                    // Check if occupied
                    if (_occupied[checkX, checkY])
                        return false;
                }
            }
            return true;
        }

        public void PlaceItem(Item item, int startX, int startY)
        {
            // Mark the grid cells as occupied
            for (int x = 0; x < item.Width; x++)
            {
                for (int y = 0; y < item.Height; y++)
                {
                    int cellX = startX + x;
                    int cellY = startY + y;

                    if (IsInsideGrid(cellX, cellY))
                        _occupied[cellX, cellY] = true;
                }
            }
        }

        public void RemoveItem(Item item, int startX, int startY)
        {
            for (int x = 0; x < item.Width; x++)
            {
                for (int y = 0; y < item.Height; y++)
                {
                    int cellX = startX + x;
                    int cellY = startY + y;

                    if (IsInsideGrid(cellX, cellY))
                        _occupied[cellX, cellY] = false;
                }
            }
        }
    }
}
