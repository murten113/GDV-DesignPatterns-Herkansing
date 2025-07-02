using System;

namespace Events
{
    public static class EventManager
    {
        // Event triggered when an item is placed: passes Item and position
        public static event Action<Items.Item, int, int> OnItemPlaced;

        // Call this to notify listeners
        public static void ItemPlaced(Items.Item item, int x, int y)
        {
            OnItemPlaced?.Invoke(item, x, y);
        }
    }
}
