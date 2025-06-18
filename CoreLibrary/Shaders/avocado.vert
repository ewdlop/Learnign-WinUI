#version 330 core

// Vertex attributes matching the Mesh.cs Vertex struct
layout (location = 0) in vec3 aPosition;    // Vector3 Position
layout (location = 1) in vec3 aNormal;      // Vector3 Normal
layout (location = 2) in vec2 aTexCoords;   // Vector2 TexCoords
layout (location = 3) in vec3 aTangent;     // Vector3 Tangent
layout (location = 4) in vec3 aBiTangent;   // Vector3 BiTangent
// Note: color attribute is available at location 5 if needed

// Uniforms following project conventions
uniform mat4 uModel;
uniform mat4 uView;
uniform mat4 uProjection;

// Outputs to fragment shader
out vec3 fPos;
out vec3 fNormal;
out vec2 fTexCoords;
out vec3 fTangent;
out vec3 fBiTangent;
out mat3 fTBN; // Tangent-BiTangent-Normal matrix for normal mapping

void main()
{
    // Transform position to world space
    vec4 worldPos = uModel * vec4(aPosition, 1.0);
    fPos = worldPos.xyz;
    
    // Pass through texture coordinates
    fTexCoords = aTexCoords;
    
    // Transform normal vectors to world space
    // Use transpose(inverse(uModel)) for non-uniform scaling
    mat3 normalMatrix = mat3(transpose(inverse(uModel)));
    fNormal = normalize(normalMatrix * aNormal);
    fTangent = normalize(normalMatrix * aTangent);
    fBiTangent = normalize(normalMatrix * aBiTangent);
    
    // Construct TBN matrix for normal mapping
    fTBN = mat3(fTangent, fBiTangent, fNormal);
    
    // Transform position to clip space
    gl_Position = uProjection * uView * worldPos;
} 