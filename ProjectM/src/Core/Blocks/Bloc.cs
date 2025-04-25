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
    
    private static readonly Collider collider = new Collider(Vector3.Zero, Vector3.One);
    
    public Bloc(Vector3 matrixPosition, Chunk chunk, BlocType type)
    {
        this.chunk = chunk;
        MatrixPosition = matrixPosition;
        Type = type;
    }

    protected BlocModel Model { get; set; }
    
    public Collider getCollider()
    {
        return collider + WorldPosition;
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