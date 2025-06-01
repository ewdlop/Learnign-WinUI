#version 330 core

in vec2 fTexCoords;
out vec4 FragColor;

void main()
{
    // Just output a bright green color to see if the mesh is rendering
    FragColor = vec4(0.0, 1.0, 0.0, 1.0);
} 