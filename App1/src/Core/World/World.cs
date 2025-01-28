using System;
using System.Collections.Generic;
using System.Numerics;
using App1.Graphics;
using App1.Graphics.Textures;
using Microsoft.Xna.Framework.Graphics;

namespace App1.Core.World;

public class World
{
    private ChunkGenerator chunkGenerator;
    private Chunk[][] chunks;
    private Vector2 worldSize = new Vector2(7, 7);
    public World(int seed)
    {
        chunkGenerator = new ChunkGenerator(seed);
        chunks = new Chunk[(int)worldSize.X][];
        for (int x = 0; x < worldSize.X; x++)
        {
            chunks[x] = new Chunk[(int)worldSize.Y];
            for (int z = 0; z < worldSize.Y; z++)
            {
                chunks[x][z] = chunkGenerator.GenerateChunk(x, z, this);
            }
        }
    }
    
    public Chunk GetChunk(int x, int z)
    {
        return chunks[x][z];
    }
    
    public void SetBlock(int x, int y, int z, int type)
    {
        int chunkX = x / Chunk.SIZE;
        int chunkZ = z / Chunk.SIZE;
        chunks[chunkX][chunkZ].SetBlock(x % Chunk.SIZE, y, z % Chunk.SIZE, type);
    }
    
    public List<CubeData> GetVisibleCubes(Texture2D[] textures)
    {
        int nbChunks = chunks.Length * chunks[0].Length;
        Console.WriteLine("Nb chunks: " + nbChunks);
        
        List<CubeData> cubes = new List<CubeData>();
        for (int x = 0; x < worldSize.X; x++)
            for (int z = 0; z < worldSize.Y; z++)
                cubes.AddRange(chunks[x][z].getVisibleCubes(textures));
        return cubes;
    }
    
    public List<FaceData> GetVisibleFaces(Atlas atlas)
    {
        int nbChunks = chunks.Length * chunks[0].Length;
        Console.WriteLine("Nb chunks: " + nbChunks);
        
        List<FaceData> cubes = new List<FaceData>();
        for (int x = 0; x < worldSize.X; x++)
            for (int z = 0; z < worldSize.Y; z++)
                cubes.AddRange(chunks[x][z].getVisibleFaces(atlas));
        return cubes;
    }
}