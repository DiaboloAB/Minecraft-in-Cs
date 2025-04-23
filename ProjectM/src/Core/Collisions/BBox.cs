using System;
using Microsoft.Xna.Framework;

namespace ProjectM.Core.Collisions;

public class BBox
{
    public Vector3 Min { get; set; }
    public Vector3 Max { get; set; }

    public BBox(Vector3 min, Vector3 max)
    {
        Min = min;
        Max = max;
    }
    
    public bool Intersects(BBox other, Vector3 offset, out BBox intersectionBox)
    {
        intersectionBox = null;

        Vector3 translatedMin = Min + offset;
        Vector3 translatedMax = Max + offset;

        if ((translatedMin.X <= other.Max.X && translatedMax.X >= other.Min.X) &&
            (translatedMin.Y <= other.Max.Y && translatedMax.Y >= other.Min.Y) &&
            (translatedMin.Z <= other.Max.Z && translatedMax.Z >= other.Min.Z))
        {
            Console.WriteLine($"Collision detected at world position: {offset}");
            Vector3 intersectionMin = Vector3.Max(translatedMin, other.Min);
            Vector3 intersectionMax = Vector3.Min(translatedMax, other.Max);
            intersectionBox = new BBox(intersectionMin, intersectionMax);
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