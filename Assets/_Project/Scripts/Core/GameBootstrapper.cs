using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameBootstrapper : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private PlayerControllerView _playerView;
    [SerializeField] private CameraDirector _cameraDirector;
    [SerializeField] private HUD _hud;
    [SerializeField] private InventoryView _inventoryView;
    [SerializeField] private PlayerInputScript _playerInput;

    [Header("Test Settings")]
    [SerializeField] private Vector2Int _playerStartPosition = new Vector2Int(0, 0);
    [SerializeField] private Vector2Int _enemyStartPosition = new Vector2Int(0, 0);
    [SerializeField] private List<ItemConfig> _startItems = new List<ItemConfig>();

    private Character _playerCharacter;
    private List<EnemyBrain> _enemyCharacterList = new List<EnemyBrain>();
    private TurnManager _turnManager;

    private bool _isInitialized = false;

    private void Start()
    {
        if (!ValidateReferences())
            return;

        // 1. Ініціалізуємо логічного персонажа
        // Параметри: Ім'я, Позиція, HP(100), MaxAP (1), MaxBonus (1), MaxMovement (10), InventoryWeight(100)
        _playerCharacter = new Character("Hero", _playerStartPosition, 100, 1, 1, 10, 100);

        // 2. Ініціалізуємо логічний менеджер ходів
        _turnManager = new TurnManager(_playerCharacter, _gridManager.GetGameGrid());

        // Поки що ми стартуємо у вільному режимі, тому ресурси не лімітовані, 
        // але TurnManager вже готовий перевести гру в бій.

        // 3. Зв'язуємо Логіку та Візуал (Передаємо POCO класи у MonoBehaviour)
        _playerView.Initialize(_playerCharacter, _turnManager, _gridManager);
        _cameraDirector.RegisterCharacter(_playerCharacter, _playerView.GetComponent<Transform>());

        EnemyView[] enemyViews = FindObjectsByType<EnemyView>();
        for (int i = 0; i < enemyViews.Length; i++)
        {
            _enemyCharacterList.Add(new("Enemy", _enemyStartPosition, 50, 1, 1, 5, 5, _playerCharacter));
            enemyViews[i].Initialize(_enemyCharacterList[i], _turnManager, _gridManager);
            _cameraDirector.RegisterCharacter(_enemyCharacterList[i], enemyViews[i].GetComponent<Transform>());
            _turnManager.RegisterCharacter(_enemyCharacterList[i]);
            Debug.Log($"Ворог №{i+1} ініціалізований!");
        }

        _inventoryView.Initialize(_playerCharacter.CharacterInventory);

        foreach(var item in _startItems)
            _playerCharacter.CharacterInventory.TryAddItem(item);

        Debug.Log("<color=green>Системи успішно ініціалізовані! Можна тестувати рух.</color>");
        _isInitialized = true;
        _cameraDirector.Initialize();
        _hud.Initialize();

        _inventoryView.OnInventoryToggled += _playerInput.SetInputBlocked;
        _inventoryView.OnItemClicked += HandleInventoryItemClicked;
    }

    private void HandleInventoryItemClicked(ItemConfig item)
    {
        _playerCharacter.TryConsumeItem(item);
    }

    private bool ValidateReferences()
    {
        bool isValid = true;
        if (_gridManager == null)
        {
            Debug.LogError("GameBootstrapper: Не призначенe посилання на GridManager!");
            isValid = false;
        }
        if (_playerView == null)
        {
            Debug.LogError("GameBootstrapper: Не призначене посилання на PlayerView!");
            isValid = false;
        }
        if (_cameraDirector == null)
        {
            Debug.LogError("GameBootstrapper: Не призначене посилання на CameraDirector!");
            isValid = false;
        }
        if (_hud == null)
        {
            Debug.LogError("GameBootstrapper: Не призначене посилання на HUD!");
            isValid = false;
        }
        if (_inventoryView == null)
        {
            Debug.LogError("GameBootstrapper: Не призначене посилання на InventoryView!");
            isValid = false;
        }
        if (_playerInput == null)
        {
            Debug.LogError("GameBootstrapper: Не призначене посилання на PlayerInput!");
            isValid = false;
        }
        return isValid;
    }
    private void Update()
    {
        if (!_isInitialized || _turnManager == null)
            return;
        _turnManager.Update(Time.deltaTime);
    }

    public TurnManager GetTurnManager() { return _turnManager; }

    void OnDestroy() 
    { 
        _isInitialized = false;
        if (_inventoryView != null && _playerInput != null) 
            _inventoryView.OnInventoryToggled -= _playerInput.SetInputBlocked;
    }
}
