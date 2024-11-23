using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace App1.Graphics;

public struct VertexPositionTexture
{
    public Vector3 Position;
    public Vector2 Texturecoordinate;

    public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration(
        new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
        new VertexElement(12, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0)
    );

    public VertexPositionTexture(Vector3 position, Vector2 texturecoordinate)
    {
        Position = position;
        Texturecoordinate = texturecoordinate;
    }
}