using System;
using System.Collections.Generic;
using App1.Graphics;
using App1.Graphics.Textures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace App1.Core.World;

public class Chunk
{
    public const int SIZE = 16;
    public const int HEIGHT = 256;
    private readonly int [,,] blocks;
    public Vector3 position;
    public bool IsDirty = true;
    
    private World world;
    
    public Chunk(Vector3 position, World world)
    {
        this.world = world;
        this.position = position;
        blocks = new int[SIZE, HEIGHT, SIZE];
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