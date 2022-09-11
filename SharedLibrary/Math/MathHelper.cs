using System;

namespace SharedLibrary.Math;

public static class MathHelper
{
    public static float DegreesToRadians(float degrees) => MathF.PI / 180f * degrees;


    //Lerp between two floats
    public static float Lerp(this float a, float b, float t)
    {
        return a + (b - a) * t;
    }
}