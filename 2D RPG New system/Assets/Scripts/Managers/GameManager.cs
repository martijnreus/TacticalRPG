using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private OverlayTile targetedOverlayTile;

    public State state;
    public Team team;

    private UnitSelectionManager unitSelectionManager;
    private UnitMovementManager unitMovementManager;
    private PathfindingManager pathfindingManager;
    private TileColorManager tileColorManager;
    private RangeManager rangeManager;
    private AttackManager attackManager;

    private List<Unit> playerTeam = new List<Unit>();
    private List<Unit> enemyTeam = new List<Unit>();

    private float timer = 3f;

    private List<OverlayTile> areaOfEffect = new List<OverlayTile>();
    private List<OverlayTile> inRangeTiles = new List<OverlayTile>();
    private List<OverlayTile> pathToWalk = new List<OverlayTile>();

    [SerializeField] private Spell spell; //TODO only temporary couple this to the unit

    public enum Team
    {
        player,
        enemy,
    }

    public enum State
    {
        waiting,
        normal,
        walking,
        attacking,
    }

    private void Awake()
    {
        unitSelectionManager = GetComponent<UnitSelectionManager>();
        unitMovementManager = GetComponent<UnitMovementManager>();
        pathfindingManager = GetComponent<PathfindingManager>();
        tileColorManager = GetComponent<TileColorManager>();
        rangeManager = GetComponent<RangeManager>();
        attackManager = GetComponent<AttackManager>();
    }

    private void Start()
    {
        state = State.waiting;

        AssignTeams();
    }

    private void Update()
    {
        targetedOverlayTile = GetFocusedOnTile();
        SetCursorPosition(targetedOverlayTile);

        switch (team)
        {
            case Team.player:
                switch (state)
                {
                   
                    case State.waiting:
                        state = State.normal;
                        break;

                    case State.normal: //TODO here you should be able to select players or start walking or attacking
                        rangeManager.GetInRangeTiles(unitSelectionManager.GetSelectedUnit().GetMovementPoints(), OverlayTile.TileColors.white);

                        // When the cursor is hovering find a path to the cursor and showcase if valid
                        pathfindingManager.FindWalkingPath(targetedOverlayTile);

                        if (Input.GetMouseButtonDown(0))
                        {
                            bool hasPlayer = unitSelectionManager.DoPlayerSelection(targetedOverlayTile);

                            if (pathfindingManager.GetPath().Count != 0 && pathfindingManager.GetPath().Count <= unitSelectionManager.GetSelectedUnit().GetMovementPoints() && hasPlayer == false)
                            {
                                StartWalking();
                            }
                        }

                        if (Input.GetKeyDown(KeyCode.K))
                        {
                            StartAttacking();
                        }

                        if (Input.GetKeyDown(KeyCode.P))
                        {
                            EndTurn();
                        }

                        break;

                    case State.walking:
                        // start walking when in this state
                        if (pathfindingManager.GetPath().Count != 0)
                        {
                            unitMovementManager.MoveUnitOverPath(pathfindingManager.GetPath());
                        }
                        else
                        {
                            StopWalking();
                        }
                        
                        break;

                    case State.attacking:
                        if (inRangeTiles.Contains(targetedOverlayTile))
                        {
                            // Show the areaOfEffect visuals
                            List<OverlayTile> oldAreaOfEffect = areaOfEffect;
                            areaOfEffect = attackManager.GetAreaOfEffect(spell, targetedOverlayTile);
                            tileColorManager.UpdateAreaOfEffect(oldAreaOfEffect, areaOfEffect);
                        }
                        else
                        {
                            // Hide the areaOfEffect
                            tileColorManager.HideAreaOfEffect(areaOfEffect);
                        }
                        
                        // Cast spell
                        if (Input.GetMouseButtonDown(0))
                        {
                            attackManager.CastSpell(spell, areaOfEffect);
                            StopAttacking();
                        }

                        if (Input.GetKeyDown(KeyCode.K))
                        {
                            StopAttacking();
                        }

                        break;
                }

                break;

            case Team.enemy:
                // Run enemy AI now wait 3 seconds to return to player turn

                if (timer > 0) 
                {
                    timer -= Time.deltaTime;
                }
                else
                {
                    team = Team.player;
                    timer = 3f;
                }
                break;
        }
    }

    private OverlayTile GetFocusedOnTile()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        return MapManager.Instance.GetOverlayTile(mousePos);
    }

    private void SetCursorPosition(OverlayTile tile)
    {
        if (tile != null)
        {
            // Move mouse cursor to the focusedTilePosition
            transform.position = tile.transform.position;
        }
        else
        {
            // Set cursor out of frame
            transform.position = new Vector3(999, 999, 999);
        }
    }

    private void AssignTeams()
    {
        Unit[] units = FindObjectsOfType<Unit>();

        foreach (Unit unit in units)
        {
            if (!unit.GetIsEnemy())
            {
                playerTeam.Add(unit);
            }
            else
            {
                enemyTeam.Add(unit);
            }
        }
    }

    public void EndTurn()
    {
        foreach(Unit unit in playerTeam)
        {
            unit.ResetMovementPoints();
        }

        //End turn
        team = Team.enemy;
    }

    public void StartAttacking()
    {
        inRangeTiles = rangeManager.GetInRangeTiles(spell.range, OverlayTile.TileColors.blue);
        unitMovementManager.HidePathColor(pathfindingManager.GetPath());
        rangeManager.HideInRangeTiles(OverlayTile.TileColors.white);
        state = State.attacking;
    }

    public void StopAttacking()
    {
        tileColorManager.HideAreaOfEffect(areaOfEffect);
        rangeManager.HideInRangeTiles(OverlayTile.TileColors.blue);
        state = State.normal;
    }

    public void StartWalking()
    {
        pathToWalk = new List<OverlayTile>(pathfindingManager.GetPath());
        pathToWalk.Add(unitSelectionManager.GetSelectedUnit().GetCurrentTile());
        state = State.walking;
    }

    public void StopWalking()
    {
        unitMovementManager.HidePathColor(pathToWalk);
        unitSelectionManager.GetSelectedUnit().SetIsWalking(false);
        state = State.normal;
    }
}
