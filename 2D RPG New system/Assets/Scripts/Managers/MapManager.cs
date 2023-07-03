using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapManager : MonoBehaviour
{
    private static MapManager _instance;

    public static MapManager Instance {get { return _instance; } }

    [SerializeField] private OverlayTile overlayTilePrefab;
    [SerializeField] private GameObject overlayContainer;

    public Dictionary<Vector2Int, OverlayTile> map;

    public Unit[] units;

    private Tilemap tileMap;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void Start()
    {
        tileMap = gameObject.GetComponentInChildren<Tilemap>();
        CreateOverlayTileMap();

        units = FindObjectsOfType<Unit>();
    }

    bool haveRun = false;
    private void Update()
    {
        if (!haveRun)
        {
            SpawnUnits();
        }
    }

    private void SpawnUnits()
    {
        haveRun = true;
        BoundsInt bounds = tileMap.cellBounds;

        foreach (Unit unit in units)
        {
            while (true)
            {
                int randomX = Random.Range(bounds.min.x, bounds.max.x);
                int randomY = Random.Range(bounds.min.y, bounds.max.y);

                if (map.ContainsKey(new Vector2Int(randomX, randomY)))
                {
                    OverlayTile tile = map[new Vector2Int(randomX, randomY)];
                    
                    if (!tile.isBlocked)
                    {
                        tile.isBlocked = true;
                        unit.transform.position = tile.transform.position;
                        unit.SetCurrentTile(tile);

                        break;
                    }
                }
            }
        }
    }

    private void CreateOverlayTileMap()
    {
        map = new Dictionary<Vector2Int, OverlayTile>();
        BoundsInt bounds = tileMap.cellBounds;

        for (int y = bounds.min.y; y < bounds.max.y; y++)
        {
            for (int x = bounds.min.x; x < bounds.max.x; x++)
            {
                Vector3Int tileLocation = new Vector3Int(x, y, 0);
                Vector2Int tileKey = new Vector2Int(x, y);

                if (tileMap.HasTile(tileLocation))
                {
                    OverlayTile overlayTile = Instantiate(overlayTilePrefab, overlayContainer.transform);
                    Vector3 cellWorldPosition = tileMap.GetCellCenterWorld(tileLocation);

                    overlayTile.transform.position = cellWorldPosition;
                    overlayTile.gridLocation = tileLocation;
                    map.Add(tileKey, overlayTile);
                }
            }
        }
    }

    public List<OverlayTile> GetNeighbourTiles(OverlayTile currentOverlayTile, List<OverlayTile> searchableTiles)
    {
        Dictionary<Vector2Int, OverlayTile> tileToSearch = new Dictionary<Vector2Int, OverlayTile>();

        if (searchableTiles.Count > 0)
        {
            foreach (OverlayTile item in searchableTiles)
            {
                tileToSearch.Add(item.grid2DLocation, item);
            }
        }
        else
        {
            tileToSearch = map;
        }

        List<OverlayTile> neighbours = new List<OverlayTile>();

        // Up
        Vector2Int locationToCheck = new Vector2Int(currentOverlayTile.gridLocation.x, currentOverlayTile.gridLocation.y + 1);

        if (tileToSearch.ContainsKey(locationToCheck))
        {
            neighbours.Add(tileToSearch[locationToCheck]);
        }

        // Down
        locationToCheck = new Vector2Int(currentOverlayTile.gridLocation.x, currentOverlayTile.gridLocation.y - 1);

        if (tileToSearch.ContainsKey(locationToCheck))
        {
            neighbours.Add(tileToSearch[locationToCheck]);
        }

        // Right
        locationToCheck = new Vector2Int(currentOverlayTile.gridLocation.x + 1, currentOverlayTile.gridLocation.y);

        if (tileToSearch.ContainsKey(locationToCheck))
        {
            neighbours.Add(tileToSearch[locationToCheck]);
        }

        // Left
        locationToCheck = new Vector2Int(currentOverlayTile.gridLocation.x - 1, currentOverlayTile.gridLocation.y);

        if (tileToSearch.ContainsKey(locationToCheck))
        {
            neighbours.Add(tileToSearch[locationToCheck]);
        }

        return neighbours;
    }
}
