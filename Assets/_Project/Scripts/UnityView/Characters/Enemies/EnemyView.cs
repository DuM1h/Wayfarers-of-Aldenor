using System.Diagnostics;
using UnityEngine;

public class EnemyView : CharacterView
{
    [Header("Animation States")]
    [SerializeField] private CharacterFacingDirection direction;
    [SerializeField] private CharacterAnimationState animationState;

    [Header("Stats")]
    [SerializeField] private int mp;
    [SerializeField] private int ap;
    [SerializeField] private int bp;

    public override void Initialize(Character character, TurnManager turnManager, GridManager gridManager)
    {
        base.Initialize(character, turnManager, gridManager);

        if (character is EnemyBrain brain)
            brain.OnGazeDirectionChanged += SetFacingDirection;
    }
    protected override void Update()
    {
        if (_logicalCharacter == null)
            return;

        mp = _logicalCharacter.AvailableMovementPoints;
        ap = _logicalCharacter.AvailableActionPoints;
        bp = _logicalCharacter.AvailableBonusActions;

        direction = _currentFacingDirection;
        animationState = _currentAnimationState;

        base.Update();
    }

    private void OnDrawGizmos()
    {
        // Перевіряємо, чи ініціалізований наш персонаж і сітка
        if (_logicalCharacter is EnemyBrain enemyBrain && _gridManager != null)
        {
            var path = enemyBrain.CurrentPath;
            if (path == null || path.Count == 0) return;

            // Встановлюємо колір ліній (наприклад, червоний з невеликою прозорістю)
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);

            // Починаємо малювати лінію від поточної логічної позиції ворога
            Vector3 previousPosition = GridManager.GetCellCenterWorld(_logicalCharacter.Position);

            foreach (Vector2Int gridPos in path)
            {
                // Перетворюємо логічну координату у фізичну
                Vector3 targetPosition = GridManager.GetCellCenterWorld(gridPos);

                // Малюємо маленьку сферу на кожній клітинці шляху
                Gizmos.DrawSphere(targetPosition, 0.15f);

                // Малюємо лінію від попередньої точки до цієї
                Gizmos.DrawLine(previousPosition, targetPosition);

                // Зсуваємо "попередню" точку для наступного кроку циклу
                previousPosition = targetPosition;
            }
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        if (_logicalCharacter != null && _logicalCharacter is EnemyBrain brain)
            brain.OnGazeDirectionChanged -= SetFacingDirection;
    }
}
