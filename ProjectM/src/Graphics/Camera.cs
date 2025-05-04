using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace ProjectM.Graphics;

public class Camera
{
    public Matrix View { get; private set; }
    public Matrix Projection { get; private set; }
    public Vector3 Position { get; set; }
    
    //360 degrees
    public Vector3 Rotation { get; private set; }
    
    public string Facing { get; private set; } = "north";
    public float FieldOfView { get; private set; }
    
    private GraphicsDevice graphicsDevice;
    
    public Camera(GraphicsDevice graphicsDevice)
    {
        FieldOfView = 90f;
        Position = new Vector3(0, 0, 5);
        Rotation = new Vector3(0, 0, 0);
        this.graphicsDevice = graphicsDevice;
        
        Console.WriteLine("Camera created at position " + Position);
        
        UpdateViewMatrix();
        UpdateProjectionMatrix(graphicsDevice);
    }

    private void UpdateViewMatrix()
    {
        Vector3 target = Position + Vector3.Transform(Vector3.Forward, Matrix.CreateRotationX(Rotation.X) * Matrix.CreateRotationY(Rotation.Y) * Matrix.CreateRotationZ(Rotation.Z));
        View = Matrix.CreateLookAt(Position, target, Vector3.Up);
    }
    
    public void SetFov(float fov)
    {
        FieldOfView = fov;
        UpdateProjectionMatrix(graphicsDevice);
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
        Vector3 forward = Vector3.Transform(Vector3.Forward, Matrix.CreateRotationY(Rotation.Y));
        Facing = "";

        if (forward.X > 0.5)
            Facing += "North ";
        if (forward.X < -0.5)
            Facing += "South ";
        if (forward.Z > 0.5)
            Facing += "East ";
        if (forward.Z < -0.5)
            Facing += "West ";
    }
    
    public void SetRotation(Vector3 rotation)
    {
        Rotation = rotation;
        
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
    
    public void SetPosition(Vector3 position)
    {
        Position = position;
        UpdateViewMatrix();
    }
    
}