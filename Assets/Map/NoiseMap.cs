using UnityEngine;


// noisemap[] generation, where each element is tile's height

public static class NoiseMap
{

   // generates a noisemap, consisting of values between 0 and 1
    public static float[,] GenerateNoiseMap(int mapSize, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        float[,] noiseMap = new float[mapSize, mapSize];

        // pseudorandom number generator
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for(int i = 0; i < octaves; i++) {
            // when getting too big values PerlinNoise returns the same values, so we use range [-100000, 100000] to prevent it
            // and adding offset to scroll through the noise
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }

        if (scale <= 0) scale = 0.0001f;    // to prevent division by zero
        
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        // getting map center
        float halfWidth = mapSize / 2f;
        float halfHeight = mapSize / 2f;

        // fill noisemap with noise
        for (int y = 0; y < mapSize; y++) {
            for (int x = 0; x < mapSize; x++) {
                if (GameManager.Instance.MapGenerator.IsCellInGrid(x, y)) {
                    // start values
                    float amplitude = 1;
                    float frequency = 1;
                    float noiseHeight = 0;

                    for (int i = 0; i < octaves; i++) {
                        // * frequency means, that the higher the frequency, the further apart the sample points will be
                        // which means that height values will change more rapidly
                        // then + octaveOffsets to randomize the result 
                        // also - half<mapSize> to scale map from center
                        float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets [i].x;
                        float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets [i].y;

                        // PerlinmNoise values is in range [0, 1], so we (* 2 - 1) for changing range to [-1, 1]
                        float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                        noiseHeight += perlinValue * amplitude;

                        // persistance is in range [0, 1] - decreases each octave
                        amplitude *= persistance;
                        // frequency increases each octave, since lacunarity should be > 1
                        frequency *= lacunarity;
                    }

                    // getting the actual range of noisemap after amplitude & frequency using
                    if (noiseHeight > maxNoiseHeight) { 
                        maxNoiseHeight = noiseHeight;
                    }
                    else if (noiseHeight < minNoiseHeight) {
                        minNoiseHeight = noiseHeight;
                    }
                    // apply noiseHeight to noiseMap
                    noiseMap [x, y] = noiseHeight;
                }
            }            
        }

        // after getting the actual range of noise values, we loop through all noiseMap values again to normalize them
        for (int y = 0; y < mapSize; y++) {
            for (int x = 0; x < mapSize; x++) {
                if (GameManager.Instance.MapGenerator.IsCellInGrid(x, y)) {
                    // InverseLerp returns range [0, 1], so, 
                    // if noiseMap[x, y] == minNoiseHeight => 0
                    // if noiseMap[x, y] == maxNoiseHeight => 1
                    noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
                }
            }
        }

        return noiseMap;
    }
    
}
