﻿#version 330 core

#extension GL_ARB_separate_shader_objects : enable

layout (location = 0) in vec3 inNormal;
layout (location = 1) in vec3 inPos;
layout (location = 2) in vec2 inUV;

struct Material {
    sampler2D texture_diffuse1;
    sampler2D texture_normal1;
};

struct Light {
    vec3 position;
    vec3 ambient;
    vec3 diffuse;
};

uniform Material material;
uniform Light light;

out vec4 FragColor;

vec3 GetNormalFromMap()
{

    vec2 uv = vec2(inUV.x, 1.0f-inUV.y);
    vec3 tangentNormal = texture(material.texture_normal1, inUV).xyz * 2.0f - 1.0f;

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
    vec3 ambient = light.ambient * texture(material.texture_diffuse1, inUV).rgb;
    vec3 norm = GetNormalFromMap();
    vec3 lightDirection = normalize(light.position - inPos);
    float diff = max(dot(norm, lightDirection), 0.0);
    vec3 diffuse = light.diffuse * (diff * texture(material.texture_diffuse1, inUV).rgb);
	
    //The resulting colour should be the amount of ambient colour + the amount of additional colour provided by the diffuse of the lamp + the specular amount
    vec3 result = ambient + diffuse;
    FragColor = vec4(result, 1.0);
}