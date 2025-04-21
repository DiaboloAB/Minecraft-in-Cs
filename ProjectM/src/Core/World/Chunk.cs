using System;
using System.Collections.Generic;
using App1.Graphics;
using App1.Graphics.Textures;
using Microsoft.Xna.Framework.Graphics;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using VertexPositionTexture = App1.Graphics.VertexPositionTexture;

namespace App1.Core.World;

public class Chunk
{
    public const int SIZE = 16;
    public const int HEIGHT = 256;
    private readonly int [,,] blocks;
    public Vector3 position;
    public bool IsDirty = true;
    public bool Generated = false;
    public bool Processing = false;
    
    private World world;
    
    public volatile bool HasMeshDataRdy = false;
    
    
    // public 
    public VertexBuffer VertexBuffer;
    public IndexBuffer IndexBuffer;
    public int IndexCount;
    
    public Chunk(Vector3 position, World world)
    {
        this.world = world;
        this.position = position;
        blocks = new int[SIZE, HEIGHT, SIZE];
    }
    
    public void CreateBuffers(GraphicsDevice graphicsDevice)
    {
        List<VertexPositionTexture> vertices = new List<VertexPositionTexture>();
        List<short> indices = new List<short>();

        short index = 0;
        for (int x = 0; x < SIZE; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                for (int z = 0; z < SIZE; z++)
                {
                    int blockType = blocks[x, y, z];
                    if (blockType == 0) continue;
                    
                    Vector3 blockPos = new Vector3(x, y, z) + position * new Vector3(SIZE, HEIGHT, SIZE);
                    AddCubeVertices(blockType, vertices, indices, blockPos, new Vector3(x, y, z), ref index);
                }
            }
        }
        
        VertexBuffer = new VertexBuffer(graphicsDevice, VertexPositionTexture.VertexDeclaration, vertices.Count, BufferUsage.WriteOnly);
        VertexBuffer.SetData(vertices.ToArray());

        IndexBuffer = new IndexBuffer(graphicsDevice, IndexElementSize.SixteenBits, indices.Count, BufferUsage.WriteOnly);
        IndexBuffer.SetData(indices.ToArray());
    }
    
    private void AddCubeVertices(int blockType, List<VertexPositionTexture> vertices, List<short> indices, Vector3 position, Vector3 blockPosChunk, ref short index)
    {
        
        if (blockPosChunk.X > 15)
        {
            Console.WriteLine("blockPosChunk: " + blockPosChunk);
        }
        int faceMask = CalculateFaceMask((int)blockPosChunk.X, (int)blockPosChunk.Y, (int)blockPosChunk.Z);
        
        for (int i = 0; i < 6; i++)
        {
            if ((faceMask & (1 << i)) == 0) continue;
            Vector2 texCoord = BlockTextureCoord.TextureCoords[blockType][i];
            
            vertices.Add(new VertexPositionTexture(position + Cube.FaceVertices[i][0], texCoord / 2048.0f));
            vertices.Add(new VertexPositionTexture(position + Cube.FaceVertices[i][1], (texCoord + new Vector2(0, 16)) / 2048.0f));
            vertices.Add(new VertexPositionTexture(position + Cube.FaceVertices[i][2], (texCoord + new Vector2(16, 16)) / 2048.0f));
            vertices.Add(new VertexPositionTexture(position + Cube.FaceVertices[i][3], (texCoord + new Vector2(16, 0)) / 2048.0f));
            
            indices.Add(index);
            indices.Add((short)(index + 1));
            indices.Add((short)(index + 2));
            indices.Add(index);
            indices.Add((short)(index + 2));
            indices.Add((short)(index + 3));
            
            index += 4;
        }
     
        IndexCount = index;
    }
    
    public int GetBlockFromNeighboringChunks(int x, int y, int z)
    {
        try
        {
            int blockType;
            if (x < 0)
            {
                blockType = world.GetChunk((int)position.X - 1, (int)position.Z).GetBlock(new Vector3(Chunk.SIZE + x, y, z));
            }
            else if (x >= Chunk.SIZE)
            {
                blockType = world.GetChunk((int)position.X + 1, (int)position.Z).GetBlock(new Vector3(x - Chunk.SIZE, y, z));
            }
            else if (z < 0)
            {
                blockType = world.GetChunk((int)position.X, (int)position.Z - 1).GetBlock(new Vector3(x, y, Chunk.SIZE + z));
            }
            else if (z >= Chunk.SIZE)
            {
                blockType = world.GetChunk((int)position.X, (int)position.Z + 1).GetBlock(new Vector3(x, y, z - Chunk.SIZE));
            }
            else
            {
                blockType = GetBlock(new Vector3(x, y, z));
            }

            // Treat Leaves as Air
            if (blockType == (int)BlockType.Leaves)
            {
                return 0;
            }

            return blockType;
        }
        catch
        {
            return 0;
        }
    }

    public int GetBlockAt(Vector3 pos)
    {
        pos = pos - position * new Vector3(SIZE, HEIGHT, SIZE);
        if (pos.X < 0 || pos.X >= SIZE || pos.Y < 0 || pos.Y >= HEIGHT || pos.Z < 0 || pos.Z >= SIZE)
        {
            return 0;
        }
        return blocks[(int)pos.X, (int)pos.Y, (int)pos.Z];
    }
    
    public int GetBlock(Vector3 pos)
    {
        if (pos.X < 0 || pos.X >= SIZE || pos.Y < 0 || pos.Y >= HEIGHT || pos.Z < 0 || pos.Z >= SIZE)
        {
            return 0;
        }
        return blocks[(int)pos.X, (int)pos.Y, (int)pos.Z];
    }
    
    public void SetBlock(int x, int y, int z, int type)
    {
        blocks[x, y, z] = type;
        IsDirty = true;
    }
    
    public List<CubeData> getVisibleCubes(Texture2D[] textures)
    {
        List<CubeData> cubes = new List<CubeData>();
        
        for (int x = 0; x < SIZE; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                for (int z = 0; z < SIZE; z++)
                {
                    int blockType = blocks[x, y, z];
                    if (blockType == 0) continue;
                    Vector3 blockPos = new Vector3(x, y, z);

                    int faceMask = CalculateFaceMask(x, y, z);
                    if (faceMask == 0) continue;
                    
                    cubes.Add(new CubeData
                    {
                        Position = blockPos + position * new Vector3(SIZE, HEIGHT, SIZE),
                        Scale = Vector3.One,
                        Texture = textures[1]
                    });
                    
                }
            }
        }
        return cubes;
    }

    public List<FaceData> getVisibleFaces(Atlas atlas)
    {
        List<FaceData> faces = new List<FaceData>();
        for (int x = 0; x < SIZE; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                for (int z = 0; z < SIZE; z++)
                {
                    int blockType = blocks[x, y, z];
                    if (blockType == 0) continue;
                    Vector3 blockPos = new Vector3(x, y, z);

                    int faceMask = CalculateFaceMask(x, y, z);

                    for (int i = 0; i < 6; i++)
                    {
                        if ((faceMask & (1 << i)) == 0) continue;
                        faces.Add(new FaceData
                        {
                            Position = blockPos + position * new Vector3(SIZE, HEIGHT, SIZE),
                            TexCoord = BlockTextureCoord.TextureCoords[blockType][i],
                            Orientation = (short)i
                        });
                    }
                    
                    
                }
            }
        }

        return faces;
    }
    
    private int CalculateFaceMask(int x, int y, int z)
    {
        int mask = 0b111111;
        //front
        if (GetBlockFromNeighboringChunks(x, y, z + 1) != 0) mask &= 0b111110;
        //back
        if (GetBlockFromNeighboringChunks(x, y, z - 1) != 0) mask &= 0b111101;
        //top
        if (GetBlockFromNeighboringChunks(x, y + 1, z) != 0) mask &= 0b111011;
        //bottom
        if (GetBlockFromNeighboringChunks(x, y - 1, z) != 0) mask &= 0b110111;
        //left
        if (GetBlockFromNeighboringChunks(x - 1, y, z) != 0) mask &= 0b101111;
        //right
        if (GetBlockFromNeighboringChunks(x + 1, y, z) != 0) mask &= 0b011111;
        
        return mask;
    }
}