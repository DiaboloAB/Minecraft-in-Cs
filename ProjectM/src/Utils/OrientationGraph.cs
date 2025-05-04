using System;
using ProjectM.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Vector2 = System.Numerics.Vector2;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using VertexPosition = Microsoft.Xna.Framework.Graphics.VertexPosition;

namespace ProjectM.Utils;

public class OrientationGraph
{
    private GraphicsDevice graphicsDevice;
    private ContentManager contentManager;
    private VertexBuffer vertexBuffer;
    private IndexBuffer indexBuffer;
    private BasicEffect basicEffect;
    public OrientationGraph(GraphicsDevice graphicsDevice)
    {
        this.graphicsDevice = graphicsDevice;
        
        basicEffect = new BasicEffect(graphicsDevice){
            VertexColorEnabled = true
        };
    }

    private void InitializeVertices()
    {
        
    }
    
    public void DrawOrientationGraph(Camera camera)
    {
        var originalDepthStencilState = graphicsDevice.DepthStencilState;

        // Disable depth buffering
        graphicsDevice.DepthStencilState = DepthStencilState.None;
        Vector3 pos = camera.Position;
        Vector3 rotation = camera.Rotation;
        
        Vector3 forward = Vector3.Transform(Vector3.Forward, Matrix.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z)); 
        
        Vector3 startPoint = pos + forward * 10;
        
        Vector3 endPoint = startPoint + Vector3.Backward;

        basicEffect.View = camera.View;
        basicEffect.Projection = camera.Projection;
        
        basicEffect.CurrentTechnique.Passes[0].Apply();
        var vertices = new[] { new VertexPositionColor(startPoint, Color.Green),  new VertexPositionColor(endPoint, Color.Green) };
        graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
        
        endPoint = startPoint + Vector3.Right;
        vertices = new[] { new VertexPositionColor(startPoint, Color.Red),  new VertexPositionColor(endPoint, Color.Red) };
        graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
        
        endPoint = startPoint + Vector3.Up;
        
        vertices = new[] { new VertexPositionColor(startPoint, Color.Blue),  new VertexPositionColor(endPoint, Color.Blue) };
        graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList, vertices, 0, 1);
        graphicsDevice.DepthStencilState = originalDepthStencilState;
    }
}