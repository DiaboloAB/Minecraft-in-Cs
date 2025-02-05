using System.Collections.Generic;
using App1.Graphics;
using App1.Graphics.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace App1.Core.World;

public class World
{
    private ChunkGenerator chunkGenerator;
    public Dictionary<(int, int), Chunk> chunks;
    private Vector2 worldSize = new Vector2(40, 40);
    
    private int seed;
    public World(int seed)
    {
        this.seed = seed;
        chunkGenerator = new ChunkGenerator(seed);
        chunks = new Dictionary<(int, int), Chunk>();
        // for (int x = 0; x < worldSize.X; x++)
        // {
        //     for (int z = 0; z < worldSize.Y; z++)
        //     {
        //         var chunk = chunkGenerator.GenerateChunk(x, z, this);
        //         chunks[(x, z)] = chunk;
        //     }
        // }
        // for (int x = 0; x < worldSize.X; x++)
        //     for (int z = 0; z < worldSize.Y; z++)
        //         chunkGenerator.GenerateTrees(chunks[(x, z)], x, z, this);
    }
    
    public Chunk GetChunk(int x, int z)
    {
        return chunks[(x, z)];
    }
    
    public Vector3 GetChunkPosition(Vector3 position)
    {
        return new Vector3((int)position.X / Chunk.SIZE, 0, (int)position.Z / Chunk.SIZE);
    }
    
    public int GetBlock(int x, int y, int z)
    {
        int chunkX = x / Chunk.SIZE;
        int chunkZ = z / Chunk.SIZE;
        return chunks[(chunkX, chunkZ)].GetBlock(new Vector3(x % Chunk.SIZE, y, z % Chunk.SIZE));
    }
    
    public void SetBlock(int x, int y, int z, int type)
    {
        if (x < 0 || y < 0 || z < 0)
            return;
        int chunkX = x / Chunk.SIZE;
        int chunkZ = z / Chunk.SIZE;
        if (!(chunkX < worldSize.X && chunkZ < worldSize.Y))
            return;
        if (!chunks.ContainsKey((chunkX, chunkZ)))
            return;
        chunks[(chunkX, chunkZ)].SetBlock(x % Chunk.SIZE, y, z % Chunk.SIZE, type);
    }
    
    public List<CubeData> GetVisibleCubes(Texture2D[] textures)
    {
        
        List<CubeData> cubes = new List<CubeData>();
        foreach(var chunk in chunks.Values)
            cubes.AddRange(chunk.getVisibleCubes(textures));
        return cubes;
    }
    
    public void GenerateChunks(Vector3 position, int radius)
    {
        for (int i = (int)(position.X - radius); i < position.Z + radius; i++)
        {
            for (int j = (int)(position.Z - radius); j < position.Z + radius; j++)
            {
                if (!chunks.ContainsKey((i, j)))
                {
                    var chunk = chunkGenerator.GenerateChunk(i, j, this);
                    chunks[(i, j)] = chunk;
                    chunk.IsDirty = true;
                    chunkGenerator.GenerateTrees(chunk, i, j, this);
                }
            }
        }
    }
    
    public List<FaceData> GetVisibleFaces(Atlas atlas)
    {
        List<FaceData> cubes = new List<FaceData>();
        foreach(var chunk in chunks.Values)
            cubes.AddRange(chunk.getVisibleFaces(atlas));
        return cubes;
    }
    
    public void CreateChunksBuffers(GraphicsDevice graphicsDevice)
    {
        // Console.WriteLine("Creating chunks buffers");
        foreach (var chunk in chunks.Values)
        {
            if (chunk.IsDirty)
            {
                // Console.WriteLine("Creating chunk buffers");
                chunk.CreateBuffers(graphicsDevice);
                chunk.IsDirty = false;
            }

                
        }
            
    }
    
}