using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseController : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private GameObject characterPrefab;

    private Unit unit;

    private PathFinder pathFinder;
    private RangeFinder rangeFinder;
    private List<OverlayTile> path = new List<OverlayTile>();
    private List<OverlayTile> inRangeTiles = new List<OverlayTile>();

    private void Start()
    {
        pathFinder = new PathFinder();
        rangeFinder = new RangeFinder();
    }

    private void Update()
    {
        RaycastHit2D? focusedTileUnit = GetFocusedOnTile();

        if (focusedTileUnit.HasValue)
        {
            OverlayTile overlayTile = focusedTileUnit.Value.collider.gameObject.GetComponent<OverlayTile>();
            transform.position = overlayTile.transform.position;

            // The mouse is on a tile the player can walk to
            if (inRangeTiles.Contains(overlayTile) && !unit.GetIsWalking())
            {
                path = pathFinder.FindPath(unit.GetCurrentTile(), overlayTile, inRangeTiles);

                ShowWalkingPathTiles();
            }
            // The player is on a tile out of the walking range
            else
            {
                if (unit != null && !unit.GetIsWalking())
                {
                    path = new List<OverlayTile>();
                }

                HideWalkingPathTiles();
            }

            if (Input.GetMouseButtonDown(0))
            {
                if (unit == null)
                {
                    unit = Instantiate(characterPrefab).GetComponent<Unit>(); //TODO make this select the player
                    PositionCharacterOnTile(overlayTile);
                    GetInRangeTiles();
                }
                else if (!unit.GetIsWalking())
                {
                    if (path.Count != 0)
                    {
                        unit.SetIsWalking(true);
                    }
                }
            }
        }

        if (path.Count > 0 && unit.GetIsWalking())
        {
            MoveAlongPath();
        }
    }

    private void GetInRangeTiles()
    {
        foreach (OverlayTile item in inRangeTiles)
        {
            item.HideTile();
        }

        inRangeTiles = rangeFinder.GetTilesInRange(unit.GetCurrentTile(), 3);

        foreach(OverlayTile item in inRangeTiles)
        {
            item.ShowTile();
        }
    }

    private void MoveAlongPath() //TODO move to movenemnt script
    {
        float step = speed * Time.deltaTime;

        unit.transform.position = Vector2.MoveTowards(unit.transform.position, path[0].transform.position, step);

        if (Vector2.Distance(unit.transform.position, path[0].transform.position) < 0.00001f)
        {
            PositionCharacterOnTile(path[0]);
            path.RemoveAt(0);
        }

        if (path.Count == 0)
        {
            GetInRangeTiles();
            unit.SetIsWalking(false);
        }
    }

    private void ShowWalkingPathTiles() //TODO move to walking visual script
    {
        foreach (OverlayTile item in inRangeTiles)
        {
            item.HideWalkingTile();
        }

        if (path.Count > 0)
        {
            unit.GetCurrentTile().ShowWalkingTile();
        }


        for (int i = 0; i < path.Count; i++)
        {
            path[i].ShowWalkingTile();
        }
    }

    private void HideWalkingPathTiles() //TODO move to walking visual script
    {
        foreach (OverlayTile item in inRangeTiles)
        {
            item.HideWalkingTile();
        }
    }

    public RaycastHit2D? GetFocusedOnTile()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

        if (hit == true)
        {
            return hit;
        }

        return null;
    }

    private void PositionCharacterOnTile(OverlayTile tile)
    {
        unit.GetCurrentTile().isBlocked = false;
        unit.transform.position = tile.transform.position;
        unit.SetCurrentTile(tile);
        unit.GetCurrentTile().isBlocked = true;
    }
}
