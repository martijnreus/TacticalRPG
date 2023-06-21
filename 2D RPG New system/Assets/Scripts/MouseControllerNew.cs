using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseControllerNew : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private GameObject characterPrefab;

    private Unit selectedUnit;
    private Unit[] units;

    private PathFinder pathFinder;
    private RangeFinder rangeFinder;
    private List<OverlayTile> path = new List<OverlayTile>();
    private List<OverlayTile> inRangeTiles = new List<OverlayTile>();

    private OverlayTile targetedOverlayTile;

    private State state;

    private enum State
    {
        normal,
        walking,
        attacking,
    }

    private void Start()
    {
        pathFinder = new PathFinder();
        rangeFinder = new RangeFinder();

        units = FindObjectsOfType<Unit>();
        selectedUnit = units[0];

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
                    FindWalkingPath();

                    if (Input.GetMouseButtonDown(0))
                    {
                        bool hasPlayer = false;
                        // check if a player has been clicked
                        foreach (Unit unit in units)
                        {
                            if (targetedOverlayTile == unit.GetCurrentTile())
                            {
                                hasPlayer = true;
                                selectedUnit = unit;
                            }
                        }

                        if (path.Count != 0 && path.Count <= selectedUnit.GetMovementPoints() && hasPlayer == false)
                        {
                            state = State.walking;
                        }
                    }

                    break;

                case State.walking:

                    if (!selectedUnit.GetIsWalking())
                    {
                        StartCoroutine(MoveUnitAlongPath());
                    }
                    break;

                case State.attacking:

                    state = State.normal; //TODO dont have attacking yet so put it back to normal 
                    break;
            }
        }
    }

    private IEnumerator MoveUnitAlongPath() //TODO move to movement script
    {
        selectedUnit.SetIsWalking(true);

        float step = speed * Time.deltaTime;

        while (true)
        {
            selectedUnit.transform.position = Vector2.MoveTowards(selectedUnit.transform.position, path[0].transform.position, step);

            if (Vector2.Distance(selectedUnit.transform.position, path[0].transform.position) < 0.00001f)
            {
                PositionCharacterOnTile(path[0]);
                path.RemoveAt(0);
                selectedUnit.RemoveMovementPoint();
            }

            if (path.Count == 0)
            {
                GetInRangeTiles();
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        selectedUnit.SetIsWalking(false);
        state = State.normal;
    }

    private void GetInRangeTiles()
    {
        ShowInRangeTiles(false);

        inRangeTiles = rangeFinder.GetTilesInRange(selectedUnit.GetCurrentTile(), 3);
        List<OverlayTile> validTiles = new List<OverlayTile>();

        foreach (OverlayTile item in inRangeTiles)
        {
            if (pathFinder.FindPath(selectedUnit.GetCurrentTile(), item).Count <= selectedUnit.GetMovementPoints())
            {
                validTiles.Add(item);
            }    
        }

        inRangeTiles = validTiles;

        ShowInRangeTiles(true);
    }

    private void ShowInRangeTiles(bool show)
    {
        foreach (OverlayTile item in inRangeTiles)
        {
            if (show)
            {
                item.ShowTile();
            }
            else
            {
                item.HideTile();
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

    private void FindWalkingPath() 
    {
        // The mouse is on a tile the player can walk to
        if (inRangeTiles.Contains(targetedOverlayTile) && !selectedUnit.GetIsWalking())
        {
            path = pathFinder.FindPath(selectedUnit.GetCurrentTile(), targetedOverlayTile, inRangeTiles);

            if (path.Count <= selectedUnit.GetMovementPoints())
            {
                ShowWalkingPathTiles();
            }
        }
        // The player is on a tile out of the walking range
        else
        {
            if (selectedUnit != null && !selectedUnit.GetIsWalking())
            {
                path = new List<OverlayTile>();
            }

            HideWalkingPathTiles();
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
            selectedUnit.GetCurrentTile().ShowWalkingTile();
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

    private void SpawnUnit()
    {
        selectedUnit = Instantiate(characterPrefab).GetComponent<Unit>(); //TODO make this select the player
        PositionCharacterOnTile(targetedOverlayTile);
        GetInRangeTiles();
    }

    private void SelectUnit()
    {
        
    }

    private void PositionCharacterOnTile(OverlayTile tile)
    {
        selectedUnit.GetCurrentTile().isBlocked = false;
        selectedUnit.transform.position = tile.transform.position;
        selectedUnit.SetCurrentTile(tile);
        selectedUnit.GetCurrentTile().isBlocked = true;
    }
}
