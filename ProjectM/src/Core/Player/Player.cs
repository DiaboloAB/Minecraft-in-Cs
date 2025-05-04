using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using ProjectM.Core.World;
using ProjectM.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ProjectM.Core.Collisions;
using ProjectM.Utils;


namespace ProjectM.Core.Player;

public class Player : Entity
{
    private float WALKING_SPEED = 4.317f;
    private float SPRINTING_SPEED = 6f;
    private Vector3 InputDirection;
    float rotationSpeed = 0.20f;
    
    float targetFov = 90f;
    
    public Vector3 Rotation = new Vector3(0, -MathHelper.PiOver2, 0);

    private float TargetSpeed;
    private float Speed;
    private bool Sprinting;
    
    private int previousScrollValue;

    public readonly Camera Camera;
    
    private Vector2 lastMousePosition = new Vector2(0, 0);
    
    CameraMode cameraMode = CameraMode.FirstPerson;
    
    TmpPlayerRenderer tmpPlayerRenderer;
    
    public Player(GraphicsDevice graphicsDevice) 
        : base(new Vector3(16, 60, 16), new Vector3(0.5f, 1.8f, 0.5f))
    {
        Camera = new Camera(graphicsDevice);
        
        Size = new Vector3(0.5f, 1.8f, 0.5f);
        
        Camera.Position = Position;
        lastMousePosition = new Vector2(graphicsDevice.Viewport.Width / 2, graphicsDevice.Viewport.Height / 2);
        Mouse.SetPosition((int)lastMousePosition.X, (int)lastMousePosition.Y);
        
        tmpPlayerRenderer = new TmpPlayerRenderer(graphicsDevice);
        
        TargetSpeed = WALKING_SPEED;
        Speed = TargetSpeed;
    }
    
    public void Update(GameTime gameTime, World.World world)
    {
        HandleInput(gameTime);
        
        if ((float)gameTime.ElapsedGameTime.TotalSeconds * 20 > 1)
            Speed = TargetSpeed;
        else 
            Speed += (TargetSpeed - Speed) * (float)gameTime.ElapsedGameTime.TotalSeconds * 20;
        float multiplier = Speed * (Flying ? 2 : 1);

        if (Flying && InputDirection.Y != 0)
            Acceleration.Y = InputDirection.Y * multiplier;
        
        if (InputDirection.X != 0 || InputDirection.Z != 0)
        {
            Vector3 forwardNoPitch = Vector3.Transform(Vector3.Forward, Matrix.CreateRotationY(Camera.Rotation.Y));
            forwardNoPitch = Vector3.Normalize(forwardNoPitch);

            Vector3 right = Vector3.Transform(Vector3.Right, Matrix.CreateRotationY(Camera.Rotation.Y));
            right = Vector3.Normalize(right);

            Acceleration = new Vector3(
                forwardNoPitch.X * InputDirection.X + right.X * InputDirection.Z,
                0,
                forwardNoPitch.Z * InputDirection.X + right.Z * InputDirection.Z
            );
            Acceleration *= multiplier;
        }

        if (InputDirection.Y > 0 && !Flying)
        {
            Jump();
        }
        
        base.Update(gameTime, world);       
        // Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        
        
        // cam update
        targetFov = Sprinting ? 100f : 90f;
        Camera.SetFov(MathHelper.Lerp(Camera.FieldOfView, targetFov, 0.2f));
        Camera.SetRotation(Rotation);
        if (cameraMode == CameraMode.FirstPerson)
            Camera.SetPosition(Position + new Vector3(0, Size.Y / 2 - 0.2f, 0));
        else
        {
            Matrix rotationMatrix = Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, 0);
            Vector3 rotatedOffset = Vector3.Transform(new Vector3(0, 0, -5), rotationMatrix);
            Camera.SetPosition(
                Position + new Vector3(0, Size.Y / 2 - 0.2f, 0) - rotatedOffset
            );
        }
    }

    private void HandleInput(GameTime gameTime)
    {
        var keyboardState = Keyboard.GetState();
        var mouseState = Mouse.GetState();
        InputDirection = Vector3.Zero;
            
        // cameraMode = CameraMode.FirstPerson;
        if (Input.IsKeyPressed(Keys.F5))
        {
            cameraMode += 1;
            if (cameraMode > CameraMode.FreeCamera)
                cameraMode = CameraMode.FirstPerson;
            Console.WriteLine("Camera mode: " + cameraMode);
        }
        
        
        if (keyboardState.IsKeyDown(Keys.Z))
            InputDirection.X = 1;
        if (keyboardState.IsKeyDown(Keys.Q))
            InputDirection.Z = -1;
        if (keyboardState.IsKeyDown(Keys.S))
            InputDirection.X = -1;
        if (keyboardState.IsKeyDown(Keys.D))
            InputDirection.Z = 1;
        if (keyboardState.IsKeyDown(Keys.Space))
            InputDirection.Y = 1;
        if (keyboardState.IsKeyDown(Keys.F))
            InputDirection.Y = -1;

        if (keyboardState.IsKeyDown(Keys.LeftShift))
        {
            TargetSpeed = SPRINTING_SPEED;
            Sprinting = true;
        }
        else
        {
            TargetSpeed = WALKING_SPEED;
            Sprinting = false;
        }

        if (Input.IsKeyPressed(Keys.F7))
        {
            Flying = !Flying;
        }
        
        Vector2 mouseDelta = new Vector2(mouseState.X, mouseState.Y) - lastMousePosition;
        if ( mouseDelta != Vector2.Zero)
            Rotate(
                new Vector3(-mouseDelta.Y * rotationSpeed, -mouseDelta.X * rotationSpeed,0)
                * (float)gameTime.ElapsedGameTime.TotalSeconds
                );
        
        Mouse.SetPosition((int)lastMousePosition.X, (int)lastMousePosition.Y);
    }
    
    public Vector3 Rotate(Vector3 rotation)
    {
        Rotation += rotation;

        Rotation.X = MathHelper.Clamp(Rotation.X, -MathHelper.PiOver2 + 0.1f, MathHelper.PiOver2 - 0.1f);

        Rotation.Y = MathHelper.WrapAngle(Rotation.Y);

        return Rotation;
    }
    
    public void Draw()
    {
        tmpPlayerRenderer.UpdateViewProjection(Camera);
        if (cameraMode != CameraMode.FirstPerson)
        {
            tmpPlayerRenderer.Draw(
                Position + new Vector3(-Size.X / 2, Size.Y / 2, -Size.Z / 2),
                Size
            );
        }
        
    }
}

public enum CameraMode
{
    FirstPerson,
    ThirdPerson,
    FreeCamera
}
