#version 330

layout(location = 0) in vec3 vPosition;
layout(location = 1) in vec2 vTexCoord;
layout(location = 2) in vec3 vNormal;
layout(location = 3) in vec4 vTangent;

uniform mat4 uProjectionMatrix;
uniform mat4 uViewMatrix;
uniform vec3 uPosition;
uniform float uRotation;

out vec2 texCoord;
out vec3 normal;

#include shadow-coords
#include calculate-light-vectors

mat3 rotationYMatrix(float a) {
    return mat3(cos(a), 0, sin(a), 0, 1, 0, -sin(a), 0, cos(a));
}

void main() {
    mat3 rotation = rotationYMatrix(uRotation);
    vec4 position = vec4(vPosition * rotation + uPosition, 1.0);
    vec4 worldPosition = uViewMatrix * position;

    normal = vNormal;
    texCoord = vTexCoord;

    calculateLightVectors(normal, vTangent.xyz, position.xyz, rotation);
    setShadowCoords(position);

    gl_Position = uProjectionMatrix * worldPosition;
}
