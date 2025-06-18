#version 330 core

// Basic vertex attributes
layout (location = 0) in vec3 aPos;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec2 aTexCoords;

// Uniforms following project conventions
uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

// Outputs to fragment shader
out vec3 fPos;
out vec3 fNormal;
out vec2 fTexCoords;

void main()
{
    // Transform position to world space
    vec4 worldPos = uModel * vec4(aPos, 1.0);
    fPos = worldPos.xyz;
    
    // Pass through texture coordinates
    fTexCoords = aTexCoords;
    
    // Transform normal to world space
    // Use transpose(inverse(uModel)) for non-uniform scaling
    fNormal = mat3(transpose(inverse(uModel))) * aNormal;
    
    // Transform position to clip space
    gl_Position = uProjection * uView * worldPos;
} 