using Grid;
using Commands;
using System;

namespace Commands
{
    public class PlaceItemCommand : ICommand
    {
        private GridSystem _gridSystem;
        private readonly Item _item;
        private int _x, _y;

        private Action<Item, int, int> _onPlaceVisual;
        private Action<Item, int, int> _onRemoveVisual;

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

        public void Execute()
        {
            if (_gridSystem.CanPlaceItem(_item, _x, _y))
            {
                _gridSystem.PlaceItem(_item, _x, _y);
                _onPlaceVisual?.Invoke(_item, _x, _y);
            }
        }

        public void Undo()
        {
            _gridSystem.RemoveItem(_item, _x, _y);
            _onRemoveVisual?.Invoke(_item, _x, _y);
        }
    }
}
