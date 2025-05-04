using System;
using Microsoft.Xna.Framework;

namespace ProjectM.Utils;

public class MathUtilities
{
    public static int FloatToIntRoundDown(float value)
    {
        return (int)Math.Floor(value);
    }
    public static Vector3 RoundVector3(Vector3 vector)
    {
        return new Vector3(
            (int)Math.Floor(vector.X),
            (int)Math.Floor(vector.Y),
            (int)Math.Floor(vector.Z)
        );
    }
}