#version 330

layout(location = 0) in vec3 vPosition;
layout(location = 1) in vec2 vTexCoord;
layout(location = 2) in vec3 vNormal;

uniform mat4 uProjectionMatrix;
uniform mat4 uViewMatrix;
uniform vec3 uPosition;
uniform vec3 uCameraPosition;

uniform vec3 uLightDirection;

out vec3 lightVector;
out vec3 eyeVector;
out vec2 texCoord;
out vec3 normal;

void main() {
    vec4 position = vec4(vPosition + uPosition, 1.0);
    vec4 worldPosition = (uViewMatrix * position);
    mat3 normalMatrix = transpose(inverse(mat3(uViewMatrix)));

    lightVector = normalize(vec4(uLightDirection, 1.0)).xyz;
    eyeVector = normalize(uCameraPosition - position.xyz);
    texCoord = vTexCoord;
    normal = vNormal;

    gl_Position = uProjectionMatrix * worldPosition;
}
