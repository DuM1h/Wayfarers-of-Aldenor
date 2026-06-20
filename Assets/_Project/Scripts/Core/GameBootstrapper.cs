using System.Collections.Generic;
using UnityEngine;

public class GameBootstrapper : MonoBehaviour
{
    [Header("Scene References")]
    [SerializeField] private GridManager _gridManager;
    [SerializeField] private PlayerControllerView _playerView;

    [Header("Test Settings")]
    [SerializeField] private Vector2Int _playerStartPosition = new Vector2Int(0, 0);
    [SerializeField] private Vector2Int _enemyStartPosition = new Vector2Int(0, 0);

    private Character _playerCharacter;
    private List<EnemyBrain> _enemyCharacterList = new List<EnemyBrain>();
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
        _playerCharacter = new Character("Hero", _playerStartPosition, 0, 0, 10);

        // 2. Ініціалізуємо логічного ворога
        // Параметри: Ім'я, Позиція, MaxAP (1), MaxBonus (1), MaxMovement (5), Персонаж гравця
        _enemyCharacterList.Add(new("Enemy", _enemyStartPosition, 1, 1, 5, 5, _playerCharacter));

        // 3. Ініціалізуємо логічний менеджер ходів
        _turnManager = new TurnManager(_playerCharacter, _gridManager.GetGameGrid());
        foreach (var enemy in _enemyCharacterList)
        {
            _turnManager.RegisterCharacter(enemy);
        }

        // Поки що ми стартуємо у вільному режимі, тому ресурси не лімітовані, 
        // але TurnManager вже готовий перевести гру в бій.

        // 4. Зв'язуємо Логіку та Візуал (Передаємо POCO класи у MonoBehaviour)
        _playerView.Initialize(_playerCharacter, _turnManager, _gridManager);

        EnemyView[] enemyViews = FindObjectsByType<EnemyView>();
        for (int i = 0; i < enemyViews.Length; i++)
        {
            enemyViews[i].Initialize(_enemyCharacterList[i], _turnManager, _gridManager);
            _enemyCharacterList[i].SetEnemyView(enemyViews[i]);
            Debug.Log($"Ворог №{i+1} ініціалізований!");
        }

        Debug.Log("<color=green>Системи успішно ініціалізовані! Можна тестувати рух.</color>");

    }

    private void Update()
    {
        _turnManager.Update();
    }
}
