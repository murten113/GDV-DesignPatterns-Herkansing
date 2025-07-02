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
    [SerializeField] private int gridWidth = 10;
    [SerializeField] private int gridHeight = 6;

    [Header("Grid Settings")]
    [SerializeField] private float cellSize = 80f;

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

        List<Item> items = new List<Item>
        {
            new Item("item01", 1, 1, 5, ItemType.Basic),
            new Item("item02", 2, 2, 10, ItemType.Basic),
            new Item("item03", 3, 1, 15, ItemType.Basic)
        };
        _itemPickerUI.Initialize(items);

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
                Camera.main,
                out Vector2 localPoint))
        {
            return;
        }

        int gridX = Mathf.FloorToInt(localPoint.x / cellSize);
        int gridY = Mathf.FloorToInt(-localPoint.y / cellSize);

        // Check bounds
        if (gridX < 0 || gridY < 0 || gridX >= gridWidth || gridY >= gridHeight)
        {
            Debug.Log("Click was outside the grid bounds.");
            return;
        }

        if (_gridSystem.CanPlaceItem(_currentItem, gridX, gridY))
        {
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
        else
        {
            Debug.Log("Cannot place item here!");
        }
    }

    private void OnPlaceItemVisual(Item item, int x, int y)
    {
        GameObject itemGO = Instantiate(itemVisualPrefab, gridVisualContainer);
        RectTransform rt = itemGO.GetComponent<RectTransform>();
        rt.anchoredPosition = new Vector2(x * cellSize, -y * cellSize);
        rt.sizeDelta = new Vector2(item.Width * cellSize, item.Height * cellSize);

        Image img = itemGO.GetComponent<Image>();
        if (img != null && item.Sprite != null)
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
        if (scoreText != null)
        {
            scoreText.text = $"Score: {_score}";
        }
    }
}
