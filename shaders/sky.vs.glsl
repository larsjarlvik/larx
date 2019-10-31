#version 330

layout(location = 0) in vec3 vPosition;
layout(location = 1) in vec2 vTexCoord;
layout(location = 2) in vec3 vNormal;

uniform mat4 uProjectionMatrix;
uniform mat4 uViewMatrix;
uniform vec3 uCameraPosition;
uniform float uFarPlane;

out vec2 vert_texCoord;
out vec3 vert_position;
out vec3 vert_normal;

void main() {
    vec3 worldPosition = vec3(uViewMatrix * vec4(vPosition * uFarPlane * 0.9 + uCameraPosition, 1.0));

    vert_position = vPosition;
    vert_texCoord = vTexCoord;
    vert_normal = vNormal;

    gl_Position = uProjectionMatrix * vec4(worldPosition, 1.0);
}
