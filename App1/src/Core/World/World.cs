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
    private Vector2 worldSize = new Vector2(5, 16);
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
        for (int x = 0; x < worldSize.X; x++)
            for (int z = 0; z < worldSize.Y; z++)
                chunkGenerator.GenerateTrees(chunks[x][z], x, z, this);

    }
    
    public Chunk GetChunk(int x, int z)
    {
        return chunks[x][z];
    }
    
    public int GetBlock(int x, int y, int z)
    {
        int chunkX = x / Chunk.SIZE;
        int chunkZ = z / Chunk.SIZE;
        return chunks[chunkX][chunkZ].GetBlock(new Vector3(x % Chunk.SIZE, y, z % Chunk.SIZE));
    }
    
    public void SetBlock(int x, int y, int z, int type)
    {
        if (x < 0 || y < 0 || z < 0)
            return;
        Console.WriteLine(x + " " + y + " " + z);
        int chunkX = x / Chunk.SIZE;
        int chunkZ = z / Chunk.SIZE;
        Console.WriteLine(" " + chunkX + " " + chunkZ);
        if (!(chunkX >= 0 && chunkZ >= 0 && chunkX < worldSize.X && chunkZ < worldSize.Y))
            return;

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