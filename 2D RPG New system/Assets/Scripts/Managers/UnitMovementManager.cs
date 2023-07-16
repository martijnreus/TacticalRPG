using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitMovementManager : MonoBehaviour
{
    [SerializeField] private float speed;

    private GameManager gameManager;
    private UnitSelectionManager unitSelectionManager;
    private RangeManager rangeManager;

    private void Awake()
    {
        unitSelectionManager = GetComponent<UnitSelectionManager>();
        gameManager = GetComponent<GameManager>();
        rangeManager = GetComponent<RangeManager>();
    }

    public IEnumerator MoveUnitAlongPath(List<OverlayTile> path)
    {
        unitSelectionManager.GetSelectedUnit().SetIsWalking(true);

        List<OverlayTile> pathCopy = new List <OverlayTile>(path);
        pathCopy.Add(unitSelectionManager.GetSelectedUnit().GetCurrentTile());

        while (true)
        {
            float step = speed * Time.deltaTime;

            unitSelectionManager.GetSelectedUnit().transform.position = Vector2.MoveTowards(unitSelectionManager.GetSelectedUnit().transform.position, path[0].transform.position, step);

            if (Vector2.Distance(unitSelectionManager.GetSelectedUnit().transform.position, path[0].transform.position) < 0.00001f)
            {
                PositionCharacterOnTile(path[0]);
                path.RemoveAt(0);
                unitSelectionManager.GetSelectedUnit().RemoveMovementPoints();
            }

            if (path.Count == 0)
            {
                rangeManager.GetInRangeTiles(unitSelectionManager.GetSelectedUnit().GetMovementPoints());
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        HidePathColor(pathCopy);

        unitSelectionManager.GetSelectedUnit().SetIsWalking(false);
        gameManager.state = GameManager.State.normal;
    }

    public void PositionCharacterOnTile(OverlayTile tile)
    {
        // Reset current Tile
        unitSelectionManager.GetSelectedUnit().GetCurrentTile().unitOnTile = null;
        unitSelectionManager.GetSelectedUnit().GetCurrentTile().isBlocked = false;

        // Set new tile
        unitSelectionManager.GetSelectedUnit().transform.position = tile.transform.position;
        unitSelectionManager.GetSelectedUnit().SetCurrentTile(tile);
        unitSelectionManager.GetSelectedUnit().GetCurrentTile().isBlocked = true;
        tile.unitOnTile = unitSelectionManager.GetSelectedUnit();
    }

    private void HidePathColor(List<OverlayTile> pathCopy)
    {
        unitSelectionManager.GetSelectedUnit().GetCurrentTile().HideColor(OverlayTile.TileColors.green);
        foreach (OverlayTile tile in pathCopy)
        {
            tile.HideColor(OverlayTile.TileColors.green);
        }
    }
}
