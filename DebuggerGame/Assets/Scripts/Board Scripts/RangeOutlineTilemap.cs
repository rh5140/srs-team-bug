using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Tilemaps;
using UnityEngine.Tilemaps;

public class RangeOutlineTilemap : MonoBehaviour
{

    Tilemap tilemap;
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
        activeTiles[x, y] = true;

    }

    public void DeactivateTile(int x, int y) {
        activeTiles[x, y] = false;
    }

    public bool isActivated(int x, int y) {
        return activeTiles[x, y];
    }
}
