#version 330
precision highp float;

layout(location = 0) in vec3 vPosition;
layout(location = 1) in vec2 vTexCoord;
layout(location = 2) in vec3 vNormal;
layout(location = 3) in vec3 vTangent;

uniform mat4 uProjectionMatrix;
uniform mat4 uViewMatrix;

out vec3 position;
out vec2 texCoord;
out vec3 normal;

#include shadow-coords
#include calculate-light-vectors
#include clip

void main()
{
    vec4 worldPosition = uViewMatrix * vec4(vPosition, 1.0);

    position = vPosition;
    texCoord = vTexCoord;
    normal = vNormal;

    clip(position);
    calculateLightVectors(normal, vTangent.xyz, position.xyz, mat3(1.0));
    setShadowCoords(vec4(position, 1.0));

    gl_Position = uProjectionMatrix * worldPosition;
}
