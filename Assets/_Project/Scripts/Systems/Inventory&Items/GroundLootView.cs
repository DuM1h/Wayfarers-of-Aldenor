using UnityEngine;

public class GroundLootView : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    public Vector2Int GridCoordinates { get; private set; }

    public void Initialize(GroundLoot logicalLoot, Vector3 worldPosition)
    {
        GridCoordinates = logicalLoot.Coordinates;
        _spriteRenderer.sprite = logicalLoot.Item.sprite;
        transform.position = GridManager.GetCellCenterWorld(logicalLoot.Coordinates);
    }
}