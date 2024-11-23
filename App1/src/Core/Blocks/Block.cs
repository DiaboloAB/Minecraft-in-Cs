namespace App1.Core.Block;

public class Block
{
    public int Type { get; }
    
    public Block(int type)
    {
        Type = type;
    }
    
    protected BlockModel Model { get; set; }
}