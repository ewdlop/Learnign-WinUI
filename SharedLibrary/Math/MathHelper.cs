using System;
using System.Numerics;

namespace SharedLibrary.Math;

public static class MathHelper
{
    public static float DegreesToRadians(float degrees) => MathF.PI / 180f * degrees;


    //Lerp between two floats
    public static float Lerp(this float a, float b, float t)
    {
        if(t is < 0 or > 1)
        {
        }
        return a + (b - a) * t;
    }

    public static double Lerp(this double a, double b, double t)
    {
        if (t is < 0 or > 1)
        {
        }
        return a + (b - a) * t;
    }

    public static Quaternion Slerp(Quaternion p, Quaternion q, float t)
    {
        if (p.Length() != 1f || q.Length() != 1f)
        {

        }
        return Quaternion.Lerp(p, q, t);
    }

    public static Quaternion Slerp(Quaternion p, Quaternion q, double t)
    {
        if (p.Length() != 1f || q.Length() != 1f)
        {

        }
        return Quaternion.Lerp(p, q, Convert.ToSingle(t));
    }
}