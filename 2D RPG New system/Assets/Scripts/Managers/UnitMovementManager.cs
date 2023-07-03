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

    public IEnumerator MoveUnitAlongPath(List<OverlayTile> path) //TODO move to movement script
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
                rangeManager.GetInRangeTiles();
                break;
            }

            yield return new WaitForEndOfFrame();
        }

        unitSelectionManager.GetSelectedUnit().SetIsWalking(false);
        gameManager.state = GameManager.State.normal;
    }

    public void PositionCharacterOnTile(OverlayTile tile)
    {
        unitSelectionManager.GetSelectedUnit().GetCurrentTile().isBlocked = false;
        unitSelectionManager.GetSelectedUnit().transform.position = tile.transform.position;
        unitSelectionManager.GetSelectedUnit().SetCurrentTile(tile);
        unitSelectionManager.GetSelectedUnit().GetCurrentTile().isBlocked = true;
    }
}
