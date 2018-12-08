#version 330

layout(location = 0) in vec3 vPosition;
layout(location = 1) in vec3 vColor;

uniform mat4 uProjectionMatrix;
uniform mat4 uViewMatrix;

out vec3 color;

void main(void)
{
    color = vColor;

    vec3 position = vec3(uViewMatrix * vec4(vPosition, 1.0));
    gl_Position = uProjectionMatrix * vec4(position, 1.0);
}
