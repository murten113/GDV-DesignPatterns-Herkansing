using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemPickerUI
{
    /// This class handles the UI for picking items in the game.
    private RectTransform contentArea;
    private Button buttonPrefab;
    private List<Item> allItems;

    public event Action<Item> ItemPicked;

    /// <summary>
    /// Constructor for ItemPickerUI.
    /// </summary>
    /// <param name="contentArea"></param>
    /// <param name="buttonPrefab"></param>
    public ItemPickerUI(RectTransform contentArea, Button buttonPrefab)
    {
        this.contentArea = contentArea;
        this.buttonPrefab = buttonPrefab;
    }


    /// <summary>
    /// Initializes the item picker UI with a list of items.
    /// </summary>
    /// <param name="items"></param>
    public void Initialize(List<Item> items)
    {
        allItems = new List<Item>(items);

        foreach (var item in items)
        {

            Button button = GameObject.Instantiate(buttonPrefab, contentArea);
            TextMeshProUGUI buttonText = button.GetComponentInChildren<TextMeshProUGUI>();


            Item currentItem = item;
            buttonText.text = $"{currentItem.Width}x{currentItem.Height}";

            button.onClick.AddListener(() =>
            {
                
                ItemPicked?.Invoke(currentItem);
                Debug.Log($"Picked up item {currentItem.Id} ({currentItem.Width}x{currentItem.Height})");


                Item nextItem;
                do
                {
                    nextItem = allItems[UnityEngine.Random.Range(0, allItems.Count)];
                } while (nextItem == currentItem && allItems.Count > 1);


                currentItem = nextItem;
                buttonText.text = $"{currentItem.Width}x{currentItem.Height}";
            });
        }
    }
}
