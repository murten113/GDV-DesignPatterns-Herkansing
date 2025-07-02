using UnityEngine;

namespace Items
{
    public class Item
    {
        public string Id { get; private set; }
        public int Width { get; private set; }
        public int Height { get; private set; }
        public int ScoreValue { get; private set; }
        public ItemType Type { get; private set; }
        public Sprite Sprite { get; private set; }  // Add this property

        public Item(string id, int width, int height, int scoreValue, ItemType type, Sprite sprite = null)
        {
            Id = id;
            Width = width;
            Height = height;
            ScoreValue = scoreValue;
            Type = type;
            Sprite = sprite;  // Assign sprite
        }
    }

    public enum ItemType
        {
            Basic,
            Rare,
            Heavy,
        }
}
