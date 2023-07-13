using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private int movementPoints;
    [SerializeField] private int attackPoints;
    [SerializeField] private bool isEnemy;
    [SerializeField] private int health;

    private OverlayTile currentTile;
    private bool isWalking;

    private int saveMovementPoints;
    private int saveAttackPoints;

    private void Start()
    {
        saveMovementPoints = movementPoints;
        saveAttackPoints = attackPoints;
    }

    public OverlayTile GetCurrentTile() { return currentTile; }

    public void SetCurrentTile(OverlayTile tile) { currentTile = tile; }

    public bool GetIsWalking() { return isWalking; }

    public void SetIsWalking(bool value) { isWalking = value; }

    public int GetMovementPoints(){ return movementPoints; }

    public void RemoveMovementPoints() { movementPoints -= 1; }

    public void ResetMovementPoints() { movementPoints = saveMovementPoints; }

    public bool GetIsEnemy() { return isEnemy; }

    public void RemoveHealth(int value) { health -= value; }

    public int GetHealth() { return health; }

    public int GetAttackPoints() { return attackPoints; }

    public void RemoveAttackPoints(int value) { attackPoints -= value; }

    public void ResetAttackPoints() { attackPoints = saveAttackPoints; }
}
