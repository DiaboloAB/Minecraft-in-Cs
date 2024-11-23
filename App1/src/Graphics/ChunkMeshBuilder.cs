using System.Collections.Generic;
using System.Numerics;
using App1.Core.World;

namespace App1.Graphics;

public class ChunkMeshBuilder
{
    private Dictionary<int, List<CubeRenderer.InstanceData>> simpleBlockInstances;
    private Dictionary<int, List<CubeRenderer.InstanceData>> complexBlockInstances;

    public void buildMeshes(Chunk chunk)
    {
        for (int i = 0; i < Chunk.SIZE; i++)
        for (int j = 0; j < Chunk.SIZE; j++)
        for (int k = 0; k < Chunk.SIZE; k++)
        {
            Vector3 pos = new Vector3(i, j, k);
            // Block block = chunk.getBlock(i, j, k);
        }
                
    }
}