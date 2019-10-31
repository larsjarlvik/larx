#version 330
#include shadow-coords
#include calculate-light-vectors

layout(location = 0) in vec3 vPosition;
layout(location = 1) in vec2 vTexCoord;

uniform mat4 uProjectionMatrix;
uniform mat4 uViewMatrix;

out vec4 vert_position;
out vec2 vert_texCoord;
out vec3 vert_lightVector;
out vec3 vert_eyeVector;
out vec4 vert_shadowCoords;

void main() {
    vec4 worldPosition = uViewMatrix * vec4(vPosition, 1.0);
    vec3 normal = normalize(vec3(0, 1, 0));
    vec3 tangent = normalize((uViewMatrix * vec4(1, 0, 0, 0)).xyz);
    vec3[] vectors = calculateLightVectors(normal, tangent, vPosition, mat3(1.0));

    vert_texCoord = vTexCoord;
    vert_position = uProjectionMatrix * worldPosition;
    vert_lightVector = vectors[0];
    vert_eyeVector = vectors[1];
    vert_shadowCoords = getShadowCoords(vec4(vPosition, 1.0));

    gl_Position = vert_position;
}
