using System.Collections.Generic;
using System.Numerics;
using System.Runtime;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = Microsoft.Xna.Framework.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;

namespace ProjectM.Graphics;

public struct CubeData
{
    public Vector3 Position { get; set; }
    public Vector3 Scale { get; set; }
    public Texture2D Texture { get; set; }
    // public Vector3 Name { get; set; }

}

public struct FaceData
{
    public Vector3 Position { get; set; }
    public Vector2 TexCoord { get; set; }
    public short Orientation { get; set; }
}

public class Cube
{
    public static short[] CreateIndices()
    {
        return new short[]
        {
            0, 1, 2, 0, 2, 3, // Front
            4, 5, 6, 4, 6, 7, // Back
            8, 9, 10, 8, 10, 11, // Top
            12, 13, 14, 12, 14, 15, // Bottom
            16, 17, 18, 16, 18, 19, // Left
            20, 21, 22, 20, 22, 23 // Right
        };
    }
        
    //all 4 vertices of all faces
    public static readonly Vector3[][] FaceVertices = new Vector3[][]
    {
        new Vector3[] 
        {
            new Vector3( 1,  1,  1),
            new Vector3( 1, 0,  1),
            new Vector3(0, 0,  1),
            new Vector3(0,  1,  1),
        },
        // Back face
        new Vector3[]
        {
            new Vector3(0,  1, 0),
            new Vector3(0, 0, 0),
            new Vector3( 1, 0, 0),
            new Vector3( 1,  1, 0),
        },
        // Top face
        new Vector3[]
        {
            new Vector3( 1,  1, 0),
            new Vector3( 1,  1,  1),
            new Vector3(0,  1,  1),
            new Vector3(0,  1, 0),
        },
        // Bottom face
        new Vector3[]
        {
            new Vector3( 1, 0,  1),
            new Vector3( 1, 0, 0),
            new Vector3(0, 0, 0),
            new Vector3(0, 0,  1),
        },
        // Left face
        new Vector3[]
        {
            new Vector3(0,  1,  1),
            new Vector3(0, 0,  1),
            new Vector3(0, 0, 0),
            new Vector3(0,  1, 0),
        },
        // Right face
        new Vector3[]
        {
            new Vector3(1,  1, 0),
            new Vector3(1, 0, 0),
            new Vector3(1, 0,  1),
            new Vector3(1,  1,  1),
        }
    };
    
    public static VertexPositionTexture[] CreateFaceVertices(int orientation, float texSize)
    {
       return new VertexPositionTexture[] 
       {
           new VertexPositionTexture(FaceVertices[orientation][0], new Vector2(0, 0)),
           new VertexPositionTexture(FaceVertices[orientation][1], new Vector2(texSize, 0)),
           new VertexPositionTexture(FaceVertices[orientation][2], new Vector2(texSize, texSize)),
           new VertexPositionTexture(FaceVertices[orientation][3], new Vector2(0, texSize))
       };
    }
    
    public static VertexPositionTexture[] CreateVertices()
    {
        float third = 1.0f / 3.0f;
        return new VertexPositionTexture[]
        {
            // Front face
            new VertexPositionTexture(new Vector3(0,  1,  1), new Vector2(0.25f, third)),
            new VertexPositionTexture(new Vector3( 1,  1,  1), new Vector2(1, third)),
            new VertexPositionTexture(new Vector3( 1, 0,  1), new Vector2(1, third * 2)),
            new VertexPositionTexture(new Vector3(0, 0,  1), new Vector2(0.25f, third * 2)),
            
            // Back face
            new VertexPositionTexture(new Vector3(0, 0, 0), new Vector2(1.0f, third * 2)),
            new VertexPositionTexture(new Vector3( 1, 0, 0), new Vector2(0.75f, third * 2)),
            new VertexPositionTexture(new Vector3( 1,  1, 0), new Vector2(0.75f, third)),
            new VertexPositionTexture(new Vector3(0,  1, 0), new Vector2(1.0f, third)),
            // Top face
            new VertexPositionTexture(new Vector3(0,  1, 0), new Vector2(0.25f, 0.0f)),
            new VertexPositionTexture(new Vector3( 1,  1, 0), new Vector2(1, 0.0f)),
            new VertexPositionTexture(new Vector3( 1,  1,  1), new Vector2(1, third)),
            new VertexPositionTexture(new Vector3(0,  1,  1), new Vector2(0.25f, third)),
            
            // Bottom face
            new VertexPositionTexture(new Vector3(0, 0,  1), new Vector2(0.25f, third * 2)),
            new VertexPositionTexture(new Vector3( 1, 0,  1), new Vector2(1, third * 2)),
            new VertexPositionTexture(new Vector3( 1, 0, 0), new Vector2(1, third * 3)),
            new VertexPositionTexture(new Vector3(0, 0, 0), new Vector2(0.25f, third * 3)),
            
            // Left face
            new VertexPositionTexture(new Vector3(0,  1, 0), new Vector2(0.0f, third)),
            new VertexPositionTexture(new Vector3(0,  1,  1), new Vector2(0.25f, third)),
            new VertexPositionTexture(new Vector3(0, 0,  1), new Vector2(0.25f, third * 2)),
            new VertexPositionTexture(new Vector3(0, 0, 0), new Vector2(0.0f, third * 2)),
            
            // Right face
            new VertexPositionTexture(new Vector3(1,  1,  1), new Vector2(1, third)),
            new VertexPositionTexture(new Vector3(1,  1, 0), new Vector2(0.75f, third)),
            new VertexPositionTexture(new Vector3(1, 0, 0), new Vector2(0.75f, third * 2)),
            new VertexPositionTexture(new Vector3(1, 0,  1), new Vector2(1, third * 2))
        };
    }

    public static List<Vector2> GrassTexCoords = new List<Vector2>
    {
        new Vector2(0 + 16.0f,0),
        new Vector2(0 + 16.0f,0),
        new Vector2(0,0),
        new Vector2(0 + 16.0f * 2,0),
        new Vector2(0 + 16.0f,0),
        new Vector2(0 + 16.0f,0)
    };
    
}
