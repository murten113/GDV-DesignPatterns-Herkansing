using UnityEngine;
using UnityEngine.UI;

public class HeldItemUI
{
    private readonly Image _heldItemImage;


    /// <summary>
    /// HeldItemUI is a class that manages the display of the currently held item in the UI.
    /// </summary>
    /// <param name="heldItemImage"></param>
    public HeldItemUI(Image heldItemImage)
    {
        _heldItemImage = heldItemImage;
        _heldItemImage.enabled = false;
    }


    /// <summary>
    /// Shows the held item in the UI with the specified sprite.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="sprite"></param>
    public void Show(Item item, Sprite sprite)
    {
        _heldItemImage.sprite = sprite;
        _heldItemImage.enabled = true;
    }


    /// <summary>
    /// Hides the held item UI, clearing the sprite and disabling the image.
    /// </summary>
    public void Hide()
    {
        _heldItemImage.enabled = false;
        _heldItemImage.sprite = null;
    }
}
