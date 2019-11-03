#version 430

layout (location = 0) in vec2 vPosition;


uniform mat4 uViewMatrix;
uniform mat4 uProjectionMatrix;
uniform mat4 uLocalMatrix;
uniform mat4 uWorldMatrix;

void main() {
    vec2 localPosition = (uLocalMatrix * vec4(vPosition.x, 0.0, vPosition.y, 1.0)).xz;
    gl_Position = uProjectionMatrix * uViewMatrix * uWorldMatrix * vec4(localPosition.x, 0.0, localPosition.y, 1.0);
}