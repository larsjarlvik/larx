#version 330
#include shadow-coords
#include clip

layout(location = 0) in vec3 vPosition;
layout(location = 1) in vec2 vTexCoord;
layout(location = 2) in vec3 vNormal;
layout(location = 3) in vec4 vTangent;
layout(location = 4) in vec3 iPosition;
layout(location = 5) in float iRotation;
layout(location = 6) in float iVariation;

uniform mat4 uProjectionMatrix;
uniform mat4 uViewMatrix;

out vec2 vert_texCoord;
out vec3 vert_normal;
out vec4 vert_shadowCoords;
out float vert_clip;
out float vert_variation;
out vec3 vert_tangent;
out vec3 vert_position;
out mat3 vert_rotation;

mat3 rotationYMatrix(float a) {
    return mat3(cos(a), 0, sin(a), 0, 1, 0, -sin(a), 0, cos(a));
}

void main() {
    mat3 rotation = rotationYMatrix(iRotation);
    vec4 position = vec4(vPosition * rotation + iPosition, 1.0);
    vec4 worldPosition = uViewMatrix * position;

    vert_normal = vNormal;
    vert_tangent = vTangent.xyz;
    vert_position = position.xyz;
    vert_rotation = rotation;
    vert_texCoord = vTexCoord;
    vert_shadowCoords = getShadowCoords(position);
    vert_clip = clip(position.xyz);
    vert_variation = iVariation;

    gl_Position = uProjectionMatrix * worldPosition;
}
