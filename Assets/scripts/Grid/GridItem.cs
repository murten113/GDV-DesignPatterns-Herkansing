namespace Grid
{
    public class GridItem
    {

        /// <summary>
        /// GridItem is a class that represents an item in the grid with its dimensions.
        /// </summary>
        public int Width { get; }
        public int Height { get; }

        public GridItem(int width, int height)
        {
            Width = width;
            Height = height;
        }
    }
}
