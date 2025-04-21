using System.Collections.Generic;

namespace ProjectM.Core.Block;

public class BlockModel
{
    public List<SubMesh> SubMeshes { get; }
    public bool IsFull { get; }

    public BlockModel(List<SubMesh> subMeshes, bool isFull)
    {
        SubMeshes = subMeshes;
        IsFull = isFull;
    }
    
}