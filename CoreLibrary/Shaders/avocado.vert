#version 330 core

// Vertex attributes
layout (location = 0) in vec3 aPosition;
layout (location = 1) in vec3 aNormal;
layout (location = 2) in vec4 aTangent;
layout (location = 3) in vec2 aTexCoord;

// Uniforms
uniform mat4 uModelMatrix;
uniform mat4 uViewMatrix;
uniform mat4 uProjectionMatrix;
uniform mat3 uNormalMatrix; // Inverse transpose of model matrix (upper 3x3)

// Outputs to fragment shader
out vec3 FragPos;
out vec2 TexCoord;
out vec3 Normal;
out vec3 Tangent;
out vec3 Bitangent;
out mat3 TBN; // Tangent-Bitangent-Normal matrix for normal mapping

void main()
{
    // Transform position to world space
    vec4 worldPos = uModelMatrix * vec4(aPosition, 1.0);
    FragPos = worldPos.xyz;
    
    // Pass through texture coordinates
    TexCoord = aTexCoord;
    
    // Transform normal to world space
    Normal = normalize(uNormalMatrix * aNormal);
    
    // Transform tangent to world space
    Tangent = normalize(uNormalMatrix * aTangent.xyz);
    
    // Calculate bitangent using cross product
    // aTangent.w determines handedness of the tangent space
    Bitangent = normalize(cross(Normal, Tangent) * aTangent.w);
    
    // Construct TBN matrix for normal mapping in fragment shader
    TBN = mat3(Tangent, Bitangent, Normal);
    
    // Transform position to clip space
    gl_Position = uProjectionMatrix * uViewMatrix * worldPos;
} 