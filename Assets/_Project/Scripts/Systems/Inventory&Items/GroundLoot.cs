using UnityEngine;

public class GroundLoot
{
    public Vector2Int Coordinates { get; private set; }
    public ItemConfig Item { get; private set; }
    public int Amount { get; private set; }

    public GroundLoot(Vector2Int coordinates, ItemConfig item, int amount)
    {
        Coordinates = coordinates;
        Item = item;
        Amount = amount;
    }
}