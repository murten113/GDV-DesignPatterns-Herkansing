public class GridSystem
{

    /// <summary>
    /// GridSystem is a class that manages a grid of items, allowing placement, removal, and checking of item positions.
    /// </summary>
    private readonly int _width;
    private readonly int _height;
    private readonly Item[,] _grid;

    /// <summary>
    /// Constructor for GridSystem.
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public GridSystem(int width, int height)
    {
        _width = width;
        _height = height;
        _grid = new Item[width, height];
    }


    /// <summary>
    /// Checks if the given coordinates are inside the grid boundaries.
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns></returns>
    public bool IsInsideGrid(int x, int y)
    {
        return x >= 0 && y >= 0 && x < _width && y < _height;
    }


    /// <summary>
    /// Checks if an item can be placed at the specified coordinates without overlapping existing items.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="startX"></param>
    /// <param name="startY"></param>
    /// <returns></returns>
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


    /// <summary>
    /// Places an item on the grid at the specified coordinates.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="startX"></param>
    /// <param name="startY"></param>
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


    /// <summary>
    /// Removes an item from the grid at the specified coordinates.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="startX"></param>
    /// <param name="startY"></param>
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

    /// <summary>
    /// Clears the entire grid, removing all items.
    /// </summary>
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
