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

    public List<OverlayTile> GetInRangeTiles(int range)
    {
        // Find in-range tiles for the selected unit
        HideInRangeTiles();

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
        ShowInRangeTiles();

        return validTiles;
    }

    private void ShowInRangeTiles()
    {
        // Show in-range tiles
        foreach (OverlayTile item in inRangeTiles)
        {
            item.ShowColor(OverlayTile.TileColors.white);
        }
    }

    public void HideInRangeTiles()
    {
        // Hide in-range tiles
        foreach (OverlayTile item in inRangeTiles)
        {
            item.HideColor(OverlayTile.TileColors.white);
        }
    }
}
