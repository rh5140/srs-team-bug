using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Creates a grid texture, adds it to a sprite renderer, and draws it according to the current board boundaries.
/// Automatically disabled for standalone builds, but, to disable in the editor, just disable the gameobject
/// </summary>
public class DebugGrid : MonoBehaviour
{
    [SerializeField]
    private int textureWidth = 32;

    [SerializeField]
    private Color textureColor = new Color(0f, 0f, 0f, 0.5f);

    void Start()
    {
#if UNITY_EDITOR
        SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
        if(spriteRenderer == null)
        {
            spriteRenderer = gameObject.AddComponent<SpriteRenderer>();
        }

        Texture2D texture = new Texture2D(textureWidth, textureWidth);
        for(int x = 0; x < textureWidth; x++)
        {
            for(int y = 0; y < textureWidth; y++)
            {
                Color color = (
                    x == 0 || y == 0
                    || x == textureWidth - 1
                    || y == textureWidth - 1
                ) ? textureColor : Color.clear;

                texture.SetPixel(
                    x,
                    y,
                    color
                );
            }
        }

        texture.filterMode = FilterMode.Point;
        texture.Apply();

        spriteRenderer.sprite = Sprite.Create(
            texture,
            rect: new Rect(0, 0, texture.width, texture.height),
            pivot: new Vector2(0, 0),
            pixelsPerUnit: 32,
            extrude: 0,
            meshType: SpriteMeshType.FullRect
        );

        spriteRenderer.drawMode = SpriteDrawMode.Tiled;

        Board board = FindObjectOfType<Board>();
        
        spriteRenderer.size = new Vector2(board.width, board.height);
        transform.position = new Vector3(-0.5f, -0.5f, transform.position.z);
#endif
    }
}
