#version 330

// TODO: Move to uniform
const float uFar = 900;

layout(location = 0) in vec3 vPosition;
layout(location = 1) in vec2 vTexCoord;

uniform mat4 uProjectionMatrix;
uniform mat4 uViewMatrix;
uniform vec3 uCameraPosition;

out vec2 texCoord;
out float yPosition;

void main() {
    vec3 position = vec3(uViewMatrix * vec4(vPosition * uFar + uCameraPosition, 1.0));

    yPosition = vPosition.y;
    texCoord = vTexCoord;
    gl_Position = uProjectionMatrix * vec4(position, 1.0);
}
