using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayTile : MonoBehaviour
{
    [HideInInspector] public int GCost;
    [HideInInspector] public int HCost;
    [HideInInspector] public int FCost { get { return GCost + HCost; } }

    [HideInInspector] public bool isBlocked;

    [HideInInspector] public OverlayTile parent;
    [HideInInspector] public Vector3Int gridLocation;
    [HideInInspector] public Vector2Int grid2DLocation { get { return new Vector2Int(gridLocation.x, gridLocation.y); } }

    [HideInInspector] public Unit unitOnTile;

    public void ShowTile()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0.4f);
    }

    public void HideTile()
    {
        gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
        HideWalkingTile();
    }

    public void ShowWalkingTile()
    {
        SpriteRenderer walkingTile = GetComponentsInChildren<SpriteRenderer>()[1];

        Color currentColor = walkingTile.color;
        walkingTile.color = new Color(currentColor.r, currentColor.g, currentColor.b, 1);
    }

    public void HideWalkingTile()
    {
        SpriteRenderer walkingTile = GetComponentsInChildren<SpriteRenderer>()[1];

        Color currentColor = walkingTile.color;
        walkingTile.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0);
    }
}
