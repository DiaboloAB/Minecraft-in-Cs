using System;
using System.Numerics;
using App1.Graphics;
using Microsoft.Xna.Framework.Graphics;
using Vector3 = Microsoft.Xna.Framework.Vector3;
using VertexPosition = Microsoft.Xna.Framework.Graphics.VertexPosition;

namespace App1.Utils;

public class OrientationGraph
{
    public static void DrawOrientationGraph(GraphicsDevice graphicsDevice, Camera camera, Vector2 pos)
    {
        // draw red for forward /blue for right/green for up lines for orientation

        Vector3 cameraRotation = camera.Rotation;
        
        Vector2 forward = new Vector2((float)Math.Cos(cameraRotation.Y), (float)Math.Sin(cameraRotation.Y));
        
        Vector2 right = new Vector2((float)Math.Cos(cameraRotation.Y + Math.PI / 2), (float)Math.Sin(cameraRotation.Y + Math.PI / 2));
        
        Vector2 up = new Vector2((float)Math.Cos(cameraRotation.X), (float)Math.Sin(cameraRotation.X));
        
        VertexPosition[] vertices = new VertexPosition[6];
        
        vertices[0] = new VertexPosition(new Vector3(pos.X, pos.Y, 0));
        vertices[1] = new VertexPosition(new Vector3(pos.X + forward.X, pos.Y + forward.Y, 0));
        
        vertices[2] = new VertexPosition(new Vector3(pos.X, pos.Y, 0));
        vertices[3] = new VertexPosition(new Vector3(pos.X + right.X, pos.Y + right.Y, 0));
        
        

    }
}