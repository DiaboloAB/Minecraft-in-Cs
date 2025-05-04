using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using ProjectM.Core.Collisions;
using ProjectM.Utils;

namespace ProjectM.Core;

public class Entity
{
    private readonly Vector3 FRICTION = new Vector3(20f, 20f, 20f);
    private readonly Vector3 DRAG_FLY = new Vector3(5f, 5f, 5f);
    private readonly Vector3 DRAG_JUMP = new Vector3(1.8f, 0f, 1.8f);
    private readonly Vector3 DRAG_FALL = new Vector3(1.8f, 0.4f, 1.8f);
    private readonly Vector3 FLYING_ACCELERATION = Vector3.Zero;
    private readonly Vector3 GRAVITY_ACCELERATION = new Vector3(0, -32f, 0);
    private float jumpHeight = 1.25f;

    protected Collider playerCollider;

    public Vector3 Position = Vector3.Zero;
    public Vector3 Size = Vector3.One;
    public Vector3 Velocity = Vector3.Zero;
    public Vector3 Acceleration = Vector3.Zero;
    public bool Grounded = false;
    public bool Flying = false;
    
    public Entity(Vector3 position, Vector3 size)
    {
        Position = position;
        Size = size;
        UpdateCollider();
    }
    
    private void UpdateCollider()
    {
        playerCollider = new Collider(
            Position - Size / 2,
            Position + Size / 2
        );
    }
    
    public void Teleport(Vector3 newPos)
    {
        Position = newPos;
        Velocity = Vector3.Zero;
    }
    
    public void Jump()
    {
        if (!Grounded)
            return;
        Velocity.Y = MathF.Sqrt(-2 * GRAVITY_ACCELERATION.Y * jumpHeight);
    }

    private Vector3 Friction()
    {
        if (Flying)
            return DRAG_FLY;
        else if (Grounded)
            return FRICTION;
        else if (Velocity.Y > 0)
            return DRAG_JUMP;
        return DRAG_FALL;
    }
    
    protected void Update(GameTime gameTime, World.World world)
    {
        Velocity += Acceleration * Friction() * (float)gameTime.ElapsedGameTime.TotalSeconds;
        Acceleration = Vector3.Zero;
        
        UpdateCollider();
        Grounded = false;
        
        for (int idx = 0; idx < 3; idx++)
        {
            Vector3 computedVelo =  Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;

            int stepX = computedVelo.X > 0 ? 1 : -1;
            int stepY = computedVelo.Y > 0 ? 1 : -1;
            int stepZ = computedVelo.Z > 0 ? 1 : -1;
            
            int stepsXz = (int)(Size.X * 2);
            int stepsY = (int)Size.Y * 2;
            
            List<Tuple<float, Vector3>> potentialCollisions = new List<Tuple<float, Vector3>>();
            
            Vector3 pos = MathUtilities.RoundVector3(Position);
            
            Vector3 newPos = Position + computedVelo;

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
                if (normal.Y == 1)
                {
                    Grounded = true;
                }
            }
        }
        Position += Velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
        Velocity += Flying ? FLYING_ACCELERATION : GRAVITY_ACCELERATION * (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        Velocity += new Vector3(
            -MathF.Min(MathF.Abs(Velocity.X * Friction().X * (float)gameTime.ElapsedGameTime.TotalSeconds), MathF.Abs(Velocity.X)) * MathF.Sign(Velocity.X),
            -MathF.Min(MathF.Abs(Velocity.Y * Friction().Y * (float)gameTime.ElapsedGameTime.TotalSeconds), MathF.Abs(Velocity.Y)) * MathF.Sign(Velocity.Y),
            -MathF.Min(MathF.Abs(Velocity.Z * Friction().Z * (float)gameTime.ElapsedGameTime.TotalSeconds), MathF.Abs(Velocity.Z)) * MathF.Sign(Velocity.Z)
        );
        
        UpdateCollider();
    }
    
    
}