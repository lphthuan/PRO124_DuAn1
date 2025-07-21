using System.Collections;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GhostTilemap : MonoBehaviour
{
    void Start()
    {
        TilemapRenderer tilemapRenderer = GetComponent<TilemapRenderer>();
        if (tilemapRenderer != null)
        {
            tilemapRenderer.material.color = new Color(1f, 1f, 1f, 0.5f); // trắng, alpha 50%
        }
    }
}
