using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace App1.Core.World;

public class ChunkGenerator
{
    private int seed;
    FastNoiseLite noise = new FastNoiseLite();
    public Texture2D[] Textures { get; set; }
    
    public ChunkGenerator(int seed)
    {
        // get random nbr
        int random = new Random().Next(0, 1000000);
        this.seed = random;
        
        noise.SetSeed(random);
        noise = new FastNoiseLite(random);
        noise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        noise.SetFrequency(0.1f);
        noise.SetFractalType(FastNoiseLite.FractalType.FBm);
        noise.SetFractalOctaves(2);
        noise.SetFractalLacunarity(2.0f);
        noise.SetFractalGain(0.5f);
        
        
        Textures = new Texture2D[BlockType.GetValues(typeof(BlockType)).Length];
    }
    
    public Chunk GenerateChunk(int x, int z, World world)
    {
        int width = Chunk.SIZE;
        int height = Chunk.HEIGHT;

        Chunk chunk = new Chunk(new Vector3(x, 0, z), world);
        
        for (int i = 0; i < width; i++)
        {
            for (int k = 0; k < width; k++)
            {
                int stoneHeight = (int)(noise.GetNoise((x * Chunk.SIZE + i) * 0.1f, (z * Chunk.SIZE  + k) * 0.1f) * 10) + 10;
                for (int j = 0; j < height; j++)
                {
                    if (j < stoneHeight)
                    {
                        chunk.SetBlock(i, j, k, (int)BlockType.Stone);
                    }
                    else if (j == stoneHeight)
                    {
                        chunk.SetBlock(i, j, k, (int)BlockType.Grass);
                    }
                    else
                    {
                        chunk.SetBlock(i, j, k, (int)BlockType.Air);
                    }
                    
                }
            }
            
        }

        return chunk;

    }
    
    
}

public enum BlockType
{
    Air = 0,
    Grass = 1,
    Stone = 2
}
