using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathFinder
{
    public List<OverlayTile> FindPath(OverlayTile startNode, OverlayTile endNode)
    {
        return FindPath(startNode, endNode, new List<OverlayTile>());
    }

    public List<OverlayTile> FindPath(OverlayTile startNode, OverlayTile endNode, List<OverlayTile> searchableTiles)
    {
        List<OverlayTile> openList = new List<OverlayTile>();
        List<OverlayTile> closeList = new List<OverlayTile>();

        openList.Add(startNode);

        while (openList.Count > 0)
        {
            OverlayTile currentOverlayTile = openList.OrderBy(x => x.FCost).First();

            openList.Remove(currentOverlayTile);
            closeList.Add(currentOverlayTile);

            if (currentOverlayTile == endNode)
            {
                // return path
                return GetPathList(startNode, endNode);
            }

            List<OverlayTile> neighbourTiles = MapManager.Instance.GetNeighbourTiles(currentOverlayTile, searchableTiles);

            foreach (OverlayTile neighbour in neighbourTiles)
            {
                if (neighbour.isBlocked || closeList.Contains(neighbour)) { continue; }

                neighbour.GCost = GetDistance(startNode, neighbour);
                neighbour.HCost = GetDistance(endNode, neighbour);

                neighbour.parent = currentOverlayTile;

                if (!openList.Contains(neighbour))
                {
                    openList.Add(neighbour);
                }
            }
        }

        return new List<OverlayTile>();
    }

    private int GetDistance(OverlayTile startNode, OverlayTile neighbour)
    {
        return Mathf.Abs(startNode.gridLocation.x - neighbour.gridLocation.x) + Mathf.Abs(startNode.gridLocation.y - neighbour.gridLocation.y);
    }

    private List<OverlayTile> GetPathList(OverlayTile startNode, OverlayTile endNode)
    {
        List<OverlayTile> path = new List<OverlayTile>();

        OverlayTile currentTile = endNode;

        while (currentTile != startNode)
        {
            path.Add(currentTile);
            currentTile = currentTile.parent;
        }

        path.Reverse();

        return path;
    }
}
