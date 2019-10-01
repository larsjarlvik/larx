#version 330

layout(location = 0) in vec3 vPosition;
layout(location = 1) in vec2 vTexCoord;
layout(location = 2) in vec3 vNormal;

uniform mat4 uProjectionMatrix;
uniform mat4 uViewMatrix;
uniform vec3 uCameraPosition;
uniform float uFarPlane;

out vec2 texCoord;
out vec3 position;
out vec3 normal;

void main() {
    vec3 worldPosition = vec3(uViewMatrix * vec4(vPosition * uFarPlane * 0.9 + uCameraPosition, 1.0));

    position = vPosition;
    texCoord = vTexCoord;
    normal = vNormal;

    gl_Position = uProjectionMatrix * vec4(worldPosition, 1.0);
}
