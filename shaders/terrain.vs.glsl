#version 330
#include shadow-coords
#include calculate-light-vectors
#include clip

precision highp float;

layout(location = 0) in vec3 vPosition;
layout(location = 1) in vec2 vTexCoord;
layout(location = 2) in vec3 vNormal;
layout(location = 3) in vec3 vTangent;

uniform mat4 uProjectionMatrix;
uniform mat4 uViewMatrix;

out vec3 vert_position;
out vec2 vert_texCoord;
out vec3 vert_normal;
out vec3 vert_lightVector;
out vec3 vert_eyeVector;
out vec4 vert_shadowCoords;

void main()
{
    vec4 worldPosition = uViewMatrix * vec4(vPosition, 1.0);
    vec3[] vectors = calculateLightVectors(vNormal, vTangent, vPosition, mat3(1.0));

    vert_position = vPosition;
    vert_texCoord = vTexCoord;
    vert_normal = vNormal;
    vert_lightVector = vectors[0];
    vert_eyeVector = vectors[1];
    vert_shadowCoords = getShadowCoords(vec4(vert_position, 1.0));

    gl_ClipDistance[0] = clip(vert_position.xyz);
    gl_Position = uProjectionMatrix * worldPosition;
}
