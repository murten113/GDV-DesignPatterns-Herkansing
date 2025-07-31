using Grid;
using Commands;
using System;

namespace Commands
{
    public class PlaceItemCommand : ICommand
    {

        /// <summary>
        /// PlaceItemCommand is a command that places an item on a grid at specified coordinates.
        /// </summary>
        private GridSystem _gridSystem;
        private readonly Item _item;
        private int _x, _y;

        private Action<Item, int, int> _onPlaceVisual;
        private Action<Item, int, int> _onRemoveVisual;

        /// <summary>
        /// Constructor for PlaceItemCommand.
        /// </summary>
        /// <param name="gridSystem"></param>
        /// <param name="item"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="onPlaceVisual"></param>
        /// <param name="onRemoveVisual"></param>
        public PlaceItemCommand(GridSystem gridSystem, Item item, int x, int y,
            Action<Item, int, int> onPlaceVisual = null,
            Action<Item, int, int> onRemoveVisual = null)
        {
            _gridSystem = gridSystem;
            _item = item;
            _x = x;
            _y = y;

            _onPlaceVisual = onPlaceVisual;
            _onRemoveVisual = onRemoveVisual;
        }


        /// <summary>
        /// Executes the command to place the item on the grid at the specified coordinates.
        /// </summary>
        public void Execute()
        {
            if (_gridSystem.CanPlaceItem(_item, _x, _y))
            {
                _gridSystem.PlaceItem(_item, _x, _y);
                _onPlaceVisual?.Invoke(_item, _x, _y);
            }
        }


        /// <summary>
        /// Undoes the command by removing the item from the grid at the specified coordinates.
        /// </summary>
        public void Undo()
        {
            _gridSystem.RemoveItem(_item, _x, _y);
            _onRemoveVisual?.Invoke(_item, _x, _y);
        }
    }
}
