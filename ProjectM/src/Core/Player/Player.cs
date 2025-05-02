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
    private float WALKING_SPEED = 2.317f;
    private float SPRINTING_SPEED = 3f;
    private Vector3 InputDirection;
    float rotationSpeed = 0.4f;
    
    float targetFov = 90f;
    
    public Vector3 Rotation { get; set; }

    private float TargetSpeed;
    private float Speed;
    private bool Sprinting;
    
    private int previousScrollValue;

    public readonly Camera Camera;
    
    private Vector2 lastMousePosition = new Vector2(0, 0);
    
    CameraMode cameraMode = CameraMode.FirstPerson;
    
    TmpPlayerRenderer tmpPlayerRenderer;
    
    public Player(GraphicsDevice graphicsDevice) 
        : base(new Vector3(0, 60, 0), new Vector3(0.5f, 1.8f, 0.5f))
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
            
            Acceleration = new Vector3(
                forwardNoPitch.X * InputDirection.Z + forwardNoPitch.Z * InputDirection.X,
                Acceleration.Y,
                forwardNoPitch.Z * InputDirection.Z - forwardNoPitch.X * InputDirection.X
            );
            Acceleration *= multiplier;
        }

        if (InputDirection.Y > 0 && !Flying && Grounded)
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
            InputDirection.Z += Speed;
        if (keyboardState.IsKeyDown(Keys.S))
            InputDirection.Z -= Speed;
        if (keyboardState.IsKeyDown(Keys.Q))
            InputDirection.X += Speed;
        if (keyboardState.IsKeyDown(Keys.D))
            InputDirection.X -= Speed;
        if (keyboardState.IsKeyDown(Keys.Space))
            InputDirection.Y += Speed;
        if (keyboardState.IsKeyDown(Keys.F))
            InputDirection.Y -= Speed;

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
