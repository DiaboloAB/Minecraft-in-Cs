using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectM.Core.World;

public class ChunkGenerator
{
    private int seed;
    FastNoiseLite noise = new FastNoiseLite();
    FastNoiseLite caveNoise;
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

        caveNoise = new FastNoiseLite(seed + 1);
        caveNoise.SetNoiseType(FastNoiseLite.NoiseType.OpenSimplex2);
        caveNoise.SetFrequency(0.5f);
        
        Textures = new Texture2D[BlocType.GetValues(typeof(BlocType)).Length];
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
                int stoneHeight = (int)(noise.GetNoise((x * Chunk.SIZE + i) * 0.1f, (z * Chunk.SIZE  + k) * 0.1f) * 10) + 50;
                for (int j = 0; j < height; j++)
                {
                    // float caveValue = caveNoise.GetNoise((x * Chunk.SIZE + i) * 0.1f, j * 0.1f, (z * Chunk.SIZE + k) * 0.1f);
                    //
                    // // Tunneling algorithm to create long caves
                    // if (caveValue > 0.5f && IsTunnel(x * Chunk.SIZE + i, j, z * Chunk.SIZE + k))
                    //     continue;
                    
                    // 1 block of grass, 2 block of dirt, rest with stone
                    if (j == stoneHeight)
                    {
                        chunk.SetBlock(i, j, k, (int)BlocType.Grass);
                    }
                    else if (j < stoneHeight && j > stoneHeight - 3)
                    {
                        chunk.SetBlock(i, j, k, (int)BlocType.Dirt);
                    }
                    else if (j < stoneHeight && j >= stoneHeight - 50)
                    {
                        chunk.SetBlock(i, j, k, (int)BlocType.Stone);
                    }
                }
            }
            
        }

        chunk.Generated = true;
        return chunk;

    }
    private bool IsTunnel(int x, int y, int z)
    {
        // Simple tunneling algorithm to create long caves
        float tunnelValueX = caveNoise.GetNoise(x * 0.05f, y * 0.05f, z * 0.05f);
        float tunnelValueY = caveNoise.GetNoise(x * 0.05f, z * 0.05f, y * 0.05f);
        float tunnelValueZ = caveNoise.GetNoise(y * 0.05f, x * 0.05f, z * 0.05f);

        return tunnelValueX > 0.5f || tunnelValueY > 0.5f || tunnelValueZ > 0.5f;
    }

    public void GenerateTrees(Chunk chunk, int chunkX, int chunkZ, World world)
    {
        Random random = new Random(seed + chunkX * 1000 + chunkZ);
        int treeCount = random.Next(1, 5); // Random number of trees per chunk

        for (int i = 0; i < treeCount; i++)
        {
            int x = random.Next(0, Chunk.SIZE);
            int z = random.Next(0, Chunk.SIZE);
            int y = FindGround(chunk, x, z);

            if (y > 0)
            {
                PlaceTree(chunk, x, y, z, world);
            }
        }
    }

    private int FindGround(Chunk chunk, int x, int z)
    {
        for (int y = Chunk.HEIGHT - 1; y >= 0; y--)
        {
            if (chunk.GetBlock(new Vector3(x, y, z)) == (int)BlocType.Grass)
            {
                return y + 1;
            }
        }
        return -1;
    }
    
    private void PlaceTree(Chunk chunk, int x, int y, int z, World world)
    {
        // Trunk
        for (int i = 0; i < 5; i++)
        {
            world.SetBloc((int)(chunk.MatrixPosition.X + x), y + i, (int)(chunk.WorldPosition.Z + z), (int)BlocType.Wood);
        }

        // leaves
        for (int i = -2; i < 3; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                for (int k = -2; k < 3; k++)
                {
                    if (Math.Abs(i) + Math.Abs(j) + Math.Abs(k) < 5)
                    {
                        world.SetBloc((int)(chunk.WorldPosition.X + x + i), y + 5 + j, (int)(chunk.WorldPosition.Z + z + k), (int)BlocType.Leaves);
                    }
                }
            }
        }
        
    }

}
