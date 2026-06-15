using UnityEngine;
using System.Collections.Generic;

public class Node
{
    public Vector2Int Coordinates { get; private set; }
    public bool IsWalkable { get; set; }
    public Node(Vector2Int coordinates, bool isWalkable)
    {
        Coordinates = coordinates;
        IsWalkable = isWalkable;
    }
}

public class GameGrid
{
    public int Width { get; private set; }
    public int Height { get; private set; }

    private Node[,] _grid;

    private static readonly Vector2Int[] Directions = new Vector2Int[]
    {
        new Vector2Int(0, 1),   // Up
        new Vector2Int(1, 0),   // Right
        new Vector2Int(0, -1),  // Down
        new Vector2Int(-1, 0)   // Left
    };
    public GameGrid(int width, int height)
    {
        Width = width;
        Height = height;
        _grid = new Node[width, height];

        InitialazeGrid();
    }

    void InitialazeGrid()
    {
        for (int x = 0; x < Width; x++)
        {
            for (int y = 0; y < Height; y++)
            {
                _grid[x, y] = new Node(new Vector2Int(x, y), true);
            }
        }
    }

    public bool IsInBounds(Vector2Int coordinates)
    {
        return coordinates.x >= 0 && coordinates.x < Width && coordinates.y >= 0 && coordinates.y < Height;
    }

    public Node GetNode(Vector2Int coordinates)
    {
        if (IsInBounds(coordinates))
        {
            return _grid[coordinates.x, coordinates.y];
        }
        return null;
    }

    public int GetDistance(Node a, Node b)
    {
        return Mathf.Abs(a.Coordinates.x - b.Coordinates.x) + Mathf.Abs(a.Coordinates.y - b.Coordinates.y);
    }

    public List<Node> GetNeighbors(Node node)
    {
        List<Node> neighbors = new List<Node>();
        foreach (var direction in Directions)
        {
            Vector2Int neighborCoordinates = node.Coordinates + direction;
            if (IsInBounds(neighborCoordinates))
            {
                Node neighborNode = GetNode(neighborCoordinates);
                if (neighborNode != null && neighborNode.IsWalkable)
                {
                    neighbors.Add(neighborNode);
                }
            }
        }
        return neighbors;
    }
}
