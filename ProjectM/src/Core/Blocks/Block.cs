using Microsoft.Xna.Framework;
using ProjectM.Core.World;

namespace ProjectM.Core.Block;

public class Block
{
    
    public BlockType Type { get; set; }
    
    public Vector3 MatrixPosition { get; set; }
    
    private Chunk chunk;
    public Vector3 WorldPosition => MatrixPosition + chunk.WorldPosition;
    
    public Block(Vector3 matrixPosition, Chunk chunk, BlockType type)
    {
        chunk = chunk;
        MatrixPosition = matrixPosition;
        Type = type;
    }

    protected BlockModel Model { get; set; }
}

public enum BlockType
{
    Air = 0,
    Grass = 1,
    Stone = 2,
    Dirt = 3,
    Wood = 4,
    Leaves = 5,
}