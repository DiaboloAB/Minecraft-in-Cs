﻿using System;
using System.Collections.Generic;
using ProjectM.Graphics;
using ProjectM.Graphics.Textures;
using Microsoft.Xna.Framework.Graphics;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using VertexPositionTexture = ProjectM.Graphics.VertexPositionTexture;

namespace ProjectM.Core.World;

public class Chunk
{
    public const int SIZE = 16;
    public const int HEIGHT = 256;
    public bool IsDirty = true;
    public bool Generated = false;
    public bool Processing = false;
    public Vector3 WorldPosition;
    public Vector3 MatrixPosition;
    
    private readonly Bloc [,,] blocs;

    
    private World world;
    
    public volatile bool HasMeshDataRdy = false;
    
    
    // public 
    public VertexBuffer VertexBuffer;
    public IndexBuffer IndexBuffer;
    public int IndexCount;
    
    public Chunk(Vector3 matrixPosition, World world)
    {
        this.world = world;
        this.MatrixPosition = matrixPosition;
        this.WorldPosition = matrixPosition * new Vector3(SIZE, 0, SIZE);
        blocs = new Bloc[SIZE, HEIGHT, SIZE];
        for (int x = 0; x < SIZE; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                for (int z = 0; z < SIZE; z++)
                {
                    blocs[x, y, z] = new Bloc(new Vector3(x, y, z), this, Core.BlocType.Air);
                }
            }
        }
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
                    int blockType = (int)blocs[x, y, z].Type;
                    if (blockType == 0) continue;
                    
                    Vector3 blockPos = new Vector3(x, y, z) + WorldPosition;
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
            Vector2 texCoord = BlocTextureCoord.TextureCoords[blockType][i];
            
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
            Bloc bloc;
            if (x < 0)
            {
                bloc = world.GetChunk((int)MatrixPosition.X - 1, (int)MatrixPosition.Z).GetBloc(Chunk.SIZE + x, y, z);
            }
            else if (x >= Chunk.SIZE)
            {
                bloc = world.GetChunk((int)MatrixPosition.X + 1, (int)MatrixPosition.Z).GetBloc(x - Chunk.SIZE, y, z);
            }
            else if (z < 0)
            {
                bloc = world.GetChunk((int)MatrixPosition.X, (int)MatrixPosition.Z - 1).GetBloc(x, y, Chunk.SIZE + z);
            }
            else if (z >= Chunk.SIZE)
            {
                bloc = world.GetChunk((int)MatrixPosition.X, (int)MatrixPosition.Z + 1).GetBloc(x, y, z - Chunk.SIZE);
            }
            else
            {
                bloc = GetBloc(x, y, z);
            }
            
            int blockType = (int)bloc.Type;

            // Treat Leaves as Air
            if (blockType == (int)BlocType.Leaves)
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

    
    public Bloc GetBloc(int x, int y, int z)
    {
        if (x < 0 || x >= SIZE || y < 0 || y >= HEIGHT || z < 0 || z >= SIZE)
        {
            return null;
        }
        return blocs[x, y, z];
    }
    
    public void SetBlock(int x, int y, int z, int type)
    {
        blocs[x, y, z].Type = (Core.BlocType)type;
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
                    int blockType = (int)blocs[x, y, z].Type;
                    if (blockType == 0) continue;
                    Vector3 blockPos = new Vector3(x, y, z);

                    int faceMask = CalculateFaceMask(x, y, z);
                    if (faceMask == 0) continue;
                    
                    cubes.Add(new CubeData
                    {
                        Position = blockPos + WorldPosition,
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
                    int blockType = (int)blocs[x, y, z].Type;
                    if (blockType == 0) continue;
                    Vector3 blockPos = new Vector3(x, y, z);

                    int faceMask = CalculateFaceMask(x, y, z);

                    for (int i = 0; i < 6; i++)
                    {
                        if ((faceMask & (1 << i)) == 0) continue;
                        faces.Add(new FaceData
                        {
                            Position = blockPos + WorldPosition,
                            TexCoord = BlocTextureCoord.TextureCoords[blockType][i],
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