using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectM.Graphics;

namespace ProjectM.Core.Player;

public class TmpPlayerRenderer
{
    private BasicEffect _basicEffect;
    private GraphicsDevice _graphicsDevice;
    private VertexPositionColor[] _boxVertices;
    private Matrix view;
    private Matrix projection;
    
    public TmpPlayerRenderer(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
        
        _basicEffect = new BasicEffect(graphicsDevice)
        {
            VertexColorEnabled = true,
            Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45), graphicsDevice.Viewport.AspectRatio, 1f, 10000f),
            View = Matrix.CreateLookAt(new Vector3(0, 0, 10), Vector3.Zero, Vector3.Up)
        };
    }
    
    public void UpdateViewProjection(Camera camera)
    {
        view = camera.View;
        projection = camera.Projection;
    }
    private VertexPositionColor[] CreateBoxVertices(Vector3 position, Vector3 size)
    {
        Vector3[] corners = new Vector3[8];

        // Define the corners of the box
        corners[0] = position + new Vector3(0, 0, 0);
        corners[1] = position + new Vector3(size.X, 0, 0);
        corners[2] = position + new Vector3(0, -size.Y, 0);
        corners[3] = position + new Vector3(size.X, -size.Y, 0);
        corners[4] = position + new Vector3(0, 0, size.X);
        corners[5] = position + new Vector3(size.X, 0, size.X);
        corners[6] = position + new Vector3(0, -size.Y, size.X);
        corners[7] = position + new Vector3(size.X, -size.Y, size.X);

        // Define the vertices for each face of the box
        List<VertexPositionColor> vertices = new List<VertexPositionColor>();

        // Front face
        vertices.AddRange(CreateQuad(corners[0], corners[1], corners[3], corners[2], Color.White));
        // Back face
        vertices.AddRange(CreateQuad(corners[5], corners[4], corners[6], corners[7], Color.White));
        // Top face
        vertices.AddRange(CreateQuad(corners[2], corners[3], corners[7], corners[6], Color.White));
        // Bottom face
        vertices.AddRange(CreateQuad(corners[0], corners[1], corners[5], corners[4], Color.White));
        // Left face
        vertices.AddRange(CreateQuad(corners[0], corners[2], corners[6], corners[4], Color.White));
        // Right face
        vertices.AddRange(CreateQuad(corners[1], corners[3], corners[7], corners[5], Color.White));

        return vertices.ToArray();
    }

    private VertexPositionColor[] CreateQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4, Color color)
    {
        return new[]
        {
            new VertexPositionColor(v1, color),
            new VertexPositionColor(v2, color),
            new VertexPositionColor(v3, color),
            new VertexPositionColor(v1, color),
            new VertexPositionColor(v3, color),
            new VertexPositionColor(v4, color)
        };
    }

    public void Draw(Vector3 position, Vector3 size)
    {
        _basicEffect.View = view;
        _basicEffect.Projection = projection;
        
        _boxVertices = CreateBoxVertices(position, size);

        
        foreach (var pass in _basicEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            _graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, _boxVertices, 0, _boxVertices.Length / 3);
        }
    }
}