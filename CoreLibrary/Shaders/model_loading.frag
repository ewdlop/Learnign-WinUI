#version 330 core
in vec2 fUv;

uniform sampler2D texture_diffuse1;
uniform sampler2D texture_specular1;

out vec4 FragColor;

void main()
{
	vec4 diffuse = texture(texture_diffuse1, fUv);
	vec4 specular = texture(texture_specular1, fUv);
    FragColor = diffuse + specular;
}