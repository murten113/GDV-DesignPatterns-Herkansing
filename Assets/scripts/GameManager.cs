using UnityEngine;
using Grid;
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
    private Dictionary<(int x, int y), GameObject> _cellGameObjects = new();
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

        // Load sprites and prefabs from Resources or inspector
        Sprite sprite1 = Resources.Load<Sprite>("Sprites/item01");
        Sprite sprite2 = Resources.Load<Sprite>("Sprites/item02");
        Sprite sprite3 = Resources.Load<Sprite>("Sprites/item03");

        GameObject prefab1 = Resources.Load<GameObject>("Prefabs/item01");
        GameObject prefab2 = Resources.Load<GameObject>("Prefabs/item02");
        GameObject prefab3 = Resources.Load<GameObject>("Prefabs/item03");

        _itemPickerUI.Initialize(new List<Item>
    {
        new Item("item01", 1, 1, 5, ItemType.Basic, sprite1, prefab1),
        new Item("item02", 2, 2, 10, ItemType.Basic, sprite2, prefab2),
        new Item("item03", 3, 1, 15, ItemType.Basic, sprite3, prefab3)
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

                _cellGameObjects[(x, y)] = cellGO;
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
            return;

        Rect rect = gridVisualContainer.rect;
        float px = localPoint.x - rect.xMin;
        float py = localPoint.y - rect.yMin;

        int gridX = Mathf.FloorToInt(px / cellSize);
        int gridY = Mathf.FloorToInt(py / cellSize);

        gridY = (gridHeight - 1) - gridY;

        Debug.Log($"Clicked grid cell: ({gridX}, {gridY})");

        // Ensure the whole item fits within grid bounds
        bool fitsInBounds =
            gridX >= 0 &&
            gridY >= 0 &&
            gridX + _currentItem.Width <= gridWidth &&
            gridY + _currentItem.Height <= gridHeight;

        if (!fitsInBounds || !_gridSystem.CanPlaceItem(_currentItem, gridX, gridY))
        {
            Debug.Log("Cannot place item here or out of bounds.");
            return;
        }


        var cmd = new PlaceItemCommand(
            _gridSystem,
            _currentItem,
            gridX,
            gridY,
            OnPlaceItemVisual,
            OnRemoveItemVisual
        );
        _commandInvoker.ExecuteCommand(cmd);

        _currentItem = null;
        _heldItemUI.Hide();
    }


    private void OnPlaceItemVisual(Item item, int x, int y)
    {
        if (!_cellGameObjects.TryGetValue((x, y), out var cellGO))
        {
            Debug.LogError($"Cell ({x},{y}) not found!");
            return;
        }

        GameObject prefab = item.Prefab ?? itemVisualPrefab; // fallback to default if none set
        var itemGO = Instantiate(prefab, cellGO.transform);
        var rt = itemGO.GetComponent<RectTransform>();

        rt.anchoredPosition = Vector2.zero;
        rt.sizeDelta = new Vector2(item.Width * cellSize, item.Height * cellSize);

        if (item.Sprite != null && itemGO.TryGetComponent<Image>(out var img))
            img.sprite = item.Sprite;

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