using UnityEngine;
using UnityEngine.UI;

public class HeldItemUI
{
    private readonly Image _heldItemImage;

    public HeldItemUI(Image heldItemImage)
    {
        _heldItemImage = heldItemImage;
        _heldItemImage.enabled = false;
    }

    public void Show(Item item, Sprite sprite)
    {
        _heldItemImage.sprite = sprite;
        _heldItemImage.enabled = true;
    }

    public void Hide()
    {
        _heldItemImage.enabled = false;
        _heldItemImage.sprite = null;
    }
}
