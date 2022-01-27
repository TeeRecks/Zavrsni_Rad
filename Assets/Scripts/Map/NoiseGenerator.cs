using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator
{
    public static float[,] GenerateHeightMap(int mapSize, float scale, Vector2 offset)
    {
        float[,] noiseMap = new float[mapSize, mapSize];

        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                float perlinX = (float)x * scale + offset.x;
                float perlinY = (float)y * scale + offset.y;

                noiseMap[x, y] = Mathf.PerlinNoise(perlinX, perlinY);


            }
        }

        return noiseMap;
    }
}
