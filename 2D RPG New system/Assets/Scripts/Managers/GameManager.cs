using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private OverlayTile targetedOverlayTile;

    public State state;

    private UnitSelectionManager unitSelectionManager;
    private UnitMovementManager unitMovementManager;
    private PathfindingManager pathfindingManager;
    private RangeManager rangeManager;

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
        rangeManager = GetComponent<RangeManager>();
    }

    private void Start()
    {
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
                    rangeManager.GetInRangeTiles();

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
