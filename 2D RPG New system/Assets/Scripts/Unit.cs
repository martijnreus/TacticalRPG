using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{
    private OverlayTile currentTile;
    private bool isWalking;

    public OverlayTile GetCurrentTile() { return currentTile; }

    public void SetCurrentTile(OverlayTile tile) { currentTile = tile; }

    public bool GetIsWalking() { return isWalking; }

    public void SetIsWalking(bool value) { isWalking = value; }
}
