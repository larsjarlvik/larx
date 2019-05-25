#version 330

layout(location = 0) in vec3 vPosition;
layout(location = 1) in vec2 vTexCoord;
layout(location = 2) in vec3 vNormal;

uniform mat4 uProjectionMatrix;
uniform mat4 uViewMatrix;
uniform vec3 position;

const vec3 lightPosition = vec3(5, 5, 5);

void main() {
    vec3 position = vec3(uViewMatrix * vec4(vPosition + position, 1.0));
    gl_Position = uProjectionMatrix * vec4(position, 1.0);
}
