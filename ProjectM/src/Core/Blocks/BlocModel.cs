using System.Collections.Generic;

namespace ProjectM.Core;

public class BlocModel
{
    public List<SubMesh> SubMeshes { get; }
    public bool IsFull { get; }

    public BlocModel(List<SubMesh> subMeshes, bool isFull)
    {
        SubMeshes = subMeshes;
        IsFull = isFull;
    }
    
}