using UnityEngine;
using UnityEngine.UI;
using Items;

public class HeldItemUI
{
    private Image _heldItemImage;

    public HeldItemUI(Image heldItemImage)
    {
        _heldItemImage = heldItemImage;
        Hide();
    }

    public void Show(Item item, Sprite sprite)
    {
        if (item == null || sprite == null)
        {
            Hide();
            return;
        }

        _heldItemImage.sprite = sprite;
        _heldItemImage.enabled = true;
    }

    public void Hide()
    {
        _heldItemImage.sprite = null;
        _heldItemImage.enabled = false;
    }
}
