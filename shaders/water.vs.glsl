#version 330

layout(location = 0) in vec3 vPosition;
layout(location = 1) in vec2 vTexCoord;

uniform mat4 uProjectionMatrix;
uniform mat4 uViewMatrix;

uniform vec3 uLightPosition;

out vec4 clipSpace;
out vec3 lightVector;
out vec3 eyeVector;
out vec2 texCoord;

void main() {
    vec4 worldPosition = (uViewMatrix * vec4(vPosition, 1.0));

    texCoord = vTexCoord;
    eyeVector = -normalize(worldPosition).xyz;
    lightVector = normalize(uViewMatrix * vec4(uLightPosition, 1.0) - worldPosition).xyz;
    clipSpace = uProjectionMatrix * worldPosition;

    gl_Position = clipSpace;
}
