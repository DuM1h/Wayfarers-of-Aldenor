using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private PlayerControllerView _playerView;

    [Header("Test Settings")]
    [SerializeField] private Vector2Int _playerStartPosition = new Vector2Int(0, 0);

    private Character _playerCharacter;
    private TurnManager _turnManager;

    private void Start()
    {
        if (_gridManager == null || _playerView == null)
        {
            Debug.LogError("GameBootstrapper: Не призначені посилання в інспекторі!");
            return;
        }

        // 1. Ініціалізуємо логічного персонажа
        // Параметри: Ім'я, Позиція, MaxAP (1), MaxBonus (1), MaxMovement (10)
        _playerCharacter = new Character("Hero", _playerStartPosition, 1, 1, 10);

        // 2. Ініціалізуємо логічний менеджер ходів
        _turnManager = new TurnManager(_playerCharacter);

        // Поки що ми стартуємо у вільному режимі, тому ресурси не лімітовані, 
        // але TurnManager вже готовий перевести гру в бій.

        // 3. Зв'язуємо Логіку та Візуал (Передаємо POCO класи у MonoBehaviour)
        _playerView.Initialize(_playerCharacter, _turnManager, _gridManager);

        Debug.Log("<color=green>Системи успішно ініціалізовані! Можна тестувати рух.</color>");

    }
}
