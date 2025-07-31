using UnityEngine;
using Grid;
using static Item;
using Commands;
using Managers;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// GameManager is the main controller for the game, managing the grid, item placement, UI interactions, and game state.
    /// </summary>
    #region Serialized Fields
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

    [Header("Menu UI")]
    [SerializeField] private GameObject menuPanel; 
    [SerializeField] private GameObject gamePanel;
    [SerializeField] private Button doneButton;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private Button playAgainButton;
    [SerializeField] private Button mainMenuButton;
    #endregion

    #region Constants
    private GridSystem _gridSystem;
    private CommandInvoker _commandInvoker;
    private Item _currentItem;

    private ItemPickerUI _itemPickerUI;
    private HeldItemUI _heldItemUI;

    private Dictionary<(int x, int y), GameObject> _placedItemVisuals = new();
    private Dictionary<(int x, int y), GameObject> _cellGameObjects = new();
    private int _score = 0;
    #endregion

    #region Menu Navigation

    /// <summary>
    /// Awake is called when the script instance is being loaded.
    /// </summary>
    private void Awake()
    {
        ShowMenu();
        doneButton.onClick.AddListener(ShowResultScreen);
        playAgainButton.onClick.AddListener(RestartGame);
        mainMenuButton.onClick.AddListener(ReturnToMainMenu);

        resultPanel.SetActive(false);
    }

    /// <summary>
    /// Starts the game by hiding the menu panel and initializing the game state.
    /// </summary>
    public void StartGame()
    {
        menuPanel.SetActive(false);
        gamePanel.SetActive(true);
        InitializeGame();
    }


    /// <summary>
    /// Shows the main menu panel and hides the game panel.
    /// </summary>
    public void ShowMenu()
    {
        menuPanel.SetActive(true);
        gamePanel.SetActive(false);
    }


    /// <summary>
    /// Quits the game application. In the editor, it stops play mode.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }


    /// <summary>
    /// Displays the result screen with the final score and hides the game panel.
    /// </summary>
    private void ShowResultScreen()
    {
        finalScoreText.text = $"Final Score: {_score}";
        resultPanel.SetActive(true);
        gamePanel.SetActive(false);
        Time.timeScale = 0f; 
    }


    /// <summary>
    /// Restarts the game by resetting the game state, hiding the result panel, and showing the game panel.
    /// </summary>
    private void RestartGame()
    {
        resultPanel.SetActive(false);
        gamePanel.SetActive(true);

        ResetGameState();
    }


    /// <summary>
    /// Returns to the main menu by hiding the result panel and showing the menu panel.
    /// </summary>
    private void ReturnToMainMenu()
    {
        resultPanel.SetActive(false);
        menuPanel.SetActive(true);
    }


    /// <summary>
    /// Resets the game state, clearing the grid and removing all placed item visuals.
    /// </summary>
    private void ResetGameState()
    {
        _gridSystem.Clear(); 
        foreach (var visual in _placedItemVisuals.Values)
            Destroy(visual);

        _placedItemVisuals.Clear();
        _score = 0;
        scoreText.text = "Score: 0";
    }


    #endregion

    #region Game Logic

    /// <summary>
    /// Initializes the game by setting up the grid system, command invoker, item picker UI, and held item UI.
    /// </summary>
    private void InitializeGame()
    {
        _gridSystem = new GridSystem(gridWidth, gridHeight);
        _commandInvoker = new CommandInvoker();

        _itemPickerUI = new ItemPickerUI(itemPickerContentArea, itemPickerButtonPrefab);
        _heldItemUI = new HeldItemUI(heldItemUIImage);

        _itemPickerUI.ItemPicked += OnItemPickedFromUI;

        _itemPickerUI.Initialize(new List<Item>
        {
            new Item("1X1", 1, 1, 5, ItemType.Basic),
            new Item("2X2", 2, 2, 15, ItemType.Special),
            new Item("3X1", 3, 1, 10, ItemType.Advanced)
        });

        CreateGridVisual();
        UpdateScore(0);
    }


    /// <summary>
    /// Creates the visual representation of the grid based on the specified width, height, and cell size.
    /// </summary>
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


    /// <summary>
    /// Handles the item picked from the UI, updating the current item and showing it in the held item UI.
    /// </summary>
    /// <param name="item"></param>
    private void OnItemPickedFromUI(Item item)
    {
        _currentItem = item;

        if (item.Sprite != null)
        {
            _heldItemUI.Show(item, item.Sprite);
        }

        Debug.Log($"Picked up item {item.Id}");
    }


    /// <summary>
    /// Update is called once per frame. It checks for mouse input to place items and handles undo commands.
    /// </summary>
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


    /// <summary>
    /// Handles the placement of the currently held item based on mouse position, checking grid boundaries and placement rules.
    /// </summary>
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

        // Check if the clicked position is within the grid bounds and if the item can be placed there
        if (!_gridSystem.IsInsideGrid(gridX, gridY) ||
            !_gridSystem.CanPlaceItem(_currentItem, gridX, gridY))
        {
            Debug.Log("Cannot place item here or out of bounds.");
            return;
        }

        // Create and execute the command to place the item
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


    /// <summary>
    /// Handles the visual placement of an item in the grid, instantiating a visual representation at the specified coordinates.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void OnPlaceItemVisual(Item item, int x, int y)
    {
        for (int i = 0; i < item.Width; i++)
        {
            for (int j = 0; j < item.Height; j++)
            {
                int cellX = x + i;
                int cellY = y + j;

                if (!_cellGameObjects.TryGetValue((cellX, cellY), out var cellGO))
                {
                    Debug.LogError($"Cell ({cellX},{cellY}) not found!");
                    continue;
                }

                var itemGO = Instantiate(itemVisualPrefab, cellGO.transform);
                var rt = itemGO.GetComponent<RectTransform>();
                rt.anchoredPosition = Vector2.zero;
                rt.sizeDelta = new Vector2(cellSize, cellSize);

                if (item.Sprite != null && itemGO.TryGetComponent<Image>(out var img))
                    img.sprite = item.Sprite;

                _placedItemVisuals[(cellX, cellY)] = itemGO;
            }
        }
        UpdateScore(item.ScoreValue);
    }

    /// <summary>
    /// Handles the removal of an item visual from the grid, destroying the GameObject and updating the score.
    /// </summary>
    /// <param name="item"></param>
    /// <param name="x"></param>
    /// <param name="y"></param>
    private void OnRemoveItemVisual(Item item, int x, int y)
    {
        for (int i = 0; i < item.Width; i++)
        {
            for (int j = 0; j < item.Height; j++)
            {
                int cellX = x + i;
                int cellY = y + j;

                if (_placedItemVisuals.TryGetValue((cellX, cellY), out var itemGO))
                {
                    Destroy(itemGO);
                    _placedItemVisuals.Remove((cellX, cellY));
                }
            }
        }
        UpdateScore(-item.ScoreValue);
    }



    /// <summary>
    /// Updates the score displayed in the UI by adding the specified delta value to the current score.
    /// </summary>
    /// <param name="delta"></param>
    private void UpdateScore(int delta)
    {
        _score += delta;
        scoreText.text = $"Score: {_score}";
    }
    #endregion
}
