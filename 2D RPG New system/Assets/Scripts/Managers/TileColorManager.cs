using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileColorManager : MonoBehaviour
{
    private AttackManager attackManager;

    private void Awake()
    {
        attackManager = GetComponent<AttackManager>();
    }

    public void UpdateAreaOfEffect(List<OverlayTile> oldAreaOfEffect, List<OverlayTile> newAreaOfEffect)
    {
        HideAreaOfEffect(oldAreaOfEffect);
        ShowAreaOfEffect(newAreaOfEffect);
    }

    public void ShowAreaOfEffect(List<OverlayTile> areaOfEffect)
    {
        attackManager.ShowAreaOfEffect(areaOfEffect);
    }

    public void HideAreaOfEffect(List<OverlayTile> areaOfEffect)
    {
        attackManager.HideAreaOfEffect(areaOfEffect);
    }
}
