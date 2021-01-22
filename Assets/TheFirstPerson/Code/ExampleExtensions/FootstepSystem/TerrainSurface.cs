using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// helper class to get texture at point on a terrain for FootstepSounds extension
// based on https://answers.unity.com/questions/34328/terrain-with-multiple-splat-textures-how-can-i-det.html
// modified to work in unity 2018.3 onward. Will not work in earlier versions

public class TerrainSurface
{
    public static float[] GetTextureMix(Terrain terrain, Vector3 worldPos)
    {
        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;
        int mapX = (int)(((worldPos.x - terrainPos.x) / terrainData.size.x) * terrainData.alphamapWidth);
        int mapZ = (int)(((worldPos.z - terrainPos.z) / terrainData.size.z) * terrainData.alphamapHeight);
        float[,,] splatmapData = terrainData.GetAlphamaps(mapX, mapZ, 1, 1);
        float[] cellMix = new float[splatmapData.GetUpperBound(2) + 1];
        for (int n = 0; n < cellMix.Length; ++n)
        {
            cellMix[n] = splatmapData[0, 0, n];
        }
        return cellMix;
    }
    public static Texture2D GetMainTexture(Terrain terrain, Vector3 worldPos)
    {
        float[] mix = GetTextureMix(terrain, worldPos);
        float maxMix = 0;
        int maxIndex = 0;

        for (int n = 0; n < mix.Length; ++n)
        {
            if (mix[n] > maxMix)
            {
                maxIndex = n;
                maxMix = mix[n];
            }
        }
        return terrain.terrainData.terrainLayers[maxIndex].diffuseTexture;
    }
}
