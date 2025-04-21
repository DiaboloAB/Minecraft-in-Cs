using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectM.Core.Block;

public class SubMesh
{
    public VertexPositionTexture[] Vertices { get; }
    public int[] Indices { get; }
    public Vector2[] TextureCoordinates { get; }
    public Vector3 Offset { get; }
    
    public SubMesh(VertexPositionTexture[] vertices, int[] indices, Vector2[] textureCoordinates, Vector3 offset)
    {
        Vertices = vertices;
        Indices = indices;
        TextureCoordinates = textureCoordinates;
        Offset = offset;
    }
}