using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static Item;
using TMPro;

public class ItemPickerUI
{
    private RectTransform _contentArea;
    private Button _buttonPrefab;
    private List<Item> _allItems;

    private readonly Dictionary<Button, int> _buttonItemIndices = new();

    public event Action<Item> ItemPicked;

    public ItemPickerUI(RectTransform contentArea, Button buttonPrefab)
    {
        _contentArea = contentArea;
        _buttonPrefab = buttonPrefab;
    }

    public void Initialize(List<Item> items)
    {
        _allItems = items;

        foreach (Transform child in _contentArea)
        {
            UnityEngine.Object.Destroy(child.gameObject);
        }

        for (int i = 0; i < items.Count; i++)
        {
            int index = i;
            Button button = UnityEngine.Object.Instantiate(_buttonPrefab, _contentArea);
            _buttonItemIndices[button] = index;

            SetButtonVisual(button, index);
            button.onClick.AddListener(() => OnItemButtonClicked(button));
        }
    }

    private void SetButtonVisual(Button button, int itemIndex)
    {
        Item item = _allItems[itemIndex];

        Image image = button.GetComponent<Image>();
        if (image != null && item.Sprite != null)
        {
            image.sprite = item.Sprite;
            image.preserveAspect = true;
        }

        TextMeshProUGUI label = button.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null)
        {
            label.text = item.Id;
        }
    }

    private void OnItemButtonClicked(Button button)
    {
        int currentIndex = _buttonItemIndices[button];

        int newIndex;
        do
        {
            newIndex = UnityEngine.Random.Range(0, _allItems.Count);
        } while (newIndex == currentIndex && _allItems.Count > 1);

        _buttonItemIndices[button] = newIndex;
        SetButtonVisual(button, newIndex);

        ItemPicked?.Invoke(_allItems[newIndex]);
    }
}
