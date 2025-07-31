using System;
using UnityEngine;

namespace Managers
{
    /// <summary>
    /// EventManager is a static class that manages game events related to item placement and removal.
    /// </summary>
    public static class EventManager
    {
        public static event Action<Item, int, int> OnItemPlaced;
        public static event Action<Item, int, int> OnItemRemoved;

        public static void ItemPlaced(Item item, int x, int y) => OnItemPlaced?.Invoke(item, x, y);
        public static void ItemRemoved(Item item, int x, int y) => OnItemRemoved?.Invoke(item, x, y);
    }
}