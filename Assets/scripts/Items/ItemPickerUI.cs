using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using Items;

public class ItemPickerUI
{
    private RectTransform _contentArea;
    private Button _itemButtonPrefab;
    private List<Item> _availableItems;

    private Dictionary<Item, Button> _itemButtons = new();

    public delegate void OnItemPicked(Item item);
    public event OnItemPicked ItemPicked;

    public ItemPickerUI(RectTransform contentArea, Button itemButtonPrefab)
    {
        _contentArea = contentArea;
        _itemButtonPrefab = itemButtonPrefab;
        _availableItems = new List<Item>();
    }

    public void Initialize(List<Item> items)
    {
        _availableItems = items;
        PopulateItems();
    }

    private void PopulateItems()
    {
        foreach (var item in _availableItems)
        {
            var buttonInstance = GameObject.Instantiate(_itemButtonPrefab, _contentArea);
            var tmpText = buttonInstance.GetComponentInChildren<TextMeshProUGUI>();
            if (tmpText != null)
                tmpText.text = item.Id;

            buttonInstance.onClick.AddListener(() => OnItemButtonClicked(item));
            _itemButtons[item] = buttonInstance;
        }
    }

    private void OnItemButtonClicked(Item item)
    {
        ItemPicked?.Invoke(item);

        // Remove button from UI and dictionary
        if (_itemButtons.TryGetValue(item, out Button button))
        {
            GameObject.Destroy(button.gameObject);
            _itemButtons.Remove(item);
        }
    }
}
