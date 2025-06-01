#version 330 core

// Inputs from vertex shader
in vec3 FragPos;
in vec2 TexCoord;
in vec3 Normal;
in vec3 Tangent;
in vec3 Bitangent;
in mat3 TBN;

// Uniforms
uniform sampler2D uBaseColorTexture;
uniform sampler2D uMetallicRoughnessTexture;
uniform sampler2D uNormalTexture;

// Lighting uniforms
uniform vec3 uLightPosition;
uniform vec3 uLightColor;
uniform vec3 uViewPosition;
uniform float uLightIntensity;

// Material uniforms (can override texture values)
uniform float uMetallicFactor;
uniform float uRoughnessFactor;
uniform vec3 uBaseColorFactor;

// Output
out vec4 FragColor;

const float PI = 3.14159265359;

// Normal Distribution Function (GGX/Trowbridge-Reitz)
float DistributionGGX(vec3 N, vec3 H, float roughness)
{
    float a = roughness * roughness;
    float a2 = a * a;
    float NdotH = max(dot(N, H), 0.0);
    float NdotH2 = NdotH * NdotH;
    
    float nom = a2;
    float denom = (NdotH2 * (a2 - 1.0) + 1.0);
    denom = PI * denom * denom;
    
    return nom / denom;
}

// Geometry Function (Smith's method)
float GeometrySchlickGGX(float NdotV, float roughness)
{
    float r = (roughness + 1.0);
    float k = (r * r) / 8.0;
    
    float nom = NdotV;
    float denom = NdotV * (1.0 - k) + k;
    
    return nom / denom;
}

float GeometrySmith(vec3 N, vec3 V, vec3 L, float roughness)
{
    float NdotV = max(dot(N, V), 0.0);
    float NdotL = max(dot(N, L), 0.0);
    float ggx2 = GeometrySchlickGGX(NdotV, roughness);
    float ggx1 = GeometrySchlickGGX(NdotL, roughness);
    
    return ggx1 * ggx2;
}

// Fresnel Function (Schlick approximation)
vec3 fresnelSchlick(float cosTheta, vec3 F0)
{
    return F0 + (1.0 - F0) * pow(clamp(1.0 - cosTheta, 0.0, 1.0), 5.0);
}

vec3 getNormalFromMap()
{
    vec3 tangentNormal = texture(uNormalTexture, TexCoord).xyz * 2.0 - 1.0;
    return normalize(TBN * tangentNormal);
}

void main()
{
    // Sample textures
    vec4 baseColor = texture(uBaseColorTexture, TexCoord);
    vec3 metallicRoughness = texture(uMetallicRoughnessTexture, TexCoord).rgb;
    
    // Extract material properties
    vec3 albedo = baseColor.rgb * uBaseColorFactor;
    float metallic = metallicRoughness.b * uMetallicFactor;
    float roughness = metallicRoughness.g * uRoughnessFactor;
    float ao = 1.0; // Ambient occlusion (not provided in this model)
    
    // Get normal from normal map
    vec3 N = getNormalFromMap();
    vec3 V = normalize(uViewPosition - FragPos);
    
    // Calculate reflectance at normal incidence
    vec3 F0 = vec3(0.04);
    F0 = mix(F0, albedo, metallic);
    
    // Calculate lighting
    vec3 L = normalize(uLightPosition - FragPos);
    vec3 H = normalize(V + L);
    float distance = length(uLightPosition - FragPos);
    float attenuation = 1.0 / (distance * distance);
    vec3 radiance = uLightColor * uLightIntensity * attenuation;
    
    // Cook-Torrance BRDF
    float NDF = DistributionGGX(N, H, roughness);
    float G = GeometrySmith(N, V, L, roughness);
    vec3 F = fresnelSchlick(max(dot(H, V), 0.0), F0);
    
    vec3 kS = F;
    vec3 kD = vec3(1.0) - kS;
    kD *= 1.0 - metallic;
    
    vec3 numerator = NDF * G * F;
    float denominator = 4.0 * max(dot(N, V), 0.0) * max(dot(N, L), 0.0) + 0.0001;
    vec3 specular = numerator / denominator;
    
    float NdotL = max(dot(N, L), 0.0);
    vec3 Lo = (kD * albedo / PI + specular) * radiance * NdotL;
    
    // Ambient lighting (simplified)
    vec3 ambient = vec3(0.03) * albedo * ao;
    
    vec3 color = ambient + Lo;
    
    // HDR tonemapping (Reinhard)
    color = color / (color + vec3(1.0));
    
    // Gamma correction
    color = pow(color, vec3(1.0/2.2));
    
    FragColor = vec4(color, baseColor.a);
} 