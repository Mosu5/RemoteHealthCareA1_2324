using DotnetNoise;

namespace VRConnection.Graphics;

/// <summary>
///     Helper class for generating Perlin noise.
///     Uses the DotnetNoise library: https://github.com/Auburn/FastNoiseLite/blob/master/CSharp/README.md
/// </summary>
public static class PerlinNoiseGenerator
{
    /// <summary>
    ///     Generates a height map using Perlin noise.
    /// </summary>
    /// <param name="minHeight">The minimum height of the terrain.</param>
    /// <param name="amplitude"> // Amplitude affects the height of the terrainimum height of the terrain.</param>
    /// <returns></returns>
    public static float[] GenerateHeightMap(float amplitude)
    {
        // Create and configure FastNoise object
        var noise = new FastNoise();
        noise.UsedNoiseType = FastNoise.NoiseType.Perlin;

        // Create height map
        var heightMap = new float[256 * 256];
        var index = 0;

        for (var y = 0; y < 256; y++)
        for (var x = 0; x < 256; x++)
        {
            // Generate noise and add to height map
            var noiseData = noise.GetPerlin(x, y);
            heightMap[index++] = noiseData * amplitude;
        }

        return heightMap;
    }
}