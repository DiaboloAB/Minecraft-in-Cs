using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

    private GraphicsDevice _graphicsDevice;
    
    private int seed;

    object worldLock = new();

    public PriorityQueue<(int, int), int> chunkQueue;
    public PriorityQueue<(int, int), int> chunkBufferQueue;
    
    public World(int seed, GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
        this.seed = seed;
        chunkGenerator = new ChunkGenerator(seed);
        chunks = new Dictionary<(int, int), Chunk>();
        chunkQueue = new PriorityQueue<(int, int), int>();
        
        chunkBufferQueue = new PriorityQueue<(int, int), int>();
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
        Task.Run(Worker);
        Task.Run(BufferWorker);
    }

    void Worker()
    {
        while (true)
        {
            List<(int, int)> batch = new List<(int, int)>();

            lock (chunkQueue)
            {
                if (chunkQueue.Count > 0)
                {
                    var ch = chunkQueue.Dequeue();
                    batch.Add(ch);
                }
            }
            
            foreach (var item in batch)
            {
                // Console.WriteLine($"Processing chunk: ({item.Item1}, {item.Item2})");
            }

            Parallel.ForEach(batch, chunk =>
            {
                var generatedChunk = chunkGenerator.GenerateChunk(chunk.Item1, chunk.Item2, this);
                StoreGeneratedChunk(chunk.Item1, chunk.Item2, generatedChunk);
            });

            Thread.Sleep(10);
        }
    }

    void BufferWorker()
    {
        while (true)
        {
            try
            {
                
                List<(int, int)> batch = new List<(int, int)>();

                lock (chunkBufferQueue)
                {
                    // Console.WriteLine($"BufferWorker: {chunkBufferQueue.Count}");
                    if (chunkBufferQueue.Count > 0)
                    {
                        var ch = chunkBufferQueue.Dequeue();
                        batch.Add(ch);
                    }
                }
                
                // foreach (var item in batch)
                // {
                //     Console.WriteLine($"Rendering chunk: ({item.Item1}, {item.Item2})");
                // }

                Parallel.ForEach(batch, chunk =>
                {
                    lock (worldLock) // Synchronize access to chunks
                    {
                        if (chunks.ContainsKey(chunk))
                        {
                            chunks[chunk].CreateBuffers(_graphicsDevice);
                            chunks[chunk].HasMeshDataRdy = true;
                        }
                    }
                });

                Thread.Sleep(10);
            } catch (Exception e)
            {
                Console.WriteLine($"BufferWorker error: {e.Message}");
            }
        }
    }

    void StoreGeneratedChunk(int x, int z, Chunk chunk)
    {
        lock (worldLock)
        {
            chunks[(x, z)] = chunk;
        }
    }
    
    public Chunk GetChunk(int x, int z)
    {
        return chunks[(x, z)];
    }
    
    public Chunk GetChunk(Vector3 position)
    {
        
        return chunks[(
            (int)position.X / Chunk.SIZE - (position.X < 0 ? 1 : 0),
            (int)position.Z / Chunk.SIZE  - (position.Z < 0 ? 1 : 0)
            )];
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
    
    public int GetBlockAt(Vector3 pos)
    {
        Chunk chunk = GetChunk(pos);
        return chunk.GetBlockAt(pos);
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
        for (int i = (int)(position.X - radius); i < (int)(position.X + radius); i++)
        {
            for (int j = (int)(position.Z - radius); j < (int)(position.Z + radius); j++)
            {
                if (!chunks.ContainsKey((i, j)))
                {
                    int distance = (int)Vector3.Distance(position, new Vector3(i, 0, j));
                    // var chunk = chunkGenerator.GenerateChunk(i, j, this);
                    chunks[(i, j)] = new Chunk(new Vector3(i, 0, j), this);
                    chunkQueue.Enqueue((i, j), Math.Abs(
                        distance));
                    // chunk.IsDirty = true; 
                    // chunkGenerator.GenerateTrees(chunk, i, j, this);
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
    
    public void CreateChunksBuffers(Vector3 position, int radius)
    {
        // Console.WriteLine("Creating chunks buffers");
        foreach (var chunk in chunks.Values)
        {
            if (chunk.IsDirty && chunk.Generated)
            {
                int distance = (int)Vector3.Distance(position, new Vector3((int)chunk.position.X, 0, (int)chunk.position.Z));
                chunkBufferQueue.Enqueue(((int)chunk.position.X, (int)chunk.position.Z), Math.Abs(
                    distance));
                chunk.HasMeshDataRdy = false;
                chunk.IsDirty = false;
            }

                
        }
            
    }

    public List<Chunk> GetChunksIntersectingBoundingBox(Vector3 position, Vector3 size)
    {
        List<Chunk> intersectingChunks = new List<Chunk>();

        // Calculate the range of chunks that the bounding box intersects
        int startX = (int)Math.Floor(position.X / Chunk.SIZE);
        int endX = (int)Math.Ceiling((position.X + size.X) / Chunk.SIZE);
        int startZ = (int)Math.Floor(position.Z / Chunk.SIZE);
        int endZ = (int)Math.Ceiling((position.Z + size.Z) / Chunk.SIZE);

        for (int x = startX; x <= endX; x++)
        {
            for (int z = startZ; z <= endZ; z++)
            {
                if (chunks.ContainsKey((x, z)))
                {
                    intersectingChunks.Add(chunks[(x, z)]);
                }
            }
        }

        return intersectingChunks;
    }
    
}