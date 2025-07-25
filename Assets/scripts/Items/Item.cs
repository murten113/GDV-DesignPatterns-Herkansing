using UnityEngine;

public enum ItemType
{
    Basic,
    Advanced,
    Special
}

[System.Serializable]
public class Item
{
    public string Id;
    public int Width;
    public int Height;
    public int ScoreValue;
    public ItemType Type;
    public Sprite Sprite;
    public GameObject Prefab;

    //// Minimal constructor
    //public Item(string id, int width, int height, int scoreValue, ItemType type)
    //{
    //    Id = id;
    //    Width = width;
    //    Height = height;
    //    ScoreValue = scoreValue;
    //    Type = type;
    //    Sprite = null;
    //    Prefab = null;
    //}

    //// Constructor with Sprite
    //public Item(string id, int width, int height, int scoreValue, ItemType type, Sprite sprite)
    //{
    //    Id = id;
    //    Width = width;
    //    Height = height;
    //    ScoreValue = scoreValue;
    //    Type = type;
    //    Sprite = sprite;
    //    Prefab = null;
    //}
    

    // Constructor with Sprite and Prefab
    public Item(string id, int width, int height, int scoreValue, ItemType type, Sprite sprite, GameObject prefab)
    {
        Id = id;
        Width = width;
        Height = height;
        ScoreValue = scoreValue;
        Type = type;
        Sprite = sprite;
        Prefab = prefab;
    }
}
