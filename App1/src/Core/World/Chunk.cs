using Microsoft.Xna.Framework;

namespace App1.Core.World;

public class Chunk
{
    public const int SIZE = 16;
    private readonly int [,,] blocks;
    private Vector3 position;
    public bool IsDirty = true;
    
    public Chunk(Vector3 position)
    {
        this.position = position;
        blocks = new int[SIZE, SIZE, SIZE];
    }
    
    public int GetBlock(Vector3 pos)
    {
        return blocks[(int)pos.X, (int)pos.Y, (int)pos.Z];
    }
    
    public void SetBlock(int x, int y, int z, int type)
    {
        blocks[x, y, z] = type;
        IsDirty = true;
    }
}