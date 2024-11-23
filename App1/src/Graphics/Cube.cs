using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace App1.Graphics;

public struct CubeData
{
    public Vector3 Position { get; set; }
    public Vector3 Scale { get; set; }
    public Texture2D Texture { get; set; }
}

public class Cube
{
    public static VertexPositionTexture[] CreateVertices()
    {
        float third = 1.0f / 3.0f;
        return new VertexPositionTexture[]
        {
            // Front face
            new VertexPositionTexture(new Vector3(-0.5f,  0.5f,  0.5f), new Vector2(0.25f, third)),
            new VertexPositionTexture(new Vector3( 0.5f,  0.5f,  0.5f), new Vector2(0.5f, third)),
            new VertexPositionTexture(new Vector3( 0.5f, -0.5f,  0.5f), new Vector2(0.5f, third * 2)),
            new VertexPositionTexture(new Vector3(-0.5f, -0.5f,  0.5f), new Vector2(0.25f, third * 2)),
            
            // Back face
            new VertexPositionTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector2(1.0f, third * 2)),
            new VertexPositionTexture(new Vector3( 0.5f, -0.5f, -0.5f), new Vector2(0.75f, third * 2)),
            new VertexPositionTexture(new Vector3( 0.5f,  0.5f, -0.5f), new Vector2(0.75f, third)),
            new VertexPositionTexture(new Vector3(-0.5f,  0.5f, -0.5f), new Vector2(1.0f, third)),
            //
            // // Top face
            new VertexPositionTexture(new Vector3(-0.5f,  0.5f, -0.5f), new Vector2(0.25f, 0.0f)),
            new VertexPositionTexture(new Vector3( 0.5f,  0.5f, -0.5f), new Vector2(0.5f, 0.0f)),
            new VertexPositionTexture(new Vector3( 0.5f,  0.5f,  0.5f), new Vector2(0.5f, third)),
            new VertexPositionTexture(new Vector3(-0.5f,  0.5f,  0.5f), new Vector2(0.25f, third)),
            //
            // // Bottom face
            new VertexPositionTexture(new Vector3(-0.5f, -0.5f,  0.5f), new Vector2(0.25f, third * 2)),
            new VertexPositionTexture(new Vector3( 0.5f, -0.5f,  0.5f), new Vector2(0.5f, third * 2)),
            new VertexPositionTexture(new Vector3( 0.5f, -0.5f, -0.5f), new Vector2(0.5f, third * 3)),
            new VertexPositionTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector2(0.25f, third * 3)),
            // //
            // // Left face
            new VertexPositionTexture(new Vector3(-0.5f,  0.5f, -0.5f), new Vector2(0.0f, third)),
            new VertexPositionTexture(new Vector3(-0.5f,  0.5f,  0.5f), new Vector2(0.25f, third)),
            new VertexPositionTexture(new Vector3(-0.5f, -0.5f,  0.5f), new Vector2(0.25f, third * 2)),
            new VertexPositionTexture(new Vector3(-0.5f, -0.5f, -0.5f), new Vector2(0.0f, third * 2)),
            //
            // Right face
            new VertexPositionTexture(new Vector3(0.5f,  0.5f,  0.5f), new Vector2(0.5f, third)),
            new VertexPositionTexture(new Vector3(0.5f,  0.5f, -0.5f), new Vector2(0.75f, third)),
            new VertexPositionTexture(new Vector3(0.5f, -0.5f, -0.5f), new Vector2(0.75f, third * 2)),
            new VertexPositionTexture(new Vector3(0.5f, -0.5f,  0.5f), new Vector2(0.5f, third * 2))
        };
    }
}
