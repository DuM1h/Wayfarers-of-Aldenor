using UnityEngine;
using UnityEngine.Tilemaps;

public class GridManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Tilemap wallsTilemap;

    private Vector3Int _tilemapOriginOffset;
    private int _computedWidth;
    private int _computedHeight;

    private GameGrid _gameGrid;
    public GameGrid GameGrid => _gameGrid;

    private void Awake()
    {
        if (!ValidateReferences()) 
            return;

        InitializeLogicalGrid();
    }

    private bool ValidateReferences()
    {
        bool isValid = true;
        if (wallsTilemap == null)
        {
            Debug.LogError("[GridManager] Не вказано Walls Tilemap! Автовизначення розміру неможливе.");
            isValid = false;
        }
        return isValid;
    }

    private void InitializeLogicalGrid()
    {
        wallsTilemap.CompressBounds();
        BoundsInt bounds = wallsTilemap.cellBounds;

        _computedWidth = bounds.size.x;
        _computedHeight = bounds.size.y;

        _tilemapOriginOffset = bounds.min;

        _gameGrid = new GameGrid(_computedWidth, _computedHeight);

        ParseTilemapObstacles();

        Debug.Log($"[GridManager] Сітку успішно створено! Авто-розмір: {_computedWidth}x{_computedHeight}. Зсув світу Unity: {_tilemapOriginOffset}");
    }

    private void ParseTilemapObstacles()
    {
        for (int x = 0; x < _computedWidth; x++)
        {
            for (int y = 0; y < _computedHeight; y++)
            {
                Vector2Int logicalCoords = new Vector2Int(x, y);

                Vector3Int tilemapCoords = LogicalToUnityCoords(logicalCoords);

                if (wallsTilemap.HasTile(tilemapCoords))
                {
                    Node node = _gameGrid.GetNode(logicalCoords);
                    if (node != null)
                    {
                        node.IsWalkable = false;
                    }
                }
                else
                {
                    Node node = _gameGrid.GetNode(logicalCoords);
                    if (node != null && node.OccupyingCharacter != null)
                    {
                        node.IsWalkable = true;
                    }
                }
            }
        }
    }

    public Vector3Int LogicalToUnityCoords(Vector2Int logicalCoords)
    {
        return new Vector3Int(
            logicalCoords.x + _tilemapOriginOffset.x,
            logicalCoords.y + _tilemapOriginOffset.y,
            0
        );
    }

    public Vector2Int UnityToLogicalCoords(Vector3Int unityCoords)
    {
        return new Vector2Int(
            unityCoords.x - _tilemapOriginOffset.x,
            unityCoords.y - _tilemapOriginOffset.y
        );
    }

    public Vector3 GetCellCenterWorld(Vector2Int logicalCoords)
    {
        Vector3Int unityCoords = LogicalToUnityCoords(logicalCoords);
        return new Vector3(unityCoords.x + 0.5f, unityCoords.y + 0.5f, 0f);
    }

    void OnDrawGizmos()
    {
        if (_gameGrid == null) return;

        for (int x = 0; x < _computedWidth; x++)
        {
            for (int y = 0; y < _computedHeight; y++)
            {
                Node node = _gameGrid.GetNode(new Vector2Int(x, y));
                if (node == null) continue;

                if (!node.IsWalkable)
                {
                    Gizmos.color = Color.red;
                }
                else if (node.IsOccupied)
                {
                    Gizmos.color = Color.yellow;
                }
                else
                {
                    Gizmos.color = Color.gray;
                }

                Vector3Int unityGridPos = LogicalToUnityCoords(new Vector2Int(x, y));
                Vector3 cellCenter = new Vector3(unityGridPos.x + 0.5f, unityGridPos.y + 0.5f, 0);

                Gizmos.DrawWireCube(cellCenter, new Vector3(0.9f, 0.9f, 0));
            }
        }
    }

    public GameGrid GetGameGrid()
    {
        return _gameGrid;
    }

    public void UpdateNodeOccupancy(Vector2Int logicalCoords, Character character)
    {
        Node node = _gameGrid.GetNode(logicalCoords);
        if (node != null)
        {
            node.IsOccupied = character != null;
            node.OccupyingCharacter = character;
        }
    }
}
