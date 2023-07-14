using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathNode
{
    // Pathfinding stuff
    public int GCost;
    public int HCost;
    public int FCost { get { return GCost + HCost; } }
    public OverlayTile parent;
}
