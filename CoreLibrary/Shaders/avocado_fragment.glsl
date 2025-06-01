#version 330 core

// Inputs from vertex shader
in vec3 fPos;
in vec3 fNormal;
in vec2 fTexCoords;
in vec3 fTangent;
in vec3 fBiTangent;
in mat3 fTBN;

// Material structure following project conventions
struct Material {
    sampler2D texture_diffuse1;    // Base color texture
    sampler2D texture_normal1;     // Normal map
    sampler2D texture_specular1;   // Metallic-roughness texture
};

// Light structure following project conventions
struct Light {
    vec3 position;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

uniform Material material;
uniform Light light;
uniform vec3 viewPos;

// Output
out vec4 FragColor;

// Function to get normal from normal map
vec3 GetNormalFromMap()
{
    vec3 tangentNormal = texture(material.texture_normal1, fTexCoords).xyz * 2.0 - 1.0;
    return normalize(fTBN * tangentNormal);
}

void main()
{
    // Sample textures
    vec4 baseColor = texture(material.texture_diffuse1, fTexCoords);
    vec3 metallicRoughness = texture(material.texture_specular1, fTexCoords).rgb;
    
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