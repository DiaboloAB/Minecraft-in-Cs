using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace App1.Graphics;

public class Camera
{
    public Matrix View { get; private set; }
    public Matrix Projection { get; private set; }
    public Vector3 Position { get; private set; }
    public Vector3 Direction { get; private set; }
    
    public Camera(GraphicsDevice graphicsDevice)
    {
        Position = new Vector3(0, 50, 100);
        Direction = Vector3.Forward;
        
        Console.WriteLine("Camera created at position " + Position);
        
        UpdateViewMatrix();
        UpdateProjectionMatrix(graphicsDevice);
    }

    private void UpdateViewMatrix()
    {
        Vector3 target = Position + Direction;
        View = Matrix.CreateLookAt(Position, target, Vector3.Up);
    }

    private void UpdateProjectionMatrix(GraphicsDevice graphicsDevice)
    {
        Projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(45),
            graphicsDevice.Viewport.AspectRatio,
            0.1f,
            1000f
        );
    }

    public void Rotate(Vector3 rotation)
    {
        Matrix rotationMatrix = Matrix.CreateFromYawPitchRoll(rotation.Y, rotation.X, rotation.Z);
        Direction = Vector3.Transform(Direction, rotationMatrix);
        
        
        UpdateViewMatrix();
    }
    public void Move(Vector3 movement)
    {
        Vector3 direction = Vector3.Normalize(Direction);
        
        Position -= direction * movement.Z;
        Position -= Vector3.Cross(Vector3.Up, direction) * movement.X;
        Position += Vector3.Up * movement.Y;
        
        UpdateViewMatrix();
    }
    
}