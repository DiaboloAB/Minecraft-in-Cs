using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace App1.Graphics;

public class Camera
{
    public Matrix View { get; private set; }
    public Matrix Projection { get; private set; }
    public Vector3 Position { get; set; }
    
    //360 degrees
    public Vector3 Rotation { get; private set; }
    
    public string Facing { get; private set; } = "north";
    public float FieldOfView { get; private set; }
    
    public Camera(GraphicsDevice graphicsDevice)
    {
        FieldOfView = 90f;
        Position = new Vector3(0, 0, 5);
        Rotation = new Vector3(0, 0, 0);
        
        Console.WriteLine("Camera created at position " + Position);
        
        UpdateViewMatrix();
        UpdateProjectionMatrix(graphicsDevice);
    }

    private void UpdateViewMatrix()
    {
        Vector3 target = Position + Vector3.Transform(Vector3.Forward, Matrix.CreateRotationX(Rotation.X) * Matrix.CreateRotationY(Rotation.Y) * Matrix.CreateRotationZ(Rotation.Z));
        View = Matrix.CreateLookAt(Position, target, Vector3.Up);
    }

    private void UpdateProjectionMatrix(GraphicsDevice graphicsDevice)
    {
        Projection = Matrix.CreatePerspectiveFieldOfView(
            MathHelper.ToRadians(FieldOfView),
            graphicsDevice.Viewport.AspectRatio,
            0.1f,
            1000f
        );
    }

    private void UpdateFacing()
    {
        if (Rotation.Y > -Math.PI / 4 && Rotation.Y < Math.PI / 4)
        {
            Facing = "north";
        }
        else if (Rotation.Y > Math.PI / 4 && Rotation.Y < 3 * Math.PI / 4)
        {
            Facing = "west";
        }
        else if (Rotation.Y < -Math.PI / 4 && Rotation.Y > -3 * Math.PI / 4)
        {
            Facing = "east";
        }
        else
        {
            Facing = "south";
        }
    }
    
    public void Rotate(Vector3 rotation)
    {
        Rotation += rotation;
        
        Console.WriteLine("Rotation: " + Rotation);
        
        
        Vector3 Rotation360 = new Vector3(
            MathHelper.WrapAngle(Rotation.X),
            MathHelper.WrapAngle(Rotation.Y),
            MathHelper.WrapAngle(Rotation.Z)
        );
        
        if (Rotation.Y > Math.PI)
        {
            Rotation = new Vector3(Rotation.X, Rotation.Y - MathHelper.TwoPi, Rotation.Z);
        }
        else if (Rotation.Y < -Math.PI)
        {
            Rotation = new Vector3(Rotation.X, Rotation.Y + MathHelper.TwoPi, Rotation.Z);
        }
        
        
        
        if (Rotation360.X > MathHelper.PiOver2 - 0.1f)
        {
            Rotation = new Vector3(MathHelper.PiOver2 - 0.1f, Rotation.Y, Rotation.Z);
        }
        else if (Rotation360.X < -MathHelper.PiOver2 + 0.1f)
        {
            Rotation = new Vector3(-MathHelper.PiOver2 + 0.1f, Rotation.Y, Rotation.Z);
        }
        
        UpdateViewMatrix();
        UpdateFacing();
    }
    public void Move(Vector3 movement)
    {
        Vector3 forwardNoPitch = Vector3.Transform(Vector3.Forward, Matrix.CreateRotationY(Rotation.Y));
        forwardNoPitch = Vector3.Normalize(forwardNoPitch);

        Position += new Vector3(
            forwardNoPitch.X * movement.Z + forwardNoPitch.Z * movement.X,
            movement.Y,
            forwardNoPitch.Z * movement.Z - forwardNoPitch.X * movement.X
        );
        UpdateViewMatrix();
    }
    
}