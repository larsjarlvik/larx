#version 430

layout (location = 0) in vec2 vPosition;

uniform vec2 uPosition;
uniform float uSize;
uniform float uScale;

uniform mat4 uViewMatrix;
uniform mat4 uProjectionMatrix;

void main() {
    vec2 position = vec2(uPosition + vPosition * uSize) * uScale;
    gl_Position = uProjectionMatrix * uViewMatrix * vec4(position.x, 0.0, position.y, 1.0);
}