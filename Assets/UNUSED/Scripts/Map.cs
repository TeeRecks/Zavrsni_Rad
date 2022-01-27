using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Map : MonoBehaviour
{
    public RawImage debugImg;

    public int mapSize;
    public float scale;
    public Vector2 offset;
    public float[,] heightMap;

    private void Update()
    {
        GenerateMap();
    }

    void GenerateMap()
    {
        heightMap = NoiseGenerator.GenerateHeightMap(mapSize, scale, offset);

        Color[] pixels = new Color[mapSize * mapSize];
        int i = 0;
        for (int x = 0; x < mapSize; x++)
        {
            for (int y = 0; y < mapSize; y++)
            {
                pixels[i] = Color.Lerp(Color.black, Color.white, heightMap[x, y]);
                i++;
            }
        }

        Texture2D tex = new Texture2D(mapSize, mapSize);
        tex.SetPixels(pixels);
        //point osigurava da se slika prikaže kakva je, nebude izbluralo sliku između pixela da se čini većom
        tex.filterMode = FilterMode.Point;
        tex.Apply();

        debugImg.texture = tex;
    }
}
