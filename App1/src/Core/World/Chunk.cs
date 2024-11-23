using Microsoft.Xna.Framework;

namespace App1.Core.World;

public class Chunk
{
    public const int SIZE = 16;
    private readonly int [,,] blocks;
    private Vector3 position;
    private bool isDirty;
    
    public Chunk(Vector3 position)
    {
        this.position = position;
        blocks = new int[SIZE, SIZE, SIZE];
    }
    
    public int getBlock(int x, int y, int z)
    {
        return blocks[x, y, z];
    }
    
    public void setBlock(int x, int y, int z, int type)
    {
        blocks[x, y, z] = type;
        isDirty = true;
    }
}