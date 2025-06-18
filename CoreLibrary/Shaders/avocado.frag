#version 330 core

// Inputs from vertex shader
in vec3 fPos;
in vec3 fNormal;
in vec2 fTexCoords;
in vec3 fTangent;
in vec3 fBiTangent;
in mat3 fTBN;

// Texture array uniforms - more flexible approach
#define MAX_DIFFUSE_TEXTURES 4
#define MAX_NORMAL_TEXTURES 4
#define MAX_SPECULAR_TEXTURES 4
#define MAX_HEIGHT_TEXTURES 4

uniform sampler2D texture_diffuse[MAX_DIFFUSE_TEXTURES];
uniform sampler2D texture_normal[MAX_NORMAL_TEXTURES];
uniform sampler2D texture_specular[MAX_SPECULAR_TEXTURES];
uniform sampler2D texture_height[MAX_HEIGHT_TEXTURES];

// Uniforms to specify how many textures of each type are actually used
uniform int num_diffuse_textures;
uniform int num_normal_textures;
uniform int num_specular_textures;
uniform int num_height_textures;

// Light structure following project conventions
struct Light {
    vec3 position;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

uniform Light light;
uniform vec3 viewPos;

// Output
out vec4 FragColor;

// Function to get normal from normal map
vec3 GetNormalFromMap()
{
    if (num_normal_textures > 0) {
        vec3 tangentNormal = texture(texture_normal[0], fTexCoords).xyz * 2.0 - 1.0;
        return normalize(fTBN * tangentNormal);
    }
    return normalize(fNormal); // Fallback to vertex normal if no normal map
}

// Function to sample diffuse textures (blend multiple if available)
vec4 GetDiffuseColor()
{
    if (num_diffuse_textures == 0) {
        return vec4(1.0); // Default white if no diffuse texture
    }
    
    vec4 result = texture(texture_diffuse[0], fTexCoords);
    
    // Blend additional diffuse textures if available
    for (int i = 1; i < num_diffuse_textures && i < MAX_DIFFUSE_TEXTURES; i++) {
        vec4 additional = texture(texture_diffuse[i], fTexCoords);
        result = mix(result, additional, 0.5); // Simple blending
    }
    
    return result;
}

// Function to sample specular/metallic-roughness textures
vec3 GetMetallicRoughness()
{
    if (num_specular_textures > 0) {
        return texture(texture_specular[0], fTexCoords).rgb;
    }
    return vec3(0.0, 0.5, 0.0); // Default: no metallic, medium roughness
}

void main()
{
    // Sample textures using helper functions
    vec4 baseColor = GetDiffuseColor();
    vec3 metallicRoughness = GetMetallicRoughness();
    
    // Extract material properties from metallic-roughness texture
    // Following glTF 2.0 specification: R=unused, G=roughness, B=metallic
    float roughness = metallicRoughness.g;
    float metallic = metallicRoughness.b;
    
    // Get normal from normal map
    vec3 norm = GetNormalFromMap();
    
    // Calculate lighting vectors
    vec3 lightDirection = normalize(light.position - fPos);
    vec3 viewDirection = normalize(viewPos - fPos);
    vec3 reflectDirection = reflect(-lightDirection, norm);
    
    // Ambient lighting
    vec3 ambient = light.ambient * baseColor.rgb;
    
    // Diffuse lighting
    float diff = max(dot(norm, lightDirection), 0.0);
    vec3 diffuse = light.diffuse * diff * baseColor.rgb;
    
    // Specular lighting with metallic/roughness consideration
    float shininess = mix(32.0, 128.0, 1.0 - roughness); // Higher roughness = lower shininess
    float spec = pow(max(dot(viewDirection, reflectDirection), 0.0), shininess);
    vec3 specular = light.specular * spec * mix(vec3(0.04), baseColor.rgb, metallic);
    
    // Combine lighting components
    vec3 result = ambient + diffuse + specular;
    
    FragColor = vec4(result, baseColor.a);
} 