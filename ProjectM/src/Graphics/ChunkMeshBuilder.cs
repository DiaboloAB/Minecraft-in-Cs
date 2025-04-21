using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ProjectM.Core.Block;
using ProjectM.Core.World;

namespace ProjectM.Graphics;

public class ChunkRenderer
{
    private BasicEffect _basicEffect;
    private GraphicsDevice _graphicsDevice;
    private int _chunkSize = 16; // Example chunk size
    private Matrix view;
    private Matrix projection;

    public ChunkRenderer(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
        _basicEffect = new BasicEffect(graphicsDevice);
        _basicEffect.VertexColorEnabled = true;
    }
    
    public void UpdateViewProjection(Camera camera)
    {
        view = camera.View;
        projection = camera.Projection;
    }

    public void DrawChunkBorderGrid(Vector3 chunkPosition, int height)
    {
        _basicEffect.View = view;
        _basicEffect.Projection = projection;

        var vertices = new List<VertexPositionColor>();

        // Add chunk border vertices
        AddChunkBorderVertices(vertices, chunkPosition, height);

        // Add blue grid vertices
        AddGridVertices(vertices, chunkPosition, height, Color.Yellow, 2, _chunkSize - 1, 4);

        // Add horizontal lines on the side
        AddHorizontalSideLines(vertices, chunkPosition, -100, 100, 4);

        // Draw the lines
        DrawLines(vertices);
    }

    private void AddChunkBorderVertices(List<VertexPositionColor> vertices, Vector3 chunkPosition, int height)
    {
        vertices.AddRange(new[]
        {
            CreateVertex(chunkPosition.X, height - 50, chunkPosition.Z, Color.Blue),
            CreateVertex(chunkPosition.X, height + 50, chunkPosition.Z, Color.Blue),

            CreateVertex(chunkPosition.X + _chunkSize, height - 50, chunkPosition.Z, Color.Blue),
            CreateVertex(chunkPosition.X + _chunkSize, height + 50, chunkPosition.Z, Color.Blue),

            CreateVertex(chunkPosition.X, height - 50, chunkPosition.Z + _chunkSize, Color.Blue),
            CreateVertex(chunkPosition.X, height + 50, chunkPosition.Z + _chunkSize, Color.Blue),

            CreateVertex(chunkPosition.X + _chunkSize, height - 50, chunkPosition.Z + _chunkSize, Color.Blue),
            CreateVertex(chunkPosition.X + _chunkSize, height + 50, chunkPosition.Z + _chunkSize, Color.Blue)
        });
    }

    private void AddGridVertices(List<VertexPositionColor> vertices, Vector3 chunkPosition, int height, Color color, int start, int end, int step)
    {
        for (int i = start; i < end; i += step)
        {
            vertices.AddRange(new[]
            {
                CreateVertex(chunkPosition.X + i, height - 50, chunkPosition.Z, color),
                CreateVertex(chunkPosition.X + i, height + 50, chunkPosition.Z, color),

                CreateVertex(chunkPosition.X, height - 50, chunkPosition.Z + i, color),
                CreateVertex(chunkPosition.X, height + 50, chunkPosition.Z + i, color),

                CreateVertex(chunkPosition.X + _chunkSize, height - 50, chunkPosition.Z + i, color),
                CreateVertex(chunkPosition.X + _chunkSize, height + 50, chunkPosition.Z + i, color),

                CreateVertex(chunkPosition.X + i, height - 50, chunkPosition.Z + _chunkSize, color),
                CreateVertex(chunkPosition.X + i, height + 50, chunkPosition.Z + _chunkSize, color)
            });
        }
    }

    private void AddHorizontalSideLines(List<VertexPositionColor> vertices, Vector3 chunkPosition, int start, int end, int step)
    {
        for (int i = start; i < end; i += step)
        {
            vertices.AddRange(new[]
            {
                CreateVertex(chunkPosition.X, i, chunkPosition.Z, Color.Yellow),
                CreateVertex(chunkPosition.X + _chunkSize, i, chunkPosition.Z, Color.Yellow),

                CreateVertex(chunkPosition.X, i, chunkPosition.Z + _chunkSize, Color.Yellow),
                CreateVertex(chunkPosition.X + _chunkSize, i, chunkPosition.Z + _chunkSize, Color.Yellow),

                CreateVertex(chunkPosition.X, i, chunkPosition.Z + _chunkSize, Color.Yellow),
                CreateVertex(chunkPosition.X, i, chunkPosition.Z, Color.Yellow),

                CreateVertex(chunkPosition.X + _chunkSize, i, chunkPosition.Z + _chunkSize, Color.Yellow),
                CreateVertex(chunkPosition.X + _chunkSize, i, chunkPosition.Z, Color.Yellow)
            });
        }
    }

    private VertexPositionColor CreateVertex(float x, float y, float z, Color color)
    {
        return new VertexPositionColor(new Vector3(x, y, z), color);
    }

    private void DrawLines(List<VertexPositionColor> vertices)
    {
        foreach (var pass in _basicEffect.CurrentTechnique.Passes)
        {
            pass.Apply();
            _graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices.ToArray(), 0, vertices.Count / 2);
        }
    }
}