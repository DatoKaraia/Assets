using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainTextureChanger : MonoBehaviour
{
    public Terrain terrain;             // Reference to the Terrain
    public int paintTextureIndex = 0;   // Index of the terrain layer to paint
    public int initialTextureIndex = 1; // Index of the initial terrain layer
    public float radius = 5.0f;         // Radius of the texture change around the player
    public Transform player;            // Reference to the player

    private TerrainData terrainData;
    private int alphamapWidth;
    private int alphamapHeight;
    private float[,,] splatmapData;

    void Start()
    {
        // Auto-assign the terrain if it hasn't been assigned in the Inspector
        if (terrain == null)
        {
            terrain = Terrain.activeTerrain;
        }

        // Proceed with setting up the terrain data
        terrainData = terrain.terrainData;
        alphamapWidth = terrainData.alphamapWidth;
        alphamapHeight = terrainData.alphamapHeight;
        splatmapData = terrainData.GetAlphamaps(0, 0, alphamapWidth, alphamapHeight);
    }

    void Update()
    {
        // Continuously update the terrain texture around the player
        UpdateTerrainTextureAroundPlayer();
    }

    void UpdateTerrainTextureAroundPlayer()
    {
        // Convert player's world position to terrain position
        Vector3 terrainPos = terrain.transform.InverseTransformPoint(player.position);

        int x = Mathf.RoundToInt((terrainPos.x / terrainData.size.x) * alphamapWidth);
        int z = Mathf.RoundToInt((terrainPos.z / terrainData.size.z) * alphamapHeight);

        int radiusInSamples = Mathf.RoundToInt(radius / terrainData.size.x * alphamapWidth);

        for (int i = 0; i < alphamapWidth; i++)
        {
            for (int j = 0; j < alphamapHeight; j++)
            {
                // Calculate the distance from the current point to the player
                float distance = Vector2.Distance(new Vector2(i, j), new Vector2(x, z));

                // Determine which texture to apply based on the distance to the player
                if (distance <= radiusInSamples)
                {
                    // Apply the paint texture within the radius
                    splatmapData[j, i, paintTextureIndex] = 1.0f;
                    splatmapData[j, i, initialTextureIndex] = 0.0f;
                }
                else
                {
                    // Revert to the initial texture outside the radius
                    splatmapData[j, i, paintTextureIndex] = 0.0f;
                    splatmapData[j, i, initialTextureIndex] = 1.0f;
                }
            }
        }

        // Apply the modified splatmap data back to the terrain
        terrainData.SetAlphamaps(0, 0, splatmapData);
    }
}
