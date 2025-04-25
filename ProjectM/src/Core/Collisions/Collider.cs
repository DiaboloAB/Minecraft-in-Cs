using System;
using System.Net.Mail;
using Microsoft.Xna.Framework;

namespace ProjectM.Core.Collisions;

public class Collider
{
    public Vector3 Min { get; set; }
    public Vector3 Max { get; set; }

    //operator overloads
    
    // + Vector3
    public static Collider operator +(Collider box, Vector3 offset)
    {
        return new Collider(box.Min + offset, box.Max + offset);
    }
    
    // - Vector3
    public static Collider operator -(Collider box, Vector3 offset)
    {
        return new Collider(box.Min - offset, box.Max - offset);
    }
    
    public Collider(Vector3 min, Vector3 max)
    {
        Min = min;
        Max = max;
    }
    
    
    float CalculateTime(float x, float y)
    {
        if (y == 0)
            return (x > 0) ? float.NegativeInfinity : float.PositiveInfinity;
        return x / y;
    }
    
    public (float entryTime, Vector3? normal) Collide(Collider with, Vector3 velocity)
    {
        var noCollision = (1f, (Vector3?)null);

        // find entry & exit times for each axis
        float vx = velocity.X;
        float vy = velocity.Y;
        float vz = velocity.Z;

        // Helper function to calculate time
        float CalculateTime(float x, float y)
        {
            if (y == 0)
                return (x > 0) ? float.NegativeInfinity : float.PositiveInfinity;
            return x / y;
        }

        float xEntry = CalculateTime(vx > 0 ? with.Min.X - this.Max.X : with.Max.X - this.Min.X, vx);
        float xExit = CalculateTime(vx > 0 ? with.Max.X - this.Min.X : with.Min.X - this.Max.X, vx);
        float yEntry = CalculateTime(vy > 0 ? with.Min.Y - this.Max.Y : with.Max.Y - this.Min.Y, vy);
        float yExit = CalculateTime(vy > 0 ? with.Max.Y - this.Min.Y : with.Min.Y - this.Max.Y, vy);
        float zEntry = CalculateTime(vz > 0 ? with.Min.Z - this.Max.Z : with.Max.Z - this.Min.Z, vz);
        float zExit = CalculateTime(vz > 0 ? with.Max.Z - this.Min.Z : with.Min.Z - this.Max.Z, vz);

        // make sure we actually got a collision
        if (xEntry < 0 && yEntry < 0 && zEntry < 0)
            return noCollision;
        if (xEntry > 1 || yEntry > 1 || zEntry > 1)
            return noCollision;

        // on which axis did we collide first?
        float entry = Math.Max(Math.Max(xEntry, yEntry), zEntry);
        float exit = Math.Min(Math.Min(xExit, yExit), zExit);

        if (entry > exit)
            return noCollision;

        // find normal of surface we collided with
        float nx = entry == xEntry ? (vx > 0 ? -1 : 1) : 0;
        float ny = entry == yEntry ? (vy > 0 ? -1 : 1) : 0;
        float nz = entry == zEntry ? (vz > 0 ? -1 : 1) : 0;

        return (entry, new Vector3(nx, ny, nz));
    }
    
    
    public bool Intersects(Collider other, out Collider intersectionBox)
    {
        intersectionBox = null;

        Vector3 translatedMin = Min;
        Vector3 translatedMax = Max;

        if ((translatedMin.X <= other.Max.X && translatedMax.X >= other.Min.X) &&
            (translatedMin.Y <= other.Max.Y && translatedMax.Y >= other.Min.Y) &&
            (translatedMin.Z <= other.Max.Z && translatedMax.Z >= other.Min.Z))
        {
            Vector3 intersectionMin = Vector3.Max(translatedMin, other.Min);
            Vector3 intersectionMax = Vector3.Min(translatedMax, other.Max);
            intersectionBox = new Collider(intersectionMin, intersectionMax);
            return true;
        }

        return false;
    }

    public bool Contains(Vector3 point, Vector3 offset)
    {
        Vector3 translatedMin = Min + offset;
        Vector3 translatedMax = Max + offset;

        return (point.X >= translatedMin.X && point.X <= translatedMax.X) &&
               (point.Y >= translatedMin.Y && point.Y <= translatedMax.Y) &&
               (point.Z >= translatedMin.Z && point.Z <= translatedMax.Z);
    }
}