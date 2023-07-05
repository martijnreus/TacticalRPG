using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private int movementPoints;
    [SerializeField] private bool isEnemy;

    private OverlayTile currentTile;
    private bool isWalking;

    private int saveMovementPoints;

    private void Start()
    {
        saveMovementPoints = movementPoints;
    }

    public OverlayTile GetCurrentTile() { return currentTile; }

    public void SetCurrentTile(OverlayTile tile) { currentTile = tile; }

    public bool GetIsWalking() { return isWalking; }

    public void SetIsWalking(bool value) { isWalking = value; }

    public int GetMovementPoints(){ return movementPoints; }

    public void RemoveMovementPoint() { movementPoints -= 1; }

    public bool GetIsEnemy() { return isEnemy; }

    public void ResetMovementPoints() { movementPoints = saveMovementPoints; }
}
