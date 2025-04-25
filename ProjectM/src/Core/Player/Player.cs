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

public class Player
{
    public Vector3 Position = new Vector3(10, 60, 10);
    public Vector3 Velocity = new Vector3(0, 0, 0);
    public Vector3 Rotation { get; set; }

    private const float Gravity = 9.8f;
    private const float JumpForce = 5f;
    
    private const float Speed = 25f;
    
    public Vector3 Size { get; set; }
    public Vector3 DiameterSize { get; set; }
    // private const float Friction = 0.1f
    
    private int previousScrollValue;

    public readonly Camera Camera;
    
    private Vector2 lastMousePosition = new Vector2(0, 0);
    
    CameraMode cameraMode = CameraMode.FirstPerson;
    
    TmpPlayerRenderer tmpPlayerRenderer;
    
    Collider playerCollider;
    
    public Player(GraphicsDevice graphicsDevice)
    {
        Camera = new Camera(graphicsDevice);
        
        Size = new Vector3(0.5f, 1.8f, 0.5f);
        
        Camera.Position = Position;
        lastMousePosition = new Vector2(graphicsDevice.Viewport.Width / 2, graphicsDevice.Viewport.Height / 2);
        Mouse.SetPosition((int)lastMousePosition.X, (int)lastMousePosition.Y);
        
        tmpPlayerRenderer = new TmpPlayerRenderer(graphicsDevice);
    }
    
    public void Update(GameTime gameTime, World.World world)
    {
        // var velocity = Velocity;
        // float gravitationalForce = 5f * Gravity;
        // velocity.Y -= gravitationalForce * (float)gameTime.ElapsedGameTime.TotalSeconds;
        //
        // Velocity = velocity;        
        
        HandleInput(gameTime);
        
        playerCollider = new Collider(
            Position - Size / 2,
            Position + Size / 2
        );
        
        HandleCollisions(gameTime, world);
        
        Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        Camera.SetRotation(Rotation);
        
        if (cameraMode == CameraMode.FirstPerson)
            Camera.SetPosition(Position);
        else
        {
            Matrix rotationMatrix = Matrix.CreateFromYawPitchRoll(Rotation.Y, Rotation.X, 0);
            Vector3 rotatedOffset = Vector3.Transform(new Vector3(0, 0, -5), rotationMatrix);
            Camera.SetPosition(
                Position - rotatedOffset
            );
        }
    }

    private void HandleCollisions(GameTime gameTime, World.World world)
    {
        // thx to https://www.youtube.com/watch?v=fWkbIOna6RA&t=3s
        for (int idx = 0; idx < 3; idx++)
        {
            Vector3 computedVelo =  Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            int stepX = computedVelo.X > 0 ? 1 : -1;
            int stepY = computedVelo.Y > 0 ? 1 : -1;
            int stepZ = computedVelo.Z > 0 ? 1 : -1;
            
            int stepsXz = (int)(Size.X + 1);
            int stepsY = (int)Size.Y + 1;
            
            List<Tuple<float, Vector3>> potentialCollisions = new List<Tuple<float, Vector3>>();

            Vector3 pos = new Vector3((int)Position.X, (int)Position.Y, (int)Position.Z);
            
            Vector3 newPos = new Vector3(
                (int)(Position.X + computedVelo.X),
                (int)(Position.Y + computedVelo.Y),
                (int)(Position.Z + computedVelo.Z)
            );

            for (int i = (int)pos.X - stepX * (stepsXz + 1); i != (int)newPos.X + stepX * (stepsXz + 2); i += stepX)
            {
                for (int j = (int)pos.Y - stepY * (stepsY + 1); j != (int)newPos.Y + stepY * (stepsY + 2); j += stepY)
                {
                    for (int k = (int)pos.Z - stepZ * (stepsXz + 1); k != (int)newPos.Z + stepZ * (stepsXz + 2); k += stepZ)
                    {
                        Bloc bloc = world.GetBloc(new Vector3(i, j, k));
                        
                        if (bloc == null)
                            continue;
                        if (bloc.Type == BlocType.Air)
                            continue;
                        var (entryTime, normal) = playerCollider.Collide(bloc.getCollider(), computedVelo);
                        if (entryTime < 1f && normal.HasValue)
                        {
                            Console.WriteLine("Collision detected with bloc " + bloc.Type);
                            potentialCollisions.Add(new Tuple<float, Vector3>(entryTime, normal.Value));
                        }
            
                    }
                }
            }

            if (potentialCollisions.Count != 0)
            {
                var entryTime = float.MaxValue;
                var normal = new Vector3(0, 0, 0);
             
                for (int i = 0; i < potentialCollisions.Count; i++)
                {
                    if (potentialCollisions[i].Item1 < entryTime)
                    {
                        entryTime = potentialCollisions[i].Item1;
                        normal = potentialCollisions[i].Item2;
                    }
                }
                
                entryTime -= 0.01f;
                
                if (normal.X != 0)
                {
                    Velocity.X = 0;
                    Position.X += computedVelo.X * entryTime;
                }

                if (normal.Y != 0)
                {
                    Velocity.Y = 0;
                    Position.Y += computedVelo.Y * entryTime;
                }

                if (normal.Z != 0)
                {
                    Velocity.Z = 0;
                    Position.Z += computedVelo.Z * entryTime;
                }

                // if (normal.Y == 1)
                // {
                //     Grounded = true;
                // }
                
                
            }
        }

    }

    private void HandleInput(GameTime gameTime)
    {
        var keyboardState = Keyboard.GetState();
        float rotationSpeed = 0.4f;
        var mouseState = Mouse.GetState();
        
        Vector3 move = new Vector3(0, 0, 0);
            
        // cameraMode = CameraMode.FirstPerson;
        if (Input.IsKeyPressed(Keys.F5))
        {
            cameraMode += 1;
            if (cameraMode > CameraMode.FreeCamera)
                cameraMode = CameraMode.FirstPerson;
            Console.WriteLine("Camera mode: " + cameraMode);
        }
        
        
        if (keyboardState.IsKeyDown(Keys.Z))
            move.Z += Speed;
        if (keyboardState.IsKeyDown(Keys.S))
            move.Z -= Speed;
        if (keyboardState.IsKeyDown(Keys.Q))
            move.X += Speed;
        if (keyboardState.IsKeyDown(Keys.D))
            move.X -= Speed;
        if (keyboardState.IsKeyDown(Keys.Space))
            move.Y += Speed;
        if (keyboardState.IsKeyDown(Keys.LeftShift))
            move.Y -= Speed;
        
        move *=  Speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        Vector3 forwardNoPitch = Vector3.Transform(Vector3.Forward, Matrix.CreateRotationY(Camera.Rotation.Y));
        forwardNoPitch = Vector3.Normalize(forwardNoPitch);

        Velocity = new Vector3(
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
