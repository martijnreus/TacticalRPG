using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private RangeFinder rangeFinder;

    public List<OverlayTile> inRangeTiles = new List<OverlayTile>();

    private OverlayTile targetedOverlayTile;

    private UnitSelectionManager unitSelectionManager;
    private UnitMovementManager unitMovementManager;
    private PathfindingManager pathfindingManager;

    public State state;

    public enum State
    {
        normal,
        walking,
        attacking,
    }

    private void Awake()
    {
        unitSelectionManager = GetComponent<UnitSelectionManager>();
        unitMovementManager = GetComponent<UnitMovementManager>();
        pathfindingManager = GetComponent<PathfindingManager>();
    }

    private void Start()
    {
        rangeFinder = new RangeFinder();

        state = State.normal;
    }

    private void Update()
    {
        RaycastHit2D? focusedTileUnit = GetFocusedOnTile();

        if (focusedTileUnit.HasValue)
        {
            targetedOverlayTile = focusedTileUnit.Value.collider.gameObject.GetComponent<OverlayTile>();
            // Move mouse cursor to the focusedTilePositiom
            transform.position = targetedOverlayTile.transform.position;

            switch (state)
            {
                case State.normal: //TODO here you should be able to select players or start walking or attacking
                    GetInRangeTiles();

                    // When the cursor is hovering find a path to the cursor and showcase if valid
                    pathfindingManager.FindWalkingPath(targetedOverlayTile);

                    if (Input.GetMouseButtonDown(0))
                    {
                        bool hasPlayer = unitSelectionManager.DoPlayerSelection(targetedOverlayTile);

                        if (pathfindingManager.GetPath().Count != 0 && pathfindingManager.GetPath().Count <= unitSelectionManager.GetSelectedUnit().GetMovementPoints() && hasPlayer == false)
                        {
                            state = State.walking;
                        }
                    }

                    break;

                case State.walking:

                    if (!unitSelectionManager.GetSelectedUnit().GetIsWalking())
                    {
                        StartCoroutine(unitMovementManager.MoveUnitAlongPath(pathfindingManager.GetPath()));
                    }
                    break;

                case State.attacking:

                    state = State.normal; //TODO dont have attacking yet so put it back to normal 
                    break;
            }
        }
    }

    // Look to which tiles the player can walk
    public void GetInRangeTiles()
    {
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

    // Shows all the tiles that are in the inRangeTiles
    private void ShowInRangeTiles()
    {
        foreach (OverlayTile item in inRangeTiles)
        {
            item.ShowTile();
        }
    }

    // Hide all the tiles that are in the inRangeTiles
    private void HideInRangeTiles()
    {
        foreach (OverlayTile item in inRangeTiles)
        {
            item.HideTile();
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
}
