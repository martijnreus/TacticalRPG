using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private int movementPoints;

    private OverlayTile currentTile;
    private bool isWalking;

    public OverlayTile GetCurrentTile() { return currentTile; }

    public void SetCurrentTile(OverlayTile tile) { currentTile = tile; }

    public bool GetIsWalking() { return isWalking; }

    public void SetIsWalking(bool value) { isWalking = value; }

    public int GetMovementPoints(){ return movementPoints; }

    public void RemoveMovementPoint() { movementPoints -= 1; }
}
