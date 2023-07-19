using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeManager : MonoBehaviour
{
    public List<OverlayTile> inRangeTiles = new List<OverlayTile>();
    private RangeFinder rangeFinder = new RangeFinder();

    private UnitSelectionManager unitSelectionManager;
    private PathfindingManager pathfindingManager;

    private void Awake()
    {
        unitSelectionManager = GetComponent<UnitSelectionManager>();
        pathfindingManager = GetComponent<PathfindingManager>();
    }

    public List<OverlayTile> GetInRangeTiles(int range, OverlayTile.TileColors color)
    {
        // Find in-range tiles for the selected unit
        HideInRangeTiles(color);

        inRangeTiles = rangeFinder.GetTilesInRange(unitSelectionManager.GetSelectedUnit().GetCurrentTile(), range);
        List<OverlayTile> validTiles = new List<OverlayTile>();

        foreach (OverlayTile item in inRangeTiles)
        {
            if (item == unitSelectionManager.GetSelectedUnit().GetCurrentTile())
            {
                validTiles.Add(item);
            }

            List<OverlayTile> path = pathfindingManager.pathFinder.FindPath(unitSelectionManager.GetSelectedUnit().GetCurrentTile(), item);
            
            if (path.Count <= range && path.Count != 0)
            {
                validTiles.Add(item);
            }
        }

        inRangeTiles = validTiles;
        ShowInRangeTiles(color);

        return validTiles;
    }

    private void ShowInRangeTiles(OverlayTile.TileColors color)
    {
        // Show in-range tiles
        foreach (OverlayTile item in inRangeTiles)
        {
            item.ShowColor(color);
        }
    }

    public void HideInRangeTiles(OverlayTile.TileColors color)
    {
        // Hide in-range tiles
        foreach (OverlayTile item in inRangeTiles)
        {
            item.HideColor(color);
        }
    }
}
