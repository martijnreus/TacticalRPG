using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayTile : MonoBehaviour
{
    [HideInInspector] public PathNode pathNode = new PathNode();

    // Tile information
    [HideInInspector] public bool isBlocked;
    [HideInInspector] public Vector3Int gridLocation;
    [HideInInspector] public Vector2Int grid2DLocation { get { return new Vector2Int(gridLocation.x, gridLocation.y); } }
    [HideInInspector] public Unit unitOnTile;

    // Tile color
    [SerializeField] private Color[] colors;
    [SerializeField] private SpriteRenderer spriteRenderer;
    private List<TileColors> colorsToShow  = new List<TileColors>();
    private Dictionary<TileColors, Color> colorDictionary = new Dictionary<TileColors, Color>();

    public enum TileColors
    {
        white,
        red,
        blue,
        green,
    }

    private void Awake()
    {
        colorDictionary.Add(TileColors.white, colors[0]);
        colorDictionary.Add(TileColors.red, colors[1]);
        colorDictionary.Add(TileColors.blue, colors[2]);
        colorDictionary.Add(TileColors.green, colors[3]);
    }

    public void HideColor(TileColors color)
    {
        colorsToShow.Remove(color);
        SetColor();
    }

    public void ShowColor(TileColors color)
    {
        colorsToShow.Add(color);
        SetColor();
    }

    private void SetColor()
    {
        // Turn all the TileColors into actual colors
        List<Color> actualColorsToShow = new List<Color>();
        foreach(TileColors color in colorsToShow)
        {
            actualColorsToShow.Add(colorDictionary[color]);
        }

        // Mix all colors into one
        Color resultColor = new Color(0, 0, 0, 0);
        foreach (Color color in actualColorsToShow)
        {
            resultColor += color;
        }

        if (actualColorsToShow.Count != 0)
        {
            resultColor /= actualColorsToShow.Count;
        }

        spriteRenderer.color = resultColor;
    }
}
