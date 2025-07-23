using System;
using System.Collections.Generic;
using Items;
using UnityEngine;
using UnityEngine.UI;

public class ItemPickerUI
{
    private readonly RectTransform _contentArea;
    private readonly Button _buttonPrefab;

    public event Action<Item> ItemPicked;

    public ItemPickerUI(RectTransform contentArea, Button buttonPrefab)
    {
        _contentArea = contentArea;
        _buttonPrefab = buttonPrefab;
    }

    public void Initialize(List<Item> items)
    {
        foreach (Transform child in _contentArea)
            UnityEngine.Object.Destroy(child.gameObject);

        foreach (var item in items)
        {
            var buttonGO = UnityEngine.Object.Instantiate(_buttonPrefab, _contentArea);
            var button = buttonGO.GetComponent<Button>();

            // Create or use an Image for the item sprite
            var iconGO = new GameObject("Icon", typeof(RectTransform), typeof(CanvasRenderer), typeof(Image));
            iconGO.transform.SetParent(buttonGO.transform, false);

            var iconRT = iconGO.GetComponent<RectTransform>();
            var iconImage = iconGO.GetComponent<Image>();
            iconImage.sprite = item.Sprite;

            // Scale to represent item footprint visually (normalized to fit button)
            float maxButtonSize = Mathf.Min(buttonGO.GetComponent<RectTransform>().rect.width, buttonGO.GetComponent<RectTransform>().rect.height);
            float scaleX = item.Width;
            float scaleY = item.Height;

            // Normalize so the largest dimension fits within the button
            float maxDim = Mathf.Max(scaleX, scaleY);
            scaleX = (scaleX / maxDim) * (maxButtonSize * 0.8f);
            scaleY = (scaleY / maxDim) * (maxButtonSize * 0.8f);

            iconRT.sizeDelta = new Vector2(scaleX, scaleY);
            iconRT.anchoredPosition = Vector2.zero;

            button.onClick.AddListener(() => ItemPicked?.Invoke(item));
        }
    }
}
