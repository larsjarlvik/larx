#version 330

layout(location = 0) in vec3 vPosition;
layout(location = 1) in vec2 vTexCoord;

uniform mat4 uProjectionMatrix;
uniform mat4 uViewMatrix;

uniform vec3 uLightDirection;

out vec4 clipSpace;
out vec3 lightVector;
out vec3 normalVector;
out vec3 eyeVector;
out vec2 texCoord;

void main() {
    texCoord = vTexCoord;

    vec4 worldPosition = (uViewMatrix * vec4(vPosition, 1.0));

    lightVector = normalize(vec4(uLightDirection, 1.0)).xyz;
    eyeVector = -normalize(worldPosition).xyz;
    clipSpace = uProjectionMatrix * worldPosition;

    gl_Position = clipSpace;
}
