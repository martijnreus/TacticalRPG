using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathfindingManager : MonoBehaviour
{
    public PathFinder pathFinder = new PathFinder();
    private List<OverlayTile> path = new List<OverlayTile>();

    private UnitSelectionManager unitSelectionManager;
    private RangeManager rangeManager;

    private void Awake()
    {
        unitSelectionManager = GetComponent<UnitSelectionManager>();
        rangeManager = GetComponent<RangeManager>();
    }

    public List<OverlayTile> GetPath()
    {
        return path;
    }

    public void FindWalkingPath(OverlayTile tile)
    {
        // Find the walking path from the current tile to the targeted tile

        // The mouse is on a tile the player can walk to
        if (rangeManager.inRangeTiles.Contains(tile) && !unitSelectionManager.GetSelectedUnit().GetIsWalking())
        {
            path = pathFinder.FindPath(unitSelectionManager.GetSelectedUnit().GetCurrentTile(), tile, rangeManager.inRangeTiles);

            if (path.Count <= unitSelectionManager.GetSelectedUnit().GetMovementPoints())
            {
                ShowWalkingPathTiles();
            }
        }
        // The mouse is on a tile out of the walking range
        else
        {
            if (unitSelectionManager.GetSelectedUnit() != null && !unitSelectionManager.GetSelectedUnit().GetIsWalking())
            {
                path = new List<OverlayTile>();
            }

            HideWalkingPathTiles();
        }
    }

    private void ShowWalkingPathTiles()
    {
        // Show the tiles along the walking path
        foreach (OverlayTile item in rangeManager.inRangeTiles)
        {
            item.HideWalkingTile();
        }

        if (path.Count > 0)
        {
            unitSelectionManager.GetSelectedUnit().GetCurrentTile().ShowWalkingTile();
        }

        for (int i = 0; i < path.Count; i++)
        {
            path[i].ShowWalkingTile();
        }
    }

    private void HideWalkingPathTiles()
    {
        // Hide the tiles along the walking path
        foreach (OverlayTile item in rangeManager.inRangeTiles)
        {
            item.HideWalkingTile();
        }
    }
}
