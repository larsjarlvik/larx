#version 330
precision highp float;

const int CLIP_BOTTOM = 1;
const int CLIP_TOP = 2;

layout(location = 0) in vec3 vPosition;
layout(location = 1) in vec2 vTexCoord;
layout(location = 2) in vec3 vNormal;
layout(location = 3) in vec3 vTangent;

uniform mat4 uProjectionMatrix;
uniform mat4 uViewMatrix;
uniform vec3 uLightDirection;
uniform int uClipPlane;
uniform vec3 uCameraPosition;
uniform mat4 uShadowMatrix;
uniform float uShadowDistance;

out vec3 position;
out vec2 texCoord;
out vec3 normal;
out vec3 lightVector;
out vec3 eyeVector;
out vec4 shadowCoords;

void setShadowCoords(vec4 position, float distance) {
    float fade = distance - (uShadowDistance - 10.0);
    fade = fade / 10.0;
    shadowCoords = uShadowMatrix * position;
    shadowCoords.w = clamp(1.0 - fade, 0.0, 1.0);
}

void main()
{
    position = vPosition;
    texCoord = vTexCoord;
    normal = vNormal;

    if (uClipPlane == CLIP_BOTTOM) {
        gl_ClipDistance[0] = position.y + 0.05;
    } else if (uClipPlane == CLIP_TOP) {
        gl_ClipDistance[0] = -position.y + 0.05;
    }

    vec3 tangent = normalize((uViewMatrix * vec4(vTangent, 0.0)).xyz);
    vec3 biTangent = normalize(cross(normal, tangent));
    mat3 tangentSpace = mat3(
        tangent.x, biTangent.x, normal.x,
        tangent.y, biTangent.y, normal.y,
        tangent.z, biTangent.z, normal.z
    );
    lightVector = tangentSpace * -uLightDirection;
    eyeVector = tangentSpace * -(uCameraPosition - position);

    vec4 worldPosition = uViewMatrix * vec4(position, 1.0);

    gl_Position = uProjectionMatrix * worldPosition;
    setShadowCoords(vec4(position, 1.0), length(gl_Position));
}
