using System;
using Microsoft.Xna.Framework;
using ProjectM.Core.Collisions;
using ProjectM.Core.World;

namespace ProjectM.Core;



public class Bloc
{
    public BlocType Type { get; set; }
    
    public Vector3 MatrixPosition { get; set; }
    
    private Chunk chunk;
    public Vector3 WorldPosition => MatrixPosition + chunk.WorldPosition;
    
    private static readonly BBox SharedCollisionBox = new BBox(Vector3.Zero, Vector3.One);
    
    public Bloc(Vector3 matrixPosition, Chunk chunk, BlocType type)
    {
        this.chunk = chunk;
        MatrixPosition = matrixPosition;
        Type = type;
    }

    protected BlocModel Model { get; set; }
    
    public bool CheckCollision(BBox other, out BBox intersectionBox)
    {
        Console.WriteLine($"Checking collision at world position: {WorldPosition}");
        return SharedCollisionBox.Intersects(other, WorldPosition, out intersectionBox);
    }

    public bool CheckCollision(Vector3 point)
    {
        return SharedCollisionBox.Contains(point, WorldPosition);
    }
}

public enum BlocType
{
    Air = 0,
    Grass = 1,
    Stone = 2,
    Dirt = 3,
    Wood = 4,
    Leaves = 5,
}