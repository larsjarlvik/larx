#version 330
#include shadow-coords
#include calculate-light-vectors
#include clip

layout(location = 0) in vec3 vPosition;
layout(location = 1) in vec2 vTexCoord;
layout(location = 2) in vec3 vNormal;
layout(location = 3) in vec4 vTangent;
layout(location = 4) in vec3 iPosition;
layout(location = 5) in float iRotation;

uniform mat4 uProjectionMatrix;
uniform mat4 uViewMatrix;

out vec2 vs_texCoord;
out vec3 vs_normal;
out LightVectors vs_lightVectors;
out vec4 vs_shadowCoords;
out vec3 vs_position;

mat3 rotationYMatrix(float a) {
    return mat3(cos(a), 0, sin(a), 0, 1, 0, -sin(a), 0, cos(a));
}

void main() {
    mat3 rotation = rotationYMatrix(iRotation);
    vec4 position = vec4(vPosition * rotation + iPosition, 1.0);
    vec4 worldPosition = uViewMatrix * position;

    vs_normal = vNormal;
    vs_texCoord = vTexCoord;
    vs_lightVectors = calculateLightVectors(vs_normal, vTangent.xyz, position.xyz, rotation);
    vs_shadowCoords = getShadowCoords(position);
    vs_position = position.xyz;

    gl_ClipDistance[0] = clip(position.xyz);
    gl_Position = uProjectionMatrix * worldPosition;
}
