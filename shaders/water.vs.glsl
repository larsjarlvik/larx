#version 330

layout(location = 0) in vec3 vPosition;
layout(location = 1) in vec2 vTexCoord;

uniform mat4 uProjectionMatrix;
uniform mat4 uViewMatrix;

out vec4 clipSpace;
out vec2 texCoord;
out vec3 position;

#include shadow-coords
#include calculate-light-vectors

void main() {
    vec4 worldPosition = uViewMatrix * vec4(vPosition, 1.0);

    texCoord = vTexCoord;
    clipSpace = uProjectionMatrix * worldPosition;
    position = vPosition;

    vec3 normal = normalize(vec3(0, 1, 0));
    vec3 tangent = normalize((uViewMatrix * vec4(1, 0, 0, 0)).xyz);

    calculateLightVectors(normal, tangent, position.xyz);
    setShadowCoords(vec4(position, 1.0));

    gl_Position = clipSpace;
}
