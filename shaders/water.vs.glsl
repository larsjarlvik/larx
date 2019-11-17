#version 330
#include shadow-coords
#include calculate-light-vectors

layout(location = 0) in vec3 vPosition;

uniform mat4 uProjectionMatrix;
uniform mat4 uViewMatrix;

out vec4 vert_position;
out vec2 vert_texCoord;
out LightVectors vert_lightVectors;
out vec4 vert_shadowCoords;

void main() {
    vec4 worldPosition = uViewMatrix * vec4(vPosition, 1.0);
    vec3 normal = normalize(vec3(0, 1, 0));
    vec3 tangent = normalize((uViewMatrix * vec4(1, 0, 0, 0)).xyz);

    vert_texCoord = vPosition.xz / 400.0;
    vert_position = uProjectionMatrix * worldPosition;
    vert_lightVectors = calculateLightVectors(normal, tangent, vPosition.xyz, mat3(1.0));
    vert_shadowCoords = getShadowCoords(vec4(vPosition, 1.0));

    gl_Position = vert_position;
}
