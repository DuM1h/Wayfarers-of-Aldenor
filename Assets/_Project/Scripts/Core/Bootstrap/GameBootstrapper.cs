using System.Collections.Generic;
using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private PlayerControllerView _playerView;
    [SerializeField] private CameraDirector _cameraDirector;
    [SerializeField] private HUD _hud;
    [SerializeField] private InventoryView _inventoryView;
    [SerializeField] private PlayerInputScript _playerInput;
    [SerializeField] private EquipmentPanelView _equipmentPanelView;

    [Header("Test Settings")]
    [SerializeField] private Vector2Int _playerStartPosition = new Vector2Int(0, 0);
    [SerializeField] private Vector2Int _enemyStartPosition = new Vector2Int(0, 0);
    [SerializeField] private List<ItemConfig> _startItems = new List<ItemConfig>();
    [SerializeField] private GroundLootView _lootPrefab;

    [Header("Player Stats")]
    [SerializeField] private int _playerMaxHealth = 100;
    [SerializeField] private int _playerArmor = 0;
    [SerializeField] private int _playerBaseDamage = 25;
    [SerializeField] private float _playerMaxWeight = 100f;

    [Header("Enemy Stats")]
    [SerializeField] private int _enemyMaxHealth = 50;
    [SerializeField] private int _enemyArmor = 0;
    [SerializeField] private int _enemyBaseDamage = 20;
    [SerializeField] private float _enemyMaxWeight = 15f;

    private PlayerCharacter _playerCharacter;
    private List<EnemyBrain> _enemyCharacterList = new List<EnemyBrain>();
    private TurnManager _turnManager;
    private LootManager _lootManager;
    private List<GroundLootView> _spawnedLootViews = new List<GroundLootView>();

    private bool _isInitialized = false;

    private void Start()
    {
        if (!ValidateReferences())
            return;

        // 1. Ініціалізуємо логічного персонажа
        // Параметри: Ім'я, Позиція, Stats, MaxAP (1), MaxBonus (1), MaxMovement (10)
        var playerStats = new CharacterStats(_playerMaxHealth, _playerMaxHealth, _playerArmor, _playerBaseDamage, _playerMaxWeight);
        _playerCharacter = new PlayerCharacter("Hero", _playerStartPosition, playerStats, 1, 1, 10);

        // 2. Ініціалізуємо логічний менеджер ходів
        _turnManager = new TurnManager(_playerCharacter, _gridManager.GetGameGrid());
        _lootManager = new LootManager();
        _lootManager.OnLootSpawned += (logicalLoot) => {
            Vector3 lootCords = GridManager.LogicalToUnityCoords(logicalLoot.Coordinates);
            GroundLootView view = Instantiate(_lootPrefab, lootCords, Quaternion.identity);
            view.Initialize(logicalLoot, lootCords);
            _spawnedLootViews.Add(view);
        };
        _lootManager.OnLootRemoved += (coords) => {
            var view = _spawnedLootViews.Find(v => v.GridCoordinates == coords);
            if (view != null)
            {
                Destroy(view.gameObject);
                _spawnedLootViews.Remove(view);
            }
        };

        // Спавним тестову моркву на клітинці (7, 3)
        _lootManager.SpawnLoot(new Vector2Int(7, 3), _startItems[2], 3);

        // Поки що ми стартуємо у вільному режимі, тому ресурси не лімітовані, 
        // але TurnManager вже готовий перевести гру в бій.

        // 3. Зв'язуємо Логіку та Візуал (Передаємо POCO класи у MonoBehaviour)
        _playerView.Initialize(_playerCharacter, _turnManager, _gridManager, _lootManager);
        _cameraDirector.RegisterCharacter(_playerCharacter, _playerView.GetComponent<Transform>());

        EnemyView[] enemyViews = FindObjectsByType<EnemyView>();
        for (int i = 0; i < enemyViews.Length; i++)
        {
            var enemyStats = new CharacterStats(_enemyMaxHealth, _enemyMaxHealth, _enemyArmor, _enemyBaseDamage, _enemyMaxWeight);
            _enemyCharacterList.Add(new EnemyBrain("Enemy", _enemyStartPosition, enemyStats, 1, 1, 5, 5, _playerCharacter));
            enemyViews[i].Initialize(_enemyCharacterList[i], _turnManager, _gridManager);
            _cameraDirector.RegisterCharacter(_enemyCharacterList[i], enemyViews[i].GetComponent<Transform>());
            _turnManager.RegisterCharacter(_enemyCharacterList[i]);
            _enemyCharacterList[i].Stats.OnDied += () =>
            {
                _turnManager.UnregisterCharacter(_enemyCharacterList[i]);
                _enemyCharacterList.Remove(_enemyCharacterList[i]);
            };
            _enemyCharacterList[i].OnCharacterDied += (coords) =>
            {
                _turnManager.ReleaseNode(coords);
            };
            Debug.Log($"Ворог №{i+1} ініціалізований!");
        }

        _inventoryView.Initialize(_playerCharacter.CharacterInventory);

        foreach(var item in _startItems)
            _playerCharacter.CharacterInventory.TryAddItem(item, 1);

        Debug.Log("<color=green>Системи успішно ініціалізовані! Можна тестувати рух.</color>");
        _isInitialized = true;
        _cameraDirector.Initialize();
        _hud.Initialize();

        _inventoryView.OnInventoryToggled += _playerInput.SetInputBlocked;
        _inventoryView.OnItemClicked += HandleInventoryItemClicked;

        _equipmentPanelView.Init(_playerCharacter);
    }

    private void HandleInventoryItemClicked(ItemConfig item)
    {
        _playerCharacter.UseOrEquipItem(item);
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
