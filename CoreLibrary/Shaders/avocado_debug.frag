#version 330 core

// Interpolated data received from vertex shader
in vec3 FragPos;        // Fragment position in world space
in vec3 Normal;         // Normal in world space
in vec2 TexCoords;      // Texture coordinates

// Output color
out vec4 FragColor;

// Texture array definitions - flexible approach
#define MAX_DIFFUSE_TEXTURES 4
#define MAX_NORMAL_TEXTURES 4
#define MAX_SPECULAR_TEXTURES 4
#define MAX_HEIGHT_TEXTURES 4

// Texture array uniforms
uniform sampler2D texture_diffuse[MAX_DIFFUSE_TEXTURES];
uniform sampler2D texture_normal[MAX_NORMAL_TEXTURES];
uniform sampler2D texture_specular[MAX_SPECULAR_TEXTURES];
uniform sampler2D texture_height[MAX_HEIGHT_TEXTURES];

// Uniforms for specifying actual number of textures used
uniform int num_diffuse_textures;
uniform int num_normal_textures;
uniform int num_specular_textures;
uniform int num_height_textures;

void main()
{
    // Default color
    vec3 finalColor = vec3(0.5, 0.5, 0.5); // Gray default
    
    // Process diffuse textures
    if (num_diffuse_textures > 0) {
        vec3 diffuseColor = vec3(0.0);
        for (int i = 0; i < num_diffuse_textures && i < MAX_DIFFUSE_TEXTURES; i++) {
            diffuseColor += texture(texture_diffuse[i], TexCoords).rgb;
        }
        finalColor = diffuseColor / float(num_diffuse_textures);
    }
    
    // If there are specular textures, blend them into the final color
    if (num_specular_textures > 0) {
        vec3 specularColor = vec3(0.0);
        for (int i = 0; i < num_specular_textures && i < MAX_SPECULAR_TEXTURES; i++) {
            specularColor += texture(texture_specular[i], TexCoords).rgb;
        }
        specularColor = specularColor / float(num_specular_textures);
        
        // Simple additive blending
        finalColor = mix(finalColor, finalColor + specularColor * 0.3, 0.5);
    }
    
    // Basic normal influence (optional)
    if (num_normal_textures > 0) {
        // Use normal texture to adjust brightness
        vec3 normalFromTex = texture(texture_normal[0], TexCoords).rgb;
        float normalIntensity = length(normalFromTex);
        finalColor *= (0.7 + normalIntensity * 0.3);
    }
    
    // Simple depth-based shading (using normals)
    vec3 normalizedNormal = normalize(Normal);
    float lightingFactor = abs(normalizedNormal.z) * 0.3 + 0.7; // Simple Z-direction lighting
    finalColor *= lightingFactor;
    
    // Output final color
          FragColor = vec4(1.0f);
} 