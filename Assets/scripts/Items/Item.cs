using UnityEngine;


/// <summary>
/// ItemType is an enumeration that defines the types of items available in the game.
/// </summary>
public enum ItemType
{
    Basic,
    Advanced,
    Special
}

/// <summary>
/// Item is a class that represents an item in the game.
/// </summary>
[System.Serializable]
public class Item
{
    public string Id;
    public int Width;
    public int Height;
    public int ScoreValue;
    public ItemType Type;
    public Sprite Sprite;


    /// <summary>
    /// Constructor for Item class.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="scoreValue"></param>
    /// <param name="type"></param>
    public Item(string id, int width, int height, int scoreValue, ItemType type)
    {
        Id = id;
        Width = width;
        Height = height;
        ScoreValue = scoreValue;
        Type = type;
        Sprite = null;
    }
}
