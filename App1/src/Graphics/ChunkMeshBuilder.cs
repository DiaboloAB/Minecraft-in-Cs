using System.Collections.Generic;
using System.Numerics;
using App1.Core.Block;
using App1.Core.World;

namespace App1.Graphics;

public class ChunkMeshBuilder
{
    private List<VertexPositionTexture> vertices;
    private List<short> indices;

    public void BuildMeshes(Chunk chunk)
    {
        vertices.Clear();
        indices.Clear();
        
        for (int i = 0; i < Chunk.SIZE; i++)
        for (int j = 0; j < Chunk.SIZE; j++)
        for (int k = 0; k < Chunk.SIZE; k++)
        {
            Vector3 pos = new Vector3(i, j, k);
            // Block block = chunk.GetBlock(pos);
        }
                
    }

    private void AddVisibleFaces(Block block)
    {
        
    }
}