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

    public void GetInRangeTiles()
    {
        // Find in-range tiles for the selected unit
        HideInRangeTiles();

        inRangeTiles = rangeFinder.GetTilesInRange(unitSelectionManager.GetSelectedUnit().GetCurrentTile(), unitSelectionManager.GetSelectedUnit().GetMovementPoints());
        List<OverlayTile> validTiles = new List<OverlayTile>();

        foreach (OverlayTile item in inRangeTiles)
        {
            if (pathfindingManager.pathFinder.FindPath(unitSelectionManager.GetSelectedUnit().GetCurrentTile(), item).Count <= unitSelectionManager.GetSelectedUnit().GetMovementPoints())
            {
                validTiles.Add(item);
            }
        }

        inRangeTiles = validTiles;

        ShowInRangeTiles();
    }

    private void ShowInRangeTiles()
    {
        // Show in-range tiles
        foreach (OverlayTile item in inRangeTiles)
        {
            item.ShowTile();
        }
    }

    private void HideInRangeTiles()
    {
        // Hide in-range tiles
        foreach (OverlayTile item in inRangeTiles)
        {
            item.HideTile();
        }
    }
}
