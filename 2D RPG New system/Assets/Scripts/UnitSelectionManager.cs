using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectionManager : MonoBehaviour
{
    private Unit selectedUnit;
    private Unit[] units;

    private MapManager mapManager;

    private void Start()
    {
        units = FindObjectsOfType<Unit>();
        selectedUnit = units[0];
    }

    public bool DoPlayerSelection(OverlayTile tile)
    {
        Unit newUnit = GetUnitAtTile(tile);

        // select a player when clicked on
        if (newUnit != null)
        {
            SelectUnit(newUnit);
            return true;
        }

        return false;
    }

    private Unit GetUnitAtTile(OverlayTile tile)
    {
        // Get the unit at the specified tile
        //return tile.unitOnTile;

        // check if a player has been clicked and return true if there is a new player selected
        foreach (Unit unit in units)
        {
            if (tile == unit.GetCurrentTile())
            {
                return unit;
            }
        }
        return null;
    }

    private void SelectUnit(Unit unit)
    {
        selectedUnit = unit;
    }

    public Unit GetSelectedUnit()
    {
        return selectedUnit;
    }
}
