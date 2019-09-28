#version 330

layout(location = 0) in vec3 vPosition;
layout(location = 1) in vec2 vTexCoord;
layout(location = 2) in vec3 vNormal;

uniform mat4 uProjectionMatrix;
uniform mat4 uViewMatrix;
uniform vec3 uPosition;

uniform vec3 uLightPosition;

out vec3 lightVector;
out vec3 normalVector;
out vec3 eyeVector;
out vec2 texCoord;

void main() {
    vec4 worldPosition = (uViewMatrix * vec4(vPosition + uPosition, 1.0));
    mat3 normalMatrix = transpose(inverse(mat3(uViewMatrix)));

    lightVector = normalize(uViewMatrix * vec4(uLightPosition, 1.0) - worldPosition).xyz;
    normalVector = normalize(normalMatrix * vNormal);
    eyeVector = -normalize(worldPosition).xyz;
    texCoord = vTexCoord;

    gl_Position = uProjectionMatrix * worldPosition;
}
