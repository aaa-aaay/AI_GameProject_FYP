using System.Collections.Generic;
using UnityEngine;

public static class AStar
{
    public static List<PathNode> FindPath(PathNode start, PathNode goal)
    {
        var openSet = new List<PathNode> { start };
        var cameFrom = new Dictionary<PathNode, PathNode>();
        var gScore = new Dictionary<PathNode, float>();
        var fScore = new Dictionary<PathNode, float>();

        gScore[start] = 0;
        fScore[start] = Heuristic(start, goal);

        while (openSet.Count > 0)
        {
            PathNode current = openSet[0];
            foreach (var node in openSet)
                if (fScore.ContainsKey(node) && fScore[node] < fScore[current])
                    current = node;

            if (current == goal)
                return ReconstructPath(cameFrom, current);

            openSet.Remove(current);

            foreach (var neighbor in current.neighbors)
            {
                float tentativeG = gScore[current] + Vector3.Distance(current.transform.position, neighbor.transform.position);

                if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
                {
                    cameFrom[neighbor] = current;
                    gScore[neighbor] = tentativeG;
                    fScore[neighbor] = tentativeG + Heuristic(neighbor, goal);

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return new List<PathNode>(); // no path found
    }

    private static float Heuristic(PathNode a, PathNode b)
    {
        return Vector3.Distance(a.transform.position, b.transform.position);
    }

    private static List<PathNode> ReconstructPath(Dictionary<PathNode, PathNode> cameFrom, PathNode current)
    {
        List<PathNode> totalPath = new List<PathNode> { current };
        while (cameFrom.ContainsKey(current))
        {
            current = cameFrom[current];
            totalPath.Insert(0, current);
        }
        return totalPath;
    }
}
