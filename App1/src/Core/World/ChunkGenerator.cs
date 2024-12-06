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
        this.seed = seed;
        
        
        Textures = new Texture2D[BlockType.GetValues(typeof(BlockType)).Length];
    }
    
    public Chunk GenerateChunk(int x, int z)
    {
        int width = Chunk.SIZE;
        int height = Chunk.HEIGHT;

        Chunk chunk = new Chunk(new Vector3(x, 0, z));
        
        for (int i = 0; i < width; i++)
        {
            for (int k = 0; k < width; k++)
            {
                int stoneHeight = (int)(noise.GetNoise((x + i) * 0.1f, (z + k) * 0.1f) * 10) + 10;
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
