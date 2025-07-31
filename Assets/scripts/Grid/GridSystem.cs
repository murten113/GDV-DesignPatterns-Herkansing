public class GridSystem
{
    private readonly int _width;
    private readonly int _height;
    private readonly Item[,] _grid;

    public GridSystem(int width, int height)
    {
        _width = width;
        _height = height;
        _grid = new Item[width, height];
    }

    public bool IsInsideGrid(int x, int y)
    {
        return x >= 0 && y >= 0 && x < _width && y < _height;
    }

    public bool CanPlaceItem(Item item, int startX, int startY)
    {
        for (int x = 0; x < item.Width; x++)
        {
            for (int y = 0; y < item.Height; y++)
            {
                int checkX = startX + x;
                int checkY = startY + y;

                if (!IsInsideGrid(checkX, checkY) || _grid[checkX, checkY] != null)
                    return false;
            }
        }

        return true;
    }

    public void PlaceItem(Item item, int startX, int startY)
    {
        for (int x = 0; x < item.Width; x++)
        {
            for (int y = 0; y < item.Height; y++)
            {
                _grid[startX + x, startY + y] = item;
            }
        }
    }

    public void RemoveItem(Item item, int startX, int startY)
    {
        for (int x = 0; x < item.Width; x++)
        {
            for (int y = 0; y < item.Height; y++)
            {
                _grid[startX + x, startY + y] = null;
            }
        }
    }

    public void Clear()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                _grid[x, y] = null;
            }
        }
    }
}
