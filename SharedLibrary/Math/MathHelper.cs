using System;

namespace SharedLibrary.Math;

public static class MathHelper
{
    public static float DegreesToRadians(float degrees) => MathF.PI / 180f * degrees;
}