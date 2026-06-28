using System.Collections.Generic;
using UnityEngine;

public static class Pathfinder
{
    private class PathNode
    {
        public Vector2Int Position;
        public PathNode Parent;
        public int GCost;
        public int HCost;
        public int FCost => GCost + HCost;
        public PathNode(Vector2Int position)
        {
            Position = position;
        }
    }

    public static Queue<Vector2Int> FindPath(Vector2Int start, Vector2Int target, GameGrid grid, Character character)
    {
        bool isEnemy = character is EnemyBrain;

        Queue<Vector2Int> path = new Queue<Vector2Int>();

        var targetNode = grid.GetNode(target);
        if (targetNode == null || !targetNode.IsWalkable)
            return path;

        List<PathNode> openList = new List<PathNode>();
        HashSet<Vector2Int> closedSet = new HashSet<Vector2Int>();

        PathNode startNode = new PathNode(start) { GCost = 0, HCost = grid.GetDistance(grid.GetNode(start), targetNode) };
        openList.Add(startNode);

        while (openList.Count > 0)
        {
            PathNode currentNode = openList[0];
            for (int i = 1; i < openList.Count; i++)
            {
                if (openList[i].FCost < currentNode.FCost || (openList[i].FCost == currentNode.FCost && openList[i].HCost < currentNode.HCost))
                {
                    currentNode = openList[i];
                }
            }

            openList.Remove(currentNode);
            closedSet.Add(currentNode.Position);

            if (currentNode.Position == target)
            {
                return RetracePath(startNode, currentNode);
            }

            Vector2Int[] directions = { Vector2Int.up, Vector2Int.down, Vector2Int.left, Vector2Int.right };

            foreach (Vector2Int direction in directions)
            {
                Vector2Int neighborPos = currentNode.Position + direction;

                if (closedSet.Contains(neighborPos) || !grid.IsInBounds(neighborPos) || !grid.GetNode(neighborPos).IsWalkable || (grid.GetNode(neighborPos).IsOccupied && !isEnemy))
                    continue;

                int tentativeGCost = currentNode.GCost + 1;

                PathNode neighbourNode = openList.Find(n => n.Position == neighborPos);

                if (neighbourNode == null) {
                    neighbourNode = new PathNode(neighborPos);
                    openList.Add(neighbourNode);
                }
                else if (tentativeGCost >= neighbourNode.GCost)
                {
                    continue;
                }

                neighbourNode.Parent = currentNode;
                neighbourNode.GCost = tentativeGCost;
                neighbourNode.HCost = grid.GetDistance(grid.GetNode(neighborPos), targetNode);
            }
        }

        return path;
    }

    private static Queue<Vector2Int> RetracePath(PathNode startNode, PathNode endNode)
    {
        List<Vector2Int> path = new List<Vector2Int>();
        PathNode currentNode = endNode;
        while (currentNode != startNode)
        {
            path.Add(currentNode.Position);
            currentNode = currentNode.Parent;
        }

        path.Reverse();
        return new Queue<Vector2Int>(path);
    }
}
