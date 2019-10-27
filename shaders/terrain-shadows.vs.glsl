#version 330

layout(location = 0) in vec3 vPosition;

uniform mat4 uProjectionMatrix;
uniform mat4 uViewMatrix;

void main() {
    vec4 worldPosition = uViewMatrix * vec4(vPosition, 1.0);
    gl_ClipDistance[0] = vPosition.y + 0.05;
    gl_Position = uProjectionMatrix * worldPosition;
}