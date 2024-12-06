using System.Collections.Generic;
using App1.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace App1.Core.World;

public class Chunk
{
    public const int SIZE = 16;
    public const int HEIGHT = 256;
    private readonly int [,,] blocks;
    private Vector3 position;
    public bool IsDirty = true;
    
    public Chunk(Vector3 position)
    {
        this.position = position;
        blocks = new int[SIZE, HEIGHT, SIZE];
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
                        Position = blockPos + position,
                        Scale = Vector3.One,
                        Texture = textures[1]
                    });
                    
                }
            }
        }
        return cubes;
    }
    
    private int CalculateFaceMask(int x, int y, int z)
    {
        int mask = 0b111111;
        
        if (GetBlock(new Vector3(x, y + 1, z)) != 0) mask &= 0b111110;
        if (GetBlock(new Vector3(x, y - 1, z)) != 0) mask &= 0b111101;
        if (GetBlock(new Vector3(x + 1, y, z)) != 0) mask &= 0b111011;
        if (GetBlock(new Vector3(x - 1, y, z)) != 0) mask &= 0b110111;
        if (GetBlock(new Vector3(x, y, z + 1)) != 0) mask &= 0b101111;
        if (GetBlock(new Vector3(x, y, z - 1)) != 0) mask &= 0b011111;
        
        
        
        
        return mask;
    }
}