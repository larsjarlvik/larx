#version 330
precision highp float;

const int CLIP_BOTTOM = 1;
const int CLIP_TOP = 2;

layout(location = 0) in vec3 vPosition;
layout(location = 1) in vec2 vTexCoord;
layout(location = 2) in vec3 vNormal;

uniform mat4 uProjectionMatrix;
uniform mat4 uViewMatrix;
uniform vec3 uLightDirection;
uniform int uClipPlane;
uniform vec3 uCameraPosition;

out vec3 position;
out vec2 texCoord;
out vec3 normal;
out vec3 lightVector;
out vec3 eyeVector;

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

    vec4 worldPosition = uViewMatrix * vec4(position, 1.0);
    lightVector = normalize(vec4(uLightDirection, 1.0)).xyz;
    eyeVector = normalize(uCameraPosition - position.xyz);

    gl_Position = uProjectionMatrix * worldPosition;
}
