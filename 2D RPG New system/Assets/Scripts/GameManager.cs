using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private GameObject characterPrefab;

    private PathFinder pathFinder;
    private RangeFinder rangeFinder;
    private List<OverlayTile> path = new List<OverlayTile>();
    private List<OverlayTile> inRangeTiles = new List<OverlayTile>();

    private OverlayTile targetedOverlayTile;

    private UnitSelectionManager unitSelectionManager;
    private UnitMovementManager unitMovementManager;

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
    }

    private void Start()
    {
        pathFinder = new PathFinder();
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
                    FindWalkingPath();

                    if (Input.GetMouseButtonDown(0))
                    {
                        bool hasPlayer = unitSelectionManager.DoPlayerSelection(targetedOverlayTile);

                        if (path.Count != 0 && path.Count <= unitSelectionManager.GetSelectedUnit().GetMovementPoints() && hasPlayer == false)
                        {
                            state = State.walking;
                        }
                    }

                    break;

                case State.walking:

                    if (!unitSelectionManager.GetSelectedUnit().GetIsWalking())
                    {
                        Debug.Log(unitMovementManager);
                        StartCoroutine(unitMovementManager.MoveUnitAlongPath(path));
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
        unitSelectionManager.GetSelectedUnit().SetIsWalking(true);

        float step = speed * Time.deltaTime;

        while (true)
        {
            unitSelectionManager.GetSelectedUnit().transform.position = Vector2.MoveTowards(unitSelectionManager.GetSelectedUnit().transform.position, path[0].transform.position, step);

            if (Vector2.Distance(unitSelectionManager.GetSelectedUnit().transform.position, path[0].transform.position) < 0.00001f)
            {
                PositionCharacterOnTile(path[0]);
                path.RemoveAt(0);
                unitSelectionManager.GetSelectedUnit().RemoveMovementPoint();
            }

            if (path.Count == 0)
            {
                GetInRangeTiles();
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        unitSelectionManager.GetSelectedUnit().SetIsWalking(false);
        state = State.normal;
    }

    // Look to which tiles the player can walk
    public void GetInRangeTiles()
    {
        HideInRangeTiles();

        inRangeTiles = rangeFinder.GetTilesInRange(unitSelectionManager.GetSelectedUnit().GetCurrentTile(), unitSelectionManager.GetSelectedUnit().GetMovementPoints());
        List<OverlayTile> validTiles = new List<OverlayTile>();

        foreach (OverlayTile item in inRangeTiles)
        {
            if (pathFinder.FindPath(unitSelectionManager.GetSelectedUnit().GetCurrentTile(), item).Count <= unitSelectionManager.GetSelectedUnit().GetMovementPoints())
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

    private void FindWalkingPath() 
    {
        // The mouse is on a tile the player can walk to
        if (inRangeTiles.Contains(targetedOverlayTile) && !unitSelectionManager.GetSelectedUnit().GetIsWalking())
        {
            path = pathFinder.FindPath(unitSelectionManager.GetSelectedUnit().GetCurrentTile(), targetedOverlayTile, inRangeTiles);

            if (path.Count <= unitSelectionManager.GetSelectedUnit().GetMovementPoints())
            {
                ShowWalkingPathTiles();
            }
        }
        // The mouse is on a tile out of the walking range
        else
        {
            if (unitSelectionManager.GetSelectedUnit() != null && !unitSelectionManager.GetSelectedUnit().GetIsWalking())
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
            unitSelectionManager.GetSelectedUnit().GetCurrentTile().ShowWalkingTile();
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
        Instantiate(characterPrefab).GetComponent<Unit>(); //TODO make this select the player
        PositionCharacterOnTile(targetedOverlayTile);
        GetInRangeTiles();
    }

    private void PositionCharacterOnTile(OverlayTile tile)
    {
        unitSelectionManager.GetSelectedUnit().GetCurrentTile().isBlocked = false;
        unitSelectionManager.GetSelectedUnit().transform.position = tile.transform.position;
        unitSelectionManager.GetSelectedUnit().SetCurrentTile(tile);
        unitSelectionManager.GetSelectedUnit().GetCurrentTile().isBlocked = true;
    }
}
