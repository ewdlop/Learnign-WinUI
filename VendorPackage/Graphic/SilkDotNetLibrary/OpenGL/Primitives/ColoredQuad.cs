﻿namespace SilkDotNetLibrary.OpenGL.Primitives;
public static class ColoredQuad
{
    public static readonly string VertexShader = @"
        #version 330 core //Using version GLSL version 3.3
        layout (location = 0) in vec4 vPos;
        
        void main()
        {
            gl_Position = vec4(vPos.x, vPos.y, vPos.z, 1.0);
        }";

    public static readonly string FragmentShader = @"
        #version 330 core
        out vec4 FragColor;
        void main()
        {
            FragColor = vec4(1.0f, 0.5f, 0.2f, 1.0f);
        }";

    //Vertex data, uploaded to the VBO.
    public static readonly float[] Vertices =
    {
        //X    Y      Z     R  G  B  A
        0.5f,  0.5f, 0.0f, 1, 0, 0, 1,
        0.5f, -0.5f, 0.0f, 0, 0, 0, 1,
        -0.5f, -0.5f, 0.0f, 0, 0, 1, 1,
        -0.5f,  0.5f, 0.5f, 0, 0, 0, 1
    };

    public static readonly uint[] Indices =
    {
            0, 1, 3,
            1, 2, 3
    };
}