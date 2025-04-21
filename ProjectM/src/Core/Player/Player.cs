using System;
using ProjectM.Core.World;
using ProjectM.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectM.Utils;


namespace ProjectM.Core.Player;

public class Player
{
    public float moveSpeed = 25f;
    
    public Vector3 Position { get; set; }
    public Vector3 Velocity { get; set; }
    public Vector3 Rotation { get; set; }

    private const float Gravity = 9.8f;
    private const float JumpForce = 5f;
    
    public Vector3 Size { get; set; }
    public Vector3 DiameterSize { get; set; }
    // private const float Friction = 0.1f
    
    private int previousScrollValue;

    public readonly Camera Camera;
    
    private Vector2 lastMousePosition = new Vector2(0, 0);
    
    CameraMode cameraMode = CameraMode.FirstPerson;
    
    
    public Player(GraphicsDevice graphicsDevice)
    {
        Camera = new Camera(graphicsDevice);
        
        Position = new Vector3(0, 75, 0);
        Size = new Vector3(0.5f, 1.8f, 0.5f);
        
        Camera.Position = Position;
        lastMousePosition = new Vector2(graphicsDevice.Viewport.Width / 2, graphicsDevice.Viewport.Height / 2);
        Mouse.SetPosition((int)lastMousePosition.X, (int)lastMousePosition.Y);
    }
    
    public void Update(GameTime gameTime, World.World world)
    {
        var velocity = Velocity;
        // velocity.Y -= Gravity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        Velocity = velocity;        
        
        HandleInput(gameTime);   
        
        Vector3 newPosition = Position + Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        Vector3 correction = HandleCollisions(newPosition, world);
        
        Position = newPosition + correction;

        if (correction != Vector3.Zero)
        {
            Velocity = Vector3.Zero;
        }



        // if (cameraMode == CameraMode.FirstPerson)
        // {
        //     Camera.Position = Position + new Vector3(0, 1.5f, 0);
        // }
        Camera.SetRotation(Rotation);
        
        if (cameraMode == CameraMode.FirstPerson)
        {
            Camera.SetPosition(Position);
        }
        else
        {
            Camera.SetPosition(
                Position + new Vector3(
                    (float)Math.Sin(Rotation.Y) * 5,
                    1.5f,
                    (float)Math.Cos(Rotation.Y) * 5)
                );

        }

    }

    private Vector3 HandleCollisions(Vector3 newPosition, World.World world)
    {
        Vector3 correction = Vector3.Zero;
        

        
        Vector3 tmp = newPosition - new Vector3(0, -Size.Y, 0);
        // Console.WriteLine(tmp);
        int block = world.GetBlockAt(tmp);
        if (block != 0)
        {
            Console.WriteLine("Collision detected");
            correction.Y = -((int)(tmp.Y + 1) - Position.Y);
        }
        
        
        return correction;
    }

    private void HandleInput(GameTime gameTime)
    {
        var keyboardState = Keyboard.GetState();
        float rotationSpeed = 0.4f;
        var mouseState = Mouse.GetState();
        
        Vector3 move = new Vector3(0, 0, 0);

        moveSpeed += mouseState.ScrollWheelValue - previousScrollValue;
        previousScrollValue = mouseState.ScrollWheelValue;
            
        // cameraMode = CameraMode.FirstPerson;
        if (Input.IsKeyPressed(Keys.F5))
        {
            cameraMode += 1;
            if (cameraMode > CameraMode.FreeCamera)
                cameraMode = CameraMode.FirstPerson;
            Console.WriteLine("Camera mode: " + cameraMode);
        }
        
        
        if (keyboardState.IsKeyDown(Keys.Z))
            move.Z += moveSpeed;
        if (keyboardState.IsKeyDown(Keys.S))
            move.Z -= moveSpeed;
        if (keyboardState.IsKeyDown(Keys.Q))
            move.X += moveSpeed;
        if (keyboardState.IsKeyDown(Keys.D))
            move.X -= moveSpeed;
        if (keyboardState.IsKeyDown(Keys.Space))
            move.Y += moveSpeed;
        if (keyboardState.IsKeyDown(Keys.LeftShift))
            move.Y -= moveSpeed;
        
        move *= (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        Vector3 forwardNoPitch = Vector3.Transform(Vector3.Forward, Matrix.CreateRotationY(Camera.Rotation.Y));
        forwardNoPitch = Vector3.Normalize(forwardNoPitch);

        Position += new Vector3(
            forwardNoPitch.X * move.Z + forwardNoPitch.Z * move.X,
            move.Y,
            forwardNoPitch.Z * move.Z - forwardNoPitch.X * move.X
        );
        
        Vector2 mouseDelta = new Vector2(mouseState.X, mouseState.Y) - lastMousePosition;
        if ( mouseDelta != Vector2.Zero)
            Rotate(
                new Vector3(-mouseDelta.Y * rotationSpeed, -mouseDelta.X * rotationSpeed,0)
                * (float)gameTime.ElapsedGameTime.TotalSeconds
                );
        
        // lastMousePosition = new Vector2(mouseState.X, mouseState.Y);
        Mouse.SetPosition((int)lastMousePosition.X, (int)lastMousePosition.Y);
    }
    
    public Vector3 Rotate(Vector3 rotation)
    {
        Rotation += rotation;
        
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
        
        return Rotation;
    }
    
    public Vector3 GetPosition()
    {
        return Position;
    }
}

public enum CameraMode
{
    FirstPerson,
    ThirdPerson,
    FreeCamera
}
