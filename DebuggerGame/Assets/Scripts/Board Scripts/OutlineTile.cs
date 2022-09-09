using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Tilemaps;
using UnityEngine.Tilemaps;

public class OutlineTile : Tile {


    public override void RefreshTile(Vector3Int position, ITilemap tilemap) {

    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
        base.GetTileData(position, tilemap, ref tileData);
        Color myColor = new Color(1.0f, 0f, 0f, 0f);
        tileData.color = myColor;
    }
}
