using UnityEngine;

public class EnemyView : CharacterView
{
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
            Vector3 previousPosition = _gridManager.GetCellCenterWorld(_logicalCharacter.Position);

            foreach (Vector2Int gridPos in path)
            {
                // Перетворюємо логічну координату у фізичну
                Vector3 targetPosition = _gridManager.GetCellCenterWorld(gridPos);

                // Малюємо маленьку сферу на кожній клітинці шляху
                Gizmos.DrawSphere(targetPosition, 0.15f);

                // Малюємо лінію від попередньої точки до цієї
                Gizmos.DrawLine(previousPosition, targetPosition);

                // Зсуваємо "попередню" точку для наступного кроку циклу
                previousPosition = targetPosition;
            }
        }
    }
}
