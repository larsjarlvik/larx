#version 430
#include morph

layout (location = 0) in vec2 vPosition;

uniform int uLod;
uniform mat4 uLocalMatrix;
uniform mat4 uWorldMatrix;

out vec2 vs_texCoord;

void main() {
    vec2 position = (uLocalMatrix * vec4(vPosition.x, 0, vPosition.y, 1)).xz;
    if (uLod > 0) position += morph(uWorldMatrix, vPosition, uLod);

    vec4 worldPosition = uWorldMatrix * vec4(position.x, 0.0, position.y, 1.0);

    vs_texCoord = position;
    gl_Position = worldPosition;
}