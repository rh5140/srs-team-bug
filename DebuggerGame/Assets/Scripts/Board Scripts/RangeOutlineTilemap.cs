using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor.Tilemaps;
using UnityEngine.Tilemaps;

public class RangeOutlineTilemap : MonoBehaviour
{

    Tilemap tilemap;
    public TileBase outlineTile;
    public int width;
    public int height;

    private bool[,] activeTiles;
    
    private void Start() {
        Tilemap[] maps = FindObjectsOfType<Tilemap>();
        foreach(Tilemap map in maps) {
            if(map.name == "OutlineMap") {
                tilemap = map;
            }
        }

        foreach (Vector3Int tilePosition in tilemap.cellBounds.allPositionsWithin)
        {
            tilemap.RemoveTileFlags(tilePosition, TileFlags.LockColor);
        }

        activeTiles = new bool[width, height];
        for(int i = 0; i < width; i++) {
            for(int j = 0; j < height; j++) {
                DeactivateTile(i,j);
            }
        }
    }

    public void ActivateTile(int x, int y) {
        if(x > width || x < 0 || y > height || y < 0) return;
        activeTiles[x, y] = true;
        tilemap.SetTile(new Vector3Int(x, y, 0), outlineTile);
    }

    public void DeactivateTile(int x, int y) {
        if(x > width || x < 0 || y > height || y < 0) return;
        activeTiles[x, y] = false;
        tilemap.SetTile(new Vector3Int(x, y, 0), null);
    }

    public bool isActivated(int x, int y) {
        return activeTiles[x, y];
    }
}
