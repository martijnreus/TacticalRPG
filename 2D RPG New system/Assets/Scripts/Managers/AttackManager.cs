using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    private UnitSelectionManager unitSelectionManager;

    private void Awake()
    {
        unitSelectionManager = FindObjectOfType<UnitSelectionManager>();
    }

    private List<OverlayTile> GetAreaOfEffect(Spell spell, OverlayTile focusedTile)
    {
        List<OverlayTile> areaOfEffect = new List<OverlayTile>();

        // Get the tiles of effect
        if (spell.spellShape == Spell.SpellShape.cross)
        {
            areaOfEffect = GetCrossShapeTiles(focusedTile, spell.range);
        }

        return areaOfEffect;
    }

    private void CastSpell(Spell spell, List<OverlayTile> areaOfEffect)
    {
        // Loop over each tile to apply effect
        foreach (OverlayTile tile in areaOfEffect)
        {
            // check if the tile contains a enemy
            if (tile.currentUnit != null)
            {
                tile.currentUnit.RemoveHealth(spell.baseDamage);
                Debug.Log("hoi");
            }
        }

        // Remove AP and stuff
        Unit selectedUnit = unitSelectionManager.GetSelectedUnit();
        selectedUnit.RemoveAttackPoints(spell.attackPoints);
    }

    // TODO replace shape functions with a json containing the data
    private List<OverlayTile> GetCrossShapeTiles(OverlayTile focusedTile, int range)
    {
        List<OverlayTile> overlayTileArea = new List<OverlayTile>();
        List<Vector2Int> locationList = new List<Vector2Int>();
        // collect every location that the attack has effect on
        for (int i = 0; i <= range; i++)
        {
            // right
            locationList.Add(new Vector2Int(focusedTile.grid2DLocation.x + i, focusedTile.grid2DLocation.y));

            // left
            locationList.Add(new Vector2Int(focusedTile.grid2DLocation.x - i, focusedTile.grid2DLocation.y));

            // up
            locationList.Add(new Vector2Int(focusedTile.grid2DLocation.x, focusedTile.grid2DLocation.y + i));

            // down
            locationList.Add(new Vector2Int(focusedTile.grid2DLocation.x, focusedTile.grid2DLocation.y - i));
        }

        // Add every valid overlayTile in a list
        foreach(Vector2Int location in locationList)
        {
            OverlayTile tile = MapManager.Instance.map[location];
            if (tile != null)
            {
                overlayTileArea.Add(tile);
            }
        }
     
        return overlayTileArea;
    }
}
