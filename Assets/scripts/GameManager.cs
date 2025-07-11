using UnityEngine;
using Grid;
using Items;
using Commands;
using Managers;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    [Header("Grid Configuration")]
    [SerializeField] private int gridWidth = 10;
    [SerializeField] private int gridHeight = 6;
    [SerializeField] private float cellSize = 60f;

    [Header("UI References")]
    [SerializeField] private RectTransform itemPickerContentArea;
    [SerializeField] private Button itemPickerButtonPrefab;
    [SerializeField] private RectTransform gridVisualContainer;
    [SerializeField] private GameObject gridCellPrefab;
    [SerializeField] private GameObject itemVisualPrefab;
    [SerializeField] private Image heldItemUIImage;
    [SerializeField] private TextMeshProUGUI scoreText;

    private GridSystem _gridSystem;
    private CommandInvoker _commandInvoker;
    private Item _currentItem;

    private ItemPickerUI _itemPickerUI;
    private HeldItemUI _heldItemUI;

    private Dictionary<(int x, int y), GameObject> _placedItemVisuals = new();
    private int _score = 0;

    private void Awake()
    {
        InitializeGame();
    }

    private void InitializeGame()
    {
        _gridSystem = new GridSystem(gridWidth, gridHeight);
        _commandInvoker = new CommandInvoker();

        _itemPickerUI = new ItemPickerUI(itemPickerContentArea, itemPickerButtonPrefab);
        _heldItemUI = new HeldItemUI(heldItemUIImage);

        _itemPickerUI.ItemPicked += OnItemPickedFromUI;

        _itemPickerUI.Initialize(new List<Item>
        {
            new Item("item01", 1, 1, 5, ItemType.Basic),
            new Item("item02", 2, 2, 10, ItemType.Basic),
            new Item("item03", 3, 1, 15, ItemType.Basic)
        });

        CreateGridVisual();
        UpdateScore(0);
    }

    private void CreateGridVisual()
    {
        gridVisualContainer.sizeDelta = new Vector2(gridWidth * cellSize, gridHeight * cellSize);

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                GameObject cellGO = Instantiate(gridCellPrefab, gridVisualContainer);
                RectTransform rt = cellGO.GetComponent<RectTransform>();
                rt.anchoredPosition = new Vector2(x * cellSize, -y * cellSize);
                rt.sizeDelta = new Vector2(cellSize, cellSize);
            }
        }
    }

    private void OnItemPickedFromUI(Item item)
    {
        _currentItem = item;

        if (item.Sprite != null)
        {
            _heldItemUI.Show(item, item.Sprite);
        }

        Debug.Log($"Picked up item {item.Id}");
    }

    private void Update()
    {
        if (_currentItem != null && Input.GetMouseButtonDown(0))
        {
            HandlePlacement();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            _commandInvoker.UndoLast();
        }
    }

    private void HandlePlacement()
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
            gridVisualContainer,
            Input.mousePosition,
            null,
            out Vector2 localPoint))
        {
            return;
        }

        Vector2 originOffset = new Vector2(
            -gridVisualContainer.pivot.x * gridVisualContainer.rect.width,
             gridVisualContainer.pivot.y * gridVisualContainer.rect.height
        );

        Vector2 relativePoint = localPoint - originOffset;

        int gridX = Mathf.FloorToInt(relativePoint.x / cellSize);
        int gridY = Mathf.FloorToInt(-relativePoint.y / cellSize);

        Debug.Log($"Mouse: {Input.mousePosition}, Grid: ({gridX}, {gridY})");

        if (!_gridSystem.IsInsideGrid(gridX, gridY) || !_gridSystem.CanPlaceItem(_currentItem, gridX, gridY))
        {
            Debug.Log("Cannot place item here or out of bounds.");
            return;
        }

        var placeCommand = new PlaceItemCommand(
            _gridSystem,
            _currentItem,
            gridX,
            gridY,
            OnPlaceItemVisual,
            OnRemoveItemVisual
        );

        _commandInvoker.ExecuteCommand(placeCommand);
        _currentItem = null;
        _heldItemUI.Hide();
    }


    private void OnPlaceItemVisual(Item item, int x, int y)
    {
        GameObject itemGO = Instantiate(itemVisualPrefab, gridVisualContainer);
        RectTransform rt = itemGO.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(x * cellSize, -y * cellSize);
        rt.sizeDelta = new Vector2(item.Width * cellSize, item.Height * cellSize);

        if (item.Sprite != null && itemGO.TryGetComponent(out Image img))
        {
            img.sprite = item.Sprite;
        }

        _placedItemVisuals[(x, y)] = itemGO;
        UpdateScore(item.ScoreValue);
    }

    private void OnRemoveItemVisual(Item item, int x, int y)
    {
        if (_placedItemVisuals.TryGetValue((x, y), out GameObject itemGO))
        {
            Destroy(itemGO);
            _placedItemVisuals.Remove((x, y));
            UpdateScore(-item.ScoreValue);
        }
    }

    private void UpdateScore(int delta)
    {
        _score += delta;
        scoreText.text = $"Score: {_score}";
    }
}
