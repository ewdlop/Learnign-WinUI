#version 330 core

// Vertex attributes, matching the Vertex structure in Mesh.cs
layout (location = 0) in vec3 aPosition;    // Vector3 Position
layout (location = 1) in vec3 aNormal;      // Vector3 Normal
layout (location = 2) in vec2 aTexCoords;   // Vector2 TexCoords
// Note: Color attribute is available at location 5 if needed

// Uniforms following project conventions
uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

// Data passed to fragment shader
out vec3 FragPos;       // Fragment position in world space
out vec3 Normal;        // Normal in world space
out vec2 TexCoords;     // Texture coordinates

void main()
{
    // Calculate vertex position in world space
    FragPos = vec3(uModel * vec4(aPosition, 1.0));
    
    // Calculate normal in world space (properly handles non-uniform scaling)
    Normal = mat3(transpose(inverse(uModel))) * aNormal;
    
    // Pass through texture coordinates directly
    TexCoords = aTexCoords;
    
    // Calculate final vertex position
    gl_Position = uProjection * uView * vec4(FragPos, 1.0);
} 