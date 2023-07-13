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
    private RangeManager rangeManager;
    private AttackManager attackManager;

    private List<Unit> playerTeam = new List<Unit>();
    private List<Unit> enemyTeam = new List<Unit>();

    private float timer = 3f;
    private bool hasSelectedAttackTile;
    private OverlayTile selectedAttackTile;
    private List<OverlayTile> areaOfEffect;

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
                        rangeManager.GetInRangeTiles(unitSelectionManager.GetSelectedUnit().GetMovementPoints());

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
                        if (!unitSelectionManager.GetSelectedUnit().GetIsWalking())
                        {
                            StartCoroutine(unitMovementManager.MoveUnitAlongPath(pathfindingManager.GetPath()));
                        }
                        break;

                    case State.attacking:

                        // Show what tiles can be hit
                        List<OverlayTile> inRangeTiles = rangeManager.GetInRangeTiles(spell.range);
                        
                        if (Input.GetMouseButtonDown(0))
                        {
                            // show what the area of effect will be
                            if (inRangeTiles.Contains(targetedOverlayTile) && targetedOverlayTile != selectedAttackTile)
                            {
                                areaOfEffect = attackManager.GetAreaOfEffect(spell, targetedOverlayTile); //TODO show this on screen
                                selectedAttackTile = targetedOverlayTile;
                                hasSelectedAttackTile = true;
                            }
                            // cast spell
                            else if (hasSelectedAttackTile == true && targetedOverlayTile == selectedAttackTile)
                            {
                                attackManager.CastSpell(spell, areaOfEffect);
                                hasSelectedAttackTile = false;
                                selectedAttackTile = null;
                                state = State.normal;
                            }   
                        }

                        if (Input.GetKeyDown(KeyCode.K))
                        {
                            state = State.normal;
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
        rangeManager.HideInRangeTiles();
        state = State.attacking;
    }
}
