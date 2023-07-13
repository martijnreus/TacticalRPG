using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackManager : MonoBehaviour
{
    private void Update()
    {
        
    }

    private void Attack(Spell spell, OverlayTile focusedTile)
    {
        //if (spell.spellShape == )
    }

    private List<OverlayTile> CrossShape(OverlayTile focusedTile, int range)
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
