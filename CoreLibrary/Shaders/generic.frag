#version 330 core
in vec3 inNormal;
in vec3 inPos;
in vec2 inUV;

struct Material {
    sampler2D texture_diffuse;
    sampler2D texture_normal;
};

struct Light {
    vec3 position;
    vec3 ambient;
    vec3 diffuse;
    vec3 specular;
};

uniform Material material;
uniform Light light;
uniform vec3 viewPos;

out vec4 FragColor;

vec3 GetNormalFromMap()
{
    vec2 uv = vec2(inUV.x, 1.0f-inUV.y);
    vec3 tangentNormal = texture(texture_normal, uv).xyz * 2.0f - 1.0f;

    vec3 q1  = dFdx(inPos);
    vec3 q2  = dFdy(inPos);
    vec2 st1 = dFdx(uv);
    vec2 st2 = dFdy(uv);

    vec3 N = normalize(inNormal);
    vec3 T = normalize(q1*st2.t - q2*st1.t);
    vec3 B = -normalize(cross(N, T));
    mat3 TBN = mat3(T, B, N);

    return normalize(TBN * tangentNormal);
}

void main()
{
      vec3 ambient = light.ambient * texture(material.diffuse, inUV).rgb;
      vec3 norm = normalize(fNormal);
      vec3 lightDirection = normalize(light.position - inPos);
      float diff = max(dot(norm, lightDirection), 0.0);
      vec3 diffuse = light.diffuse * (diff * texture(material.diffuse, inUV).rgb);
	
    //The resulting colour should be the amount of ambient colour + the amount of additional colour provided by the diffuse of the lamp + the specular amount
    vec3 result = ambient + diffuse;
    FragColor = vec4(result, 1.0);
}